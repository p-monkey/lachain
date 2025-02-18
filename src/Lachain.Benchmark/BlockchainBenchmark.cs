﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Google.Protobuf;
using Lachain.Core.Blockchain.Error;
using Lachain.Core.Blockchain.Interface;
using Lachain.Core.Blockchain.Operations;
using Lachain.Core.Blockchain.Pool;
using Lachain.Core.Blockchain.VM;
using Lachain.Core.CLI;
using Lachain.Core.Config;
using Lachain.Core.DI;
using Lachain.Core.DI.Modules;
using Lachain.Core.DI.SimpleInjector;
using Lachain.Core.Vault;
using Lachain.Crypto;
using Lachain.Crypto.ECDSA;
using Lachain.Crypto.Misc;
using Lachain.Logger;
using Lachain.Networking;
using Lachain.Proto;
using Lachain.Storage.State;
using Lachain.Utility;
using Lachain.Utility.Serialization;
using Lachain.Utility.Utils;
using Nethereum.Util;
using NLog;
using TransactionUtils = Lachain.Crypto.TransactionUtils;

namespace Lachain.Benchmark
{
    public class BlockchainBenchmark : IBootstrapper
    {
        private static readonly ILogger<BlockchainBenchmark> Logger =
            LoggerFactory.GetLoggerForClass<BlockchainBenchmark>();

        private readonly IContainer _container;

        private IBlockManager _blockManager = null!;
        private IConfigManager _configManager = null!;
        private IStateManager _stateManager = null!;
        private ITransactionBuilder _transactionBuilder = null!;
        private ITransactionPool _transactionPool = null!;
        private ITransactionSigner _transactionSigner = null!;
        private IPrivateWallet _wallet = null!;

        private static readonly ICrypto Crypto = CryptoProvider.GetCrypto();

        public BlockchainBenchmark()
        {
            if (Directory.Exists("ChainLachain"))
                Directory.Delete("ChainLachain", true);

            var containerBuilder =
                new SimpleInjectorContainerBuilder(new ConfigManager("config.json", new RunOptions()));

            containerBuilder.RegisterModule<BlockchainModule>();
            containerBuilder.RegisterModule<ConfigModule>();
            containerBuilder.RegisterModule<ConsensusModule>();
            containerBuilder.RegisterModule<NetworkModule>();
            containerBuilder.RegisterModule<StorageModule>();
            containerBuilder.RegisterModule<RpcModule>();
            containerBuilder.RegisterModule<ConsoleModule>();

            _container = containerBuilder.Build();
        }

