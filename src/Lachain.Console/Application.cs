﻿using System;
using System.Linq;
using System.Threading;
using Lachain.Core.Blockchain.Interface;
using Lachain.Core.Blockchain.Validators;
using Lachain.Core.Blockchain.Pool;
using Lachain.Core.Blockchain.Hardfork;
using Lachain.Core.Blockchain.SystemContracts;
using Lachain.Core.CLI;
using Lachain.Core.Config;
using Lachain.Core.Consensus;
using Lachain.Core.RPC.HTTP.Web3;
using Lachain.Core.DI;
using Lachain.Core.DI.Modules;
using Lachain.Core.DI.SimpleInjector;
using Lachain.Core.Network;
using Lachain.Core.RPC;
using Lachain.Core.ValidatorStatus;
using Lachain.Core.Vault;
using Lachain.Core.Network.FastSynchronizerBatch;
using Lachain.Crypto;
using Lachain.Logger;
using Lachain.Networking;
using Lachain.Storage.Repositories;
using Lachain.Storage.State;
using Lachain.Storage.Trie;
using Lachain.Utility.Utils;
using NLog;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using Lachain.Storage;
using Lachain.Core.Blockchain;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Lachain.Storage;
using Lachain.Storage.Trie;
using Lachain.Storage.State;
using Lachain.Utility.Utils;
using Lachain.Core.RPC.HTTP.Web3;
using Lachain.Crypto;
using Lachain.Utility.Serialization;
using Lachain.Proto;

namespace Lachain.Console
{
    public class Application : IBootstrapper, IDisposable
    {
        private readonly IContainer _container;
        private static readonly ILogger<Application> Logger = LoggerFactory.GetLoggerForClass<Application>();



        public Application(string configPath, RunOptions options)
        {
            var logLevel = options.LogLevel ?? Environment.GetEnvironmentVariable("LOG_LEVEL");
            if (logLevel != null) logLevel = char.ToUpper(logLevel[0]) + logLevel.ToLower().Substring(1);
            if (!new[] {"Trace", "Debug", "Info", "Warn", "Error", "Fatal"}.Contains(logLevel))
                logLevel = "Info";
            LogManager.Configuration.Variables["consoleLogLevel"] = logLevel;
            LogManager.ReconfigExistingLoggers();

            var containerBuilder = new SimpleInjectorContainerBuilder(new ConfigManager(configPath, options));
            containerBuilder.RegisterModule<BlockchainModule>();
            containerBuilder.RegisterModule<ConfigModule>();
            containerBuilder.RegisterModule<ConsensusModule>();
            containerBuilder.RegisterModule<NetworkModule>();
            containerBuilder.RegisterModule<StorageModule>();
            containerBuilder.RegisterModule<RpcModule>();
            containerBuilder.RegisterModule<ConsoleModule>();
            _container = containerBuilder.Build();
        }
        private static uint _next = 48821;

        private static uint Rand()
        {
            unchecked
            {
                _next = _next * 1103515245 + 12345;
                return _next / 2;
            }
        }

        public byte[] RandomBytes()
        {
            byte[] buffer = new byte[232];
            for(int i=0; i<58; i++)
            {
                var x = Rand();
                for (var j = 0; j < 4; ++j)
                    buffer[i * 4 + j] = (byte)((x >> (8 * j)) & 0xFF);
            }
            return buffer;
        }

        void Write(int n, IRocksDbContext dbContext)
        {
            Logger.LogInformation($"Data insertion started");
            RocksDbAtomicWrite tx = new RocksDbAtomicWrite(dbContext);
            for(int i=1; i<=n; i++)
            {
                tx.Put(EntryPrefix.PersistentHashMap.BuildPrefix((ulong)i), RandomBytes());
                if(i%1000==0)
                {
                    tx.Commit();
                    tx = new RocksDbAtomicWrite(dbContext);
                }
                if(i%1000000==0) Logger.LogInformation($"Insertion going on: {i}");
            }
            tx.Commit();
            Logger.LogInformation($"Data Insertion Ended");
        }
        void Read(int n, int q, IRocksDbContext dbContext)
        {
            Logger.LogInformation($"Data Query started");
            for(int i=1; i<=q; i++)
            {
                ulong j = (ulong)(Rand()%n) + 1;
                dbContext.Get(EntryPrefix.PersistentHashMap.BuildPrefix(j));
                if(i%1000000==0) Logger.LogInformation($"Query going on: {i}");
            }
            Logger.LogInformation($"Data Query Ended");
        }