        public void Start(RunOptions options)
        {
            _blockManager = _container.Resolve<IBlockManager>();
            _configManager = _container.Resolve<IConfigManager>();
            _stateManager = _container.Resolve<IStateManager>();
            _transactionBuilder = _container.Resolve<ITransactionBuilder>();
            _transactionPool = _container.Resolve<ITransactionPool>();
            _transactionSigner = _container.Resolve<ITransactionSigner>();
            _wallet = _container.Resolve<IPrivateWallet>();

            var keyPair = _wallet.EcdsaKeyPair;

            Console.WriteLine("-------------------------------");
            Console.WriteLine("Private Key: " + keyPair.PrivateKey.Encode().ToHex());
            Console.WriteLine("Public Key: " + keyPair.PublicKey.EncodeCompressed().ToHex());
            Console.WriteLine("Address: " + Crypto.ComputeAddress(keyPair.PublicKey.EncodeCompressed()).ToHex());
            Console.WriteLine("-------------------------------");

            if (_blockManager.TryBuildGenesisBlock())
                Console.WriteLine("Generated genesis block");

            var genesisBlock = _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(0);
            Console.WriteLine("Genesis Block: " + genesisBlock!.Hash.ToHex());
            Console.WriteLine($" + prevBlockHash: {genesisBlock.Header.PrevBlockHash.ToHex()}");
            Console.WriteLine($" + merkleRoot: {genesisBlock.Header.MerkleRoot.ToHex()}");
            Console.WriteLine($" + nonce: {genesisBlock.Header.Nonce}");
            Console.WriteLine($" + transactionHashes: {genesisBlock.TransactionHashes.Count}");
            foreach (var s in genesisBlock.TransactionHashes)
                Console.WriteLine($" + - {s.ToHex()}");
            Console.WriteLine($" + hash: {genesisBlock.Hash.ToHex()}");

            var address1 = "0xe3c7a20ee19c0107b9121087bcba18eb4dcb8576".HexToUInt160();
            var address2 = "0x6bc32575acb8754886dc283c2c8ac54b1bd93195".HexToUInt160();

            Console.WriteLine("-------------------------------");
            // Console.WriteLine("Current block header height: " + blockchainContext.CurrentBlockHeight);
            Console.WriteLine("-------------------------------");
            Console.WriteLine("Balance of LA 0x3e: " +
                              _stateManager.LastApprovedSnapshot.Balances.GetBalance(address1));
            Console.WriteLine("Balance of LA 0x6b: " +
                              _stateManager.LastApprovedSnapshot.Balances.GetBalance(address2));
            Console.WriteLine("-------------------------------");

            Logger.LogInformation($"Setting chainId");
            var chainId = _configManager.GetConfig<NetworkConfig>("network")?.ChainId ?? 42;
            var newChainId = _configManager.GetConfig<NetworkConfig>("network")?.NewChainId ?? chainId;
            TransactionUtils.SetChainId((int)chainId, newChainId);
            
            // _BenchTxProcessing(_transactionBuilder, _transactionSigner, keyPair);
            // _BenchOneTxInBlock(_transactionBuilder, _transactionSigner, keyPair);
            
            Console.WriteLine("---------------START - TX POOL BENCHMARK----------------");
            _Bench_Tx_Pool(_transactionBuilder, _transactionSigner, keyPair);
            Console.WriteLine("---------------END - TX POOL BENCHMARK----------------");
            
            Console.WriteLine("---------------START - BLOCK EMULATE BENCHMARK----------------");
            _Bench_Emulate_Block(_transactionBuilder, _transactionSigner, keyPair);
            Console.WriteLine("---------------END - BLOCK EMULATE BENCHMARK----------------");
            
            Console.WriteLine("---------------START - BLOCK EMULATE + EXECUTE BENCHMARK----------------");
            _Bench_Emulate_Execute_Tx(_transactionBuilder, _transactionSigner, keyPair);
            Console.WriteLine("---------------END - BLOCK EMULATE + EXECUTE BENCHMARK----------------");
            
            Console.WriteLine("---------------START - MULTIPLE BLOCKS EMULATE + EXECUTE BENCHMARK----------------");
            _Bench_Execute_Blocks(_transactionBuilder, _transactionSigner, keyPair);
            Console.WriteLine("---------------END - MULTIPLE BLOCKS EMULATE + EXECUTE BENCHMARK----------------");

            Environment.Exit(0);
        }

        private static void _Benchmark(string text, Func<int, int> action, uint tries)
        {
            var lastTime = TimeUtils.CurrentTimeMillis();
            var mod = tries / 100;
            if (mod == 0)
                mod = 1;
            for (var i = 0; i < tries; i++)
            {
                if (i % mod == 0)
                {
                    Console.CursorLeft = 0;
                    Console.Write($"{text} {100 * i / tries}%");
                }

                action(i);
            }

            var deltaTime = TimeUtils.CurrentTimeMillis() - lastTime;
            Console.CursorLeft = text.Length;
            Console.WriteLine($"{1000.0 * tries / deltaTime} RPS");
        }

        private void _BenchTxProcessing(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair)
        {
            var address1 = "0x6bc32575acb8754886dc283c2c8ac54b1bd93195".HexToUInt160();
            var address2 = "0xe3c7a20ee19c0107b9121087bcba18eb4dcb8576".HexToUInt160();

            var transactionPool = _container.Resolve<ITransactionPool>();

            const int txGenerate = 2000;
            const int txPerBlock = 1000;

            var byteCode =
                "0061736d010000000117056000017f60037f7f7f0060017f0060027f7f0060000002ae010903656e760d6765745f63616c6c5f73697a65000003656e760f636f70795f63616c6c5f76616c7565000103656e760b73797374656d5f68616c74000203656e760a6765745f73656e646572000203656e761063727970746f5f6b656363616b323536000103656e760c6c6f61645f73746f72616765000103656e760a7365745f72657475726e000303656e760c736176655f73746f72616765000103656e760b77726974655f6576656e74000103030204040405017001010105030100020615037f01419088040b7f00419088040b7f004190080b072d04066d656d6f727902000b5f5f686561705f6261736503010a5f5f646174615f656e640302057374617274000a0ae2260202000bdc2605027f077e037f037e027f23808080800041c0026b2200248080808000024002401080808080004104490d0041004104200041206a1081808080000240024002400240024002400240024002400240024002400240024002400240024020002802202201417f4a0d0020014197acb4e87d4c0d0120014198acb4e87d460d03200141a3f0caeb7d460d042001417f460d110c090b200141bf82bfc8014c0d01200141c082bfc801460d04200141f0c08a8c03460d05200141ddc5b5f703470d084105108280808000200041c0026a2480808080000f0b20014189bc9d9d7b460d05200141a98bf0dc7b470d07024010808080800041374b0d0041061082808080000b41044138200041206a10818080800020002903342102200029033c210320002903442104200029034c210520002903202106200029032821072000200028023036021820002007370310200020063703082000200537038001200020043703782000200337037020002002370368200041a0026a108380808000200020002d00a0023a008801200020002800a10236008901200020002900a50237008d01200020002800ad0236009501200020002f00b1023b009901200020002d00b3023a009b01200041b1016a200028029801360000200041f7013a00a00120002000290390013700a90120002000290388013700a101200041003b00b501200041a0016a4117200041e0016a108480808000200041e0016a4120200041a0026a108580808000200020002903a00222053703c001200020002903a80222043703c801200020002903b00222033703d001200020002903b80222023703d80120022108200029038001220621070240024020022006520d00200321082003200041f8006a2903002207520d00200421082004200041f0006a2903002207520d0020052108200520002903682207510d010b200820075a0d0041041082808080000b200041b8026a4200370300200041a0026a41106a4200370300200041a0026a41086a22094200370300200042003703a00220022006427f857c2003200041e8006a41106a290300427f857c22022003542004200041e8006a41086a290300427f857c2206200454200520002903682204427f857c2203200554220120062001ad7c2205507172220120022001ad7c2206507172ad7c2102200342017c21072003427f510d08200020073703a0020c090b2001450d0520014186fafb1e470d064100210141002d008c88808000410171450d0a41002802848880800022014183807c6a2209418080046d210a200941808004480d0d200a40001a4100410028028888808000200a6a3602888880800041002802848880800021010c0d0b200041f1016a4100360000200042003700e901200042003700e101200041f5003a00e001200041003b00f501200041e0016a4117200041a0026a108480808000200041a0026a4120200041206a108580808000200029032021022000290328210320002903302104200020002903383703f801200020043703f001200020033703e801200020023703e001200041e0016a4120108680808000200041c0026a2480808080000f0b024010808080800041cb004b0d0041061082808080000b410441cc00200041206a1081808080004105108280808000200041c0026a2480808080000f0b024010808080800041374b0d0041061082808080000b41044138200041206a10818080800020002903342102200029033c210320002903442104200029034c210720002903202105200029032821062000200028023036029801200020063703900120002005370388012000200737038001200020043703782000200337037020002002370368200041b1016a200028029801360000200041f7013a00a00120002000290390013700a90120002000290388013700a101200041003b00b501200041a0016a4117200041e0016a108480808000200041e0016a4120200041a0026a108580808000200020002903a00222083703c001200020002903a802220c3703c801200020002903b002220d3703d001200020002903b802220e3703d80142002105200041b8026a4200370300200041b0026a4200370300200041a8026a22014200370300200042003703a002420021060240200220087c220820025a0d0042012106200142013703000b200020083703a0022003200c7c220c20067c210802400240200c2003540d00200642005220085071450d010b42012105200041b0026a42013703000b200041a8026a20083703002004200d7c220620057c21030240024020062004540d0042002104200542005220035071450d010b42012104200041b8026a42013703000b200041a0026a41106a22092003370300200041c0016a41106a220a2003370300200041a0026a41186a220b2007200e7c20047c2203370300200041c0016a41086a220f200041a0026a41086a2201290300370300200041c0016a41186a22102003370300200020002903a0023703c001200020002903c0013703a0022000200f2903003703a8022000200a2903003703b002200020102903003703b802200041a0016a41116a20004188016a41106a280200360000200041003b00b501200041f7013a00a001200020004188016a41086a2903003700a90120002000290388013700a101200041a0016a4117200041e0016a108480808000200041e0016a4120200041a0026a10878080800020004180026a41116a41003600004200210320004200370089022000420037008102200041f5003a008002200041003b00950220004180026a4117200041e0016a108480808000200041e0016a4120200041a0026a108580808000200020002903a00222053703a001200020002903a80222063703a801200020002903b00222073703b001200020002903b80222083703b801200b42003703002009420037030020014200370300200042003703a002420021040240200220057c220520025a0d0042012104200142013703000b200020053703a002200041e8006a41086a290300220520067c220620047c21020240024020062005540d00200442005220025071450d010b42012103200041b0026a42013703000b200041a8026a2002370300200041f8006a290300220420077c220520037c21020240024020052004540d0042002104200342005220025071450d010b42012104200041b8026a42013703000b200041a0026a41106a2002370300200041a0016a41106a22012002370300200041a0016a41086a2209200041a0026a41086a290300370300200041a0026a41186a200041e8006a41186a29030020087c20047c2202370300200041a0016a41186a220a2002370300200020002903a0023703a001200020002903a0013703a002200020092903003703a802200020012903003703b0022000200a2903003703b80220004191026a410036000020004200370089022000420037008102200041003b009502200041f5003a00800220004180026a4117200041e0016a108480808000200041e0016a4120200041a0026a108780808000200041c0026a2480808080000f0b024010808080800041174b0d0041061082808080000b41044118200041a0016a10818080800020002903a001210220002903a8012103200020002802b0013602d001200020033703c801200020023703c00141002d0080888080002101200041f1016a20002802d001360000200020013a00e001200020002903c8013700e901200020002903c0013700e101200041003b00f501200041e0016a4117200041a0026a108480808000200041a0026a4120200041206a108580808000200029032021022000290328210320002903302104200020002903383703f801200020043703f001200020033703e801200020023703e001200041e0016a4120108680808000200041c0026a2480808080000f0b4105108280808000200041c0026a2480808080000f0b024010808080800041234b0d0041061082808080000b41044124200041e0016a10818080800020002903e001210220002903e801210320002903f001210420002903f8012105200041206a108380808000200020002d00203a00a001200020002800213600a101200020002900253700a5012000200028002d3600ad01200020002f00313b00b101200020002d00333a00b30120002005370338200020043703302000200337032820002002370320200041c0016a41116a20002802b001360000200041003b00d501200041f7013a00c001200020002903a8013700c901200020002903a0013700c101200041c0016a4117200041a0026a108480808000200041a0026a4120200041206a108780808000200041206a108380808000200020002d00203a00a001200020002800213600a101200020002900253700a5012000200028002d3600ad01200020002f00313b00b101200020002d00333a00b301200041c0016a41106a20002802b001360200200020002903a8013703c801200020002903a0013703c001200041a0026a41116a4100360000200042003700a902200042003700a102200041003b00b502200041b7013a00a002200041a0026a4117200041206a108480808000200041206a4120200041c0016a108780808000200041386a4200370300200041206a41106a4200370300200041286a4200370300200042003703202000419ed6f2d67b3602a002200041206a200041a0026a4104108880808000200041c0026a2480808080000f0b4105108280808000200041c0026a2480808080000f0b20094201370300200020073703a002200542017c2205500d010b200041a8026a20053703000c020b200041a8026a2005370300200041b0026a22014201370300200642017c220650450d0120012006370300200041b8026a4201370300200242017c21020c020b41004100360284888080003f0021094100410136028c888080004100200941016a360288888080000c020b200041b0026a20063703000b200041a0026a41186a22092002370300200041c0016a41186a220a2002370300200041c0016a41086a220b200041a0026a41086a2201290300370300200041c0016a41106a220f200041a0026a41106a2210290300370300200020002903a0023703c001200020002903c0013703a0022000200b2903003703a8022000200f2903003703b0022000200a2903003703b802200041a0016a41116a20004188016a41106a280200360000200041003b00b501200041f7013a00a001200020004188016a41086a2903003700a90120002000290388013700a101200041a0016a4117200041e0016a108480808000200041e0016a4120200041a0026a10878080800020004180026a41116a200041086a41106a280200360000200041f7013a0080022000200041086a41086a290300370089022000200029030837008102200041003b00950220004180026a4117200041e0016a108480808000200041e0016a4120200041a0026a108580808000200020002903a00222053703a001200020002903a80222063703a801200020002903b00222073703b001200020002903b80222083703b80142002103200942003703002010420037030020014200370300200042003703a002420021020240200420057c220520045a0d0042012102200142013703000b200020053703a002200041e8006a41086a290300220520067c220620027c21040240024020062005540d00200242005220045071450d010b42012103200041b0026a42013703000b200041a8026a2004370300200041f8006a290300220420077c220520037c21020240024020052004540d0042002104200342005220025071450d010b42012104200041b8026a42013703000b200041a0026a41106a2002370300200041a0016a41106a22012002370300200041a0016a41086a2209200041a0026a41086a290300370300200041a0026a41186a200041e8006a41186a29030020087c20047c2202370300200041a0016a41186a220a2002370300200020002903a0023703a001200020002903a0013703a002200020092903003703a802200020012903003703b0022000200a2903003703b80220004191026a200041086a41106a280200360000200041003b009502200041f7013a0080022000200041086a41086a29030037008902200020002903083700810220004180026a4117200041e0016a108480808000200041e0016a4120200041a0026a108780808000200041013a00a002200041a0026a4101108680808000200041c0026a2480808080000f0b410021094100200141036a418080046f36028488808000200141c2a8013b0000200141c3003a00020240024041002d008c88808000410171450d0041002802848880800022094187807c6a220a418080046d210b200a41808004480d01200b40001a4100410028028888808000200b6a3602888880800041002802848880800021090c010b41004100360284888080003f00210a4100410136028c888080004100200a41016a360288888080000b4100200941076a418080046f3602848880800020094103360200200920012d00003a0004200920012d00013a0005200920012d00023a0006200941071086808080000240024041002d008c888080002209410171450d002001450d030c010b41004100360284888080003f0021094100410136028c888080004100200941016a36028888808000410121092001450d020b20094101710d0141004100360284888080003f0021014100410136028c888080004100200141016a36028888808000200041c0026a2480808080000f0b41051082808080000b200041c0026a2480808080000b0b1a02004180080b01f7004184080b0c00000000000000000000000000a301046e616d65019b010b000d6765745f63616c6c5f73697a65010f636f70795f63616c6c5f76616c7565020b73797374656d5f68616c74030a6765745f73656e646572041063727970746f5f6b656363616b323536050c6c6f61645f73746f72616765060a7365745f72657475726e070c736176655f73746f72616765080b77726974655f6576656e7409115f5f7761736d5f63616c6c5f63746f72730a05737461727400250970726f647563657273010c70726f6365737365642d62790105636c616e6705392e302e30"
                    .HexToBytes();
            var deployAbi =
                ContractEncoder.Encode("constructor(address,uint256)", address1, Money.FromDecimal(10000000));
            var deployTx = transactionBuilder.DeployTransaction(address1, byteCode, deployAbi);

            var deployError = transactionPool.Add(transactionSigner.Sign(deployTx, keyPair, true));
            if (deployError != OperatingError.Ok)
                throw new Exception("Unable to add deploy tx (" + deployError + ")");
            var contract = deployTx.From.ToBytes().Concat(deployTx.Nonce.ToBytes()).Ripemd();

            _Benchmark("Building TX pool... ", i =>
            {
                var tx = transactionBuilder.TokenTransferTransaction(contract, address1, address2,
                    Money.FromDecimal(1.2m));
                tx.Nonce += (ulong)i + 1;
                var error = transactionPool.Add(transactionSigner.Sign(tx, keyPair, true));
                if (error != OperatingError.Ok)
                    throw new Exception("Unable to add transcation to pool (" + error + ")");
                return i;
            }, txGenerate);

            var blocks = new BlockWithTransactions[transactionPool.Size() / txPerBlock];

            _Benchmark("Generating blocks... ", i =>
            {
                var txs = transactionPool.Peek(txPerBlock, txPerBlock);
                // var latestBlock = blockchainContext.CurrentBlock;
                // if (i > 0)
                //     latestBlock = blocks[i - 1].Block;
                // var blockWithTxs = new BlockBuilder(latestBlock.Header)
                //     .WithTransactions(txs)
                //     .Build(123456);
                // var block = blockWithTxs.Block;
                // block.Multisig = new MultiSig
                // {
                //     Quorum = 1,
                //     Signatures =
                //     {
                //         new MultiSig.Types.SignatureByValidator
                //         {
                //             Key = keyPair.PublicKey,
                //             Value = blockManager.Sign(block.Header, keyPair)
                //         }
                //     },
                //     Validators = {keyPair.PublicKey}
                // };
                // blocks[i] = blockWithTxs;
                return i;
            }, transactionPool.Size() / txPerBlock);

            var currentTime = TimeUtils.CurrentTimeMillis();

            _Benchmark("Processing blocks... ", i =>
            {
                var blockWithTxs = blocks[i];
                // blockchainManager.PersistBlockManually(blockWithTxs.Block, blockWithTxs.Transactions);
                return i;
            }, (uint)blocks.Length);

            var elapsedTime = TimeUtils.CurrentTimeMillis() - currentTime;
            Console.WriteLine($"Transaction processing... {1000.0 * txGenerate / elapsedTime} TPS");
        }