        public async void Start(RunOptions options)
        {
            var configManager = _container.Resolve<IConfigManager>();
            var blockManager = _container.Resolve<IBlockManager>();
            var consensusManager = _container.Resolve<IConsensusManager>();
            var validatorStatusManager = _container.Resolve<IValidatorStatusManager>();
            var transactionVerifier = _container.Resolve<ITransactionVerifier>();
            var validatorManager = _container.Resolve<IValidatorManager>();
            var blockSynchronizer = _container.Resolve<IBlockSynchronizer>();
            var networkManager = _container.Resolve<INetworkManager>();
            var commandManager = _container.Resolve<IConsoleManager>();
            var rpcManager = _container.Resolve<IRpcManager>();
            var stateManager = _container.Resolve<IStateManager>();
            var wallet = _container.Resolve<IPrivateWallet>();
            var metricsService = _container.Resolve<IMetricsService>();
            var snapshotIndexRepository = _container.Resolve<ISnapshotIndexRepository>();
            var localTransactionRepository = _container.Resolve<ILocalTransactionRepository>();
            var NodeRetrieval = _container.Resolve<INodeRetrieval>();
            var dbContext = _container.Resolve<IRocksDbContext>();
            var storageManager = _container.Resolve<IStorageManager>();
            var transactionPool = _container.Resolve<ITransactionPool>();

        //    int n = 40000000;
        //    Write(n, dbContext);
        //    Read(n, n/4, dbContext);

            // set chainId from config
            var chainId = configManager.GetConfig<NetworkConfig>("network")?.ChainId;
            if (chainId == null || chainId == 0) throw new Exception("chainId is not defined in the config file");

            Logger.LogInformation($"Chainid {chainId}");
            TransactionUtils.SetChainId((int)chainId);

            var version = Assembly.GetEntryAssembly()!
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
            Logger.LogInformation($"Version: {version}");

            // set cycle and validatorCount
            StakingContract.Initialize(configManager.GetConfig<NetworkConfig>("network"));

            // set hardfork heights
            Logger.LogInformation($"Setting hardfork heights.");
            var hardforkConfig = configManager.GetConfig<HardforkConfig>("hardfork") ??
                    throw new Exception("No 'hardfork' section in config file");
            HardforkHeights.SetHardforkHeights(hardforkConfig);

            rpcManager.Start();
            
            if (options.RollBackTo.HasValue)
            {
                Logger.LogWarning($"Performing roll back to block {options.RollBackTo.Value}");
                var snapshot = snapshotIndexRepository.GetSnapshotForBlock(options.RollBackTo.Value);
                stateManager.RollbackTo(snapshot);
                wallet.DeleteKeysAfterBlock(options.RollBackTo.Value);
                stateManager.Commit();
                Logger.LogWarning($"Rollback to block {options.RollBackTo.Value} complete");
            }

        
            localTransactionRepository.SetWatchAddress(wallet.EcdsaKeyPair.PublicKey.GetAddress()); 

            if (blockManager.TryBuildGenesisBlock())
                Logger.LogInformation("Generated genesis block");

            var genesisBlock = stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(0)
                               ?? throw new Exception("Genesis block was not persisted");
            Logger.LogInformation("Genesis Block: " + genesisBlock.Hash.ToHex());
            Logger.LogInformation($" + prevBlockHash: {genesisBlock.Header.PrevBlockHash.ToHex()}");
            Logger.LogInformation($" + merkleRoot: {genesisBlock.Header.MerkleRoot.ToHex()}");
            Logger.LogInformation($" + nonce: {genesisBlock.Header.Nonce}");
            Logger.LogInformation($" + transactionHashes: {genesisBlock.TransactionHashes.ToArray().Length}");
            foreach (var s in genesisBlock.TransactionHashes)
                Logger.LogInformation($" + - {s.ToHex()}");
            Logger.LogInformation($" + hash: {genesisBlock.Hash.ToHex()}");

            Logger.LogInformation("Current block height: " + blockManager.GetHeight());
            Logger.LogInformation($"Node public key: {wallet.EcdsaKeyPair.PublicKey.EncodeCompressed().ToHex()}");
            Logger.LogInformation($"Node address: {wallet.EcdsaKeyPair.PublicKey.GetAddress().ToHex()}");

            if (options.SetStateTo.Any())
            {
                List<string> args = options.SetStateTo.ToList();
            //    System.Console.WriteLine(args);
                ulong blockNumber = 0;
                if( !(args is null) && args.Count>0)
                {
                    blockNumber = Convert.ToUInt64(args[0]);
                }
                FastSynchronizerBatch.StartSync(stateManager, dbContext, snapshotIndexRepository,
                                                storageManager.GetVersionFactory(), blockNumber);

            }
            /*    if(blockManager.GetHeight()==0)
                FastSynchronizerBatch.StartSync(stateManager, dbContext, snapshotIndexRepository,
                                                storageManager.GetVersionFactory(), 0); */

            var networkConfig = configManager.GetConfig<NetworkConfig>("network") ??
                                throw new Exception("No 'network' section in config file");

            metricsService.Start();
            networkManager.Start();
            transactionVerifier.Start();
            commandManager.Start(wallet.EcdsaKeyPair);

            // pending transactions are restored from pool repository to in-memory storage
            // it's important to restore pool after transactionVerifier and before blockSynchronizer starts
            transactionPool.Restore();

            blockSynchronizer.Start();
            Logger.LogInformation("Synchronizing blocks...");
            blockSynchronizer.SynchronizeWith(
                validatorManager.GetValidatorsPublicKeys((long) blockManager.GetHeight())
                    .Where(key => !key.Equals(wallet.EcdsaKeyPair.PublicKey))
            );
            Logger.LogInformation("Block synchronization finished, starting consensus...");
            consensusManager.Start(blockManager.GetHeight() + 1);
            validatorStatusManager.Start(false);

            System.Console.CancelKeyPress += (sender, e) =>
            {
                System.Console.WriteLine("Interrupt received. Exiting...");
                _interrupt = true;
                Dispose();
            };

            while (!_interrupt)
                Thread.Sleep(1000);
        }
        private bool _interrupt;

        public void Dispose()
        {
            _container.Dispose();
        }
    }
}