        private static void _BenchOneTxInBlock(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair
        )
        {
            var address1 = "0x6bc32575acb8754886dc283c2c8ac54b1bd93195".HexToUInt160();
            var address2 = "0xe3c7a20ee19c0107b9121087bcba18eb4dcb8576".HexToUInt160();
            var lastTime = TimeUtils.CurrentTimeMillis();
            const int tries = 100;
            for (var i = 0; i < tries; i++)
            {
                if (i % 10 == 0)
                {
                    Console.CursorLeft = 0;
                    Console.Write($"Benchmarking... {100 * i / tries}%");
                }

                var transferTx =
                    transactionBuilder.TransferTransaction(address1, address2, Money.FromDecimal(1.2m));
                var signed = transactionSigner.Sign(transferTx, keyPair, true);
                // var blockWithTxs = new BlockBuilder(latestBlock.Header)
                //     .WithTransactions(new[] {signed})
                //     .Build(123456);
                // // var stateHash = blockchainManager.CalcStateHash(blockWithTxs.Block, blockWithTxs.Transactions);
                // var block = blockWithTxs.Block;
                // // block.Header.StateHash = stateHash;
                // block.Hash = block.Header.Keccak();
                // block.Multisig = new MultiSig
                // {
                //     Quorum = 1,
                //     Signatures =
                //     {
                //         new MultiSig.Types.SignatureByValidator
                //         {
                //             Key = keyPair.PublicKey,
                //             Value = blockManager.Sign(block.Header, keyPair)
                //         }
                //     },
                //     Validators = {keyPair.PublicKey}
                // };
                // // blockchainManager.PersistBlockManually(block, blockWithTxs.Transactions);
            }

            var deltaTime = TimeUtils.CurrentTimeMillis() - lastTime;
            Console.CursorLeft = "Benchmarking... ".Length;
            Console.WriteLine($"{1000 * tries / deltaTime} RPS");
        }

        private void _Bench_Emulate_Block(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair)
        {
            const int txGenerate = 1000;
            const int txPerBlock = 1000;
            
            Logger.LogInformation($"Setting initial balance for the 'From' address");
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(keyPair.PublicKey.GetAddress(),
                Money.Parse("200000"));

            var txReceipts = new List<TransactionReceipt>();
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < txGenerate; i++)
            {
                var randomValue = new Random().Next(1, 100);
                var amount = Money.Parse($"{randomValue}.0").ToUInt256();

                byte[] random = new byte[32];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(random);

                var tx = new Transaction
                {
                    To = random.Slice(0, 20).ToUInt160(),
                    From = keyPair.PublicKey.GetAddress(),
                    GasPrice = (ulong)Money.Parse("0.0000001").ToWei(),
                    GasLimit = 100000000,
                    Nonce = (ulong)i,
                    Value = amount
                };

                txReceipts.Add(transactionSigner.Sign(tx, keyPair,  true));    
            }
            watch.Stop();
            Console.WriteLine($"Building TXs Time: {watch.ElapsedMilliseconds} ms");

            Block block = null!;
            watch.Restart();
            for (int i = 0; i < txGenerate / txPerBlock; i++)
            {
                block = BuildBlock(txReceipts.ToArray());    
            }
            watch.Stop();
            Console.WriteLine($"Building Block Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            for (int i = 0; i < txGenerate / txPerBlock; i++)
            {
                EmulateBlock(block, txReceipts.ToArray());
            }
            watch.Stop();
            Console.WriteLine($"Block Emulation Time: {watch.ElapsedMilliseconds} ms");
        }
        
        private void _Bench_Emulate_Execute_Tx(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair)
        {
            const int txGenerate = 1000;
            const int txPerBlock = 1000;

            Logger.LogInformation($"Setting initial balance for the 'From' address");
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(keyPair.PublicKey.GetAddress(),
                Money.Parse("200000"));

            var txReceipts = new List<TransactionReceipt>();
            
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < txGenerate; i++)
            {
                var randomValue = new Random().Next(1, 100);
                var amount = Money.Parse($"{randomValue}.0").ToUInt256();

                byte[] random = new byte[32];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(random);

                var tx = new Transaction
                {
                    To = random.Slice(0, 20).ToUInt160(),
                    From = keyPair.PublicKey.GetAddress(),
                    GasPrice = (ulong)Money.Parse("0.0000001").ToWei(),
                    GasLimit = 100000000,
                    Nonce = (ulong)i,
                    Value = amount
                };

                txReceipts.Add(transactionSigner.Sign(tx, keyPair,  true));    
            }
            watch.Stop();
            Console.WriteLine($"Building TXs Time: {watch.ElapsedMilliseconds} ms");
            
            Block block = null!;
            watch.Restart();
            for (int i = 0; i < txGenerate / txPerBlock; i++)
            {
                block = BuildBlock(txReceipts.ToArray());    
            }
            watch.Stop();
            Console.WriteLine($"Building Block Time: {watch.ElapsedMilliseconds} ms");

            watch.Restart();
            for (int i = 0; i < txGenerate / txPerBlock; i++)
            {
                ExecuteBlock(block, txReceipts.ToArray());
            }
            watch.Stop();
            Console.WriteLine($"Block Emulation + Execution Time: {watch.ElapsedMilliseconds} ms");

            var executedBlock = _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(block!.Header.Index);
            Console.WriteLine($"Executed Transactions: {executedBlock!.TransactionHashes.Count}");
            Console.WriteLine(
                $"Balance After Transaction {_stateManager.LastApprovedSnapshot.Balances.GetBalance(keyPair.PublicKey.GetAddress())}");
        }
        
        private void _Bench_Tx_Pool(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair)
        {
            const int txGenerate = 1000;
            const int txPerBlock = 1000;

            Logger.LogInformation($"Setting initial balance for the 'From' address");
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(keyPair.PublicKey.GetAddress(),
                Money.Parse("200000"));
            
            var txReceipts = new List<TransactionReceipt>();
            for (int i = 0; i < txGenerate; i++)
            {
                var randomValue = new Random().Next(1, 100);
                var amount = Money.Parse($"{randomValue}.0").ToUInt256();

                byte[] random = new byte[32];
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                rng.GetBytes(random);

                var tx = new Transaction
                {
                    To = random.Slice(0, 20).ToUInt160(),
                    From = keyPair.PublicKey.GetAddress(),
                    GasPrice = (ulong)Money.Parse("0.0000001").ToWei(),
                    GasLimit = 100000000,
                    Nonce = (ulong)i,
                    Value = amount
                };
                txReceipts.Add(transactionSigner.Sign(tx, keyPair, true));
            }
            
            ITransactionPool transactionPool = _container.Resolve<ITransactionPool>();
            var watch = System.Diagnostics.Stopwatch.StartNew();
            foreach (var txr in txReceipts)
            {
                transactionPool.Add(txr, false);
            }
            watch.Stop();
            Console.WriteLine($"Time to Add {transactionPool.Transactions.Count} Tx to pool: {watch.ElapsedMilliseconds} ms");
            
            watch.Restart();
            var txs = transactionPool.Peek(txGenerate, txGenerate);
            watch.Stop();
            Console.WriteLine($"Time to Peek {txs.Count} Tx from pool: {watch.ElapsedMilliseconds} ms");
        }

        private void _Bench_Execute_Blocks(
            ITransactionBuilder transactionBuilder,
            ITransactionSigner transactionSigner,
            EcdsaKeyPair keyPair)
        {
            const int txGenerate = 50;
            const int txPerBlock = 10;

            Logger.LogInformation($"Setting initial balance for the 'From' address");
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(keyPair.PublicKey.GetAddress(),
                Money.Parse("2000000"));

            for (var k = 0; k < txGenerate / txPerBlock; k++)
            {
                var txReceipts = new List<TransactionReceipt>();
                var watch = System.Diagnostics.Stopwatch.StartNew();
                for (int i = 0; i < txPerBlock; i++)
                {
                    var randomValue = new Random().Next(1, 100);
                    var amount = Money.Parse($"{randomValue}.0").ToUInt256();

                    byte[] random = new byte[32];
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    rng.GetBytes(random);
                    
                    var tx = new Transaction
                    {
                        To = random.Slice(0, 20).ToUInt160(),
                        From = keyPair.PublicKey.GetAddress(),
                        GasPrice = (ulong)Money.Parse("0.0000001").ToWei(),
                        GasLimit = 100000000,
                        Nonce = _transactionPool.GetNextNonceForAddress(keyPair.PublicKey.GetAddress()) + (ulong)i,
                        Value = amount
                    };
                    
                    txReceipts.Add(transactionSigner.Sign(tx, keyPair, true));
                }

                watch.Stop();
                Console.WriteLine($"Building TXs Time: {watch.ElapsedMilliseconds} ms");

                Block block = null!;
                watch.Restart();
                block = BuildBlock(txReceipts.ToArray());
                watch.Stop();
                Console.WriteLine($"Building Block Time: {watch.ElapsedMilliseconds} ms");

                watch.Restart();
                ExecuteBlock(block, txReceipts.ToArray());
                watch.Stop();
                Console.WriteLine($"Block Emulation + Execution Time: {watch.ElapsedMilliseconds} ms");
                
                var executedBlock =
                    _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(block!.Header.Index);
                Console.WriteLine($"Executed Transactions: {executedBlock!.TransactionHashes.Count}");
                Console.WriteLine(
                    $"Balance After Transaction {_stateManager.LastApprovedSnapshot.Balances.GetBalance(keyPair.PublicKey.GetAddress())}");
            }
        }

        private Block BuildBlock(TransactionReceipt[]? receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };

            var merkleRoot = UInt256Utils.Zero;

            if (receipts.Any())
                merkleRoot = MerkleTree.ComputeRoot(receipts.Select(tx => tx.Hash).ToArray()) ??
                             throw new InvalidOperationException();

            var predecessor =
                _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(_stateManager.LastApprovedSnapshot.Blocks
                    .GetTotalBlockHeight());
            var (header, multisig) =
                BuildHeaderAndMultisig(merkleRoot, predecessor, _stateManager.LastApprovedSnapshot.StateHash);

            return new Block
            {
                Header = header,
                Hash = header.Keccak(),
                Multisig = multisig,
                TransactionHashes = { receipts.Select(tx => tx.Hash) },
            };
        }

        private (BlockHeader, MultiSig) BuildHeaderAndMultisig(UInt256 merkleRoot, Block? predecessor,
            UInt256 stateHash)
        {
            var blockIndex = predecessor!.Header.Index + 1;
            var header = new BlockHeader
            {
                Index = blockIndex,
                PrevBlockHash = predecessor!.Hash,
                MerkleRoot = merkleRoot,
                StateHash = stateHash,
                Nonce = blockIndex
            };

            var keyPair = _wallet.EcdsaKeyPair;

            var headerSignature = Crypto.SignHashed(
                header.Keccak().ToBytes(),
                keyPair.PrivateKey.Encode(), true
            ).ToSignature(true);

            var multisig = new MultiSig
            {
                Quorum = 1,
                Validators = { _wallet.EcdsaKeyPair.PublicKey },
                Signatures =
                {
                    new MultiSig.Types.SignatureByValidator
                    {
                        Key = _wallet.EcdsaKeyPair.PublicKey,
                        Value = headerSignature,
                    }
                }
            };
            return (header, multisig);
        }

        private void EmulateBlock(Block block, TransactionReceipt[]? receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };
            _blockManager.Emulate(block, receipts);
        }
        
        private OperatingError ExecuteBlock(Block block, TransactionReceipt[]? receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };

            var (_, _, stateHash, _) = _blockManager.Emulate(block, receipts);

            var predecessor =
                _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(_stateManager.LastApprovedSnapshot.Blocks
                    .GetTotalBlockHeight());
            var (header, multisig) = BuildHeaderAndMultisig(block.Header.MerkleRoot, predecessor, stateHash);

            block.Header = header;
            block.Multisig = multisig;
            block.Hash = header.Keccak();

            var status = _blockManager.Execute(block, receipts, true, true);
            Logger.LogInformation($"Executed block: {block.Header.Index}");
            return status;
        }
    }
}