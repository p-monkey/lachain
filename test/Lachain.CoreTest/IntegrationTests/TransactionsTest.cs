using System.IO;
using Lachain.Proto;

using System.Reflection;
using Lachain.Core.Blockchain.Error;
using Lachain.Core.Blockchain.Pool;
using Lachain.Core.Blockchain.Operations;
using Lachain.Core.CLI;
using Lachain.Core.Config;
using Lachain.Core.DI;
using Lachain.Core.DI.Modules;
using Lachain.Core.DI.SimpleInjector;
using Lachain.Crypto;
using Lachain.Crypto.ECDSA;
using Lachain.Storage.State;
using Lachain.Utility.Utils;
using Lachain.UtilityTest;
using NUnit.Framework;

using Lachain.Utility;
using Lachain.Networking;
using Transaction = Lachain.Proto.Transaction;

namespace Lachain.CoreTest.IntegrationTests
{
    public class TransactionsTest
    {
        private IStateManager? _stateManager;
        private IContainer? _container;

        [SetUp]
        public void Setup()
        {
            TestUtils.DeleteTestChainData();

            var containerBuilder = new SimpleInjectorContainerBuilder(new ConfigManager(
                Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json"),
                new RunOptions()
            ));
            containerBuilder.RegisterModule<BlockchainModule>();
            containerBuilder.RegisterModule<ConfigModule>();
            containerBuilder.RegisterModule<StorageModule>();

            _container = containerBuilder.Build();
            _stateManager = _container.Resolve<IStateManager>();

            TestUtils.DeleteTestChainData();
        }

        [TearDown]
        public void Teardown()
        {
            TestUtils.DeleteTestChainData();
        }

        [Test]
        public void Test_Tx_Building()
        {
            var signer = new TransactionSigner();
            var keyPair = new EcdsaKeyPair("0xD95D6DB65F3E2223703C5D8E205D98E3E6B470F067B0F94F6C6BF73D4301CE48"
                .HexToBytes().ToPrivateKey());

            var tx = new Transaction
            {
                To = "0xB8CD3195faf7da8a87A2816B9b4bBA2A19D25dAb".HexToUInt160(),
                From = keyPair.PublicKey.GetAddress(),
                GasPrice = (ulong)Money.Parse("0.0000001").ToWei(),
                GasLimit = 100000000,
                Nonce = 0,
                Value = Money.Parse("20.0").ToUInt256()
            };

            // this is correct RLP of unsigned ethereum tx, check at https://toolkit.abdk.consulting/ethereum#transaction
            var expectedRawHash =
                "0xef8085174876e8008405f5e10094b8cd3195faf7da8a87a2816b9b4bba2a19d25dab8901158e460913d0000080298080"
                    .HexToBytes()
                    .Keccak();
            Assert.AreEqual(expectedRawHash, tx.RawHash());

            // this is correct RLP of signed ethereum tx, check at https://toolkit.abdk.consulting/ethereum#transaction
            // signature is deterministic in compliance with https://tools.ietf.org/html/rfc6979
            var expectedFullHash =
                "0xf86f8085174876e8008405f5e10094b8cd3195faf7da8a87a2816b9b4bba2a19d25dab8901158e460913d000008076a0a62d5dc477e8ed4ed7077c129bac8b68c3e260c99329513f28e3f97b5d9f532da04333f86ce60ed12ea85aa7c9e5f3713b5b81dfbd7f492afc667e0dd5dd0a5939"
                    .HexToBytes()
                    .Keccak();
            var receipt = signer.Sign(tx, keyPair);
            Assert.AreEqual(
                expectedFullHash,
                receipt.Hash
            );
        }

        [Test]
        public void Test_Tx_Pool_Adding()
        {

            var txPool = _container.Resolve<ITransactionPool>();

            // set chainId from config
            if (TransactionUtils.ChainId == 0)
            {
                var configManager = _container.Resolve<IConfigManager>();
                var chainId = configManager.GetConfig<NetworkConfig>("network")?.ChainId;
                TransactionUtils.SetChainId((int)chainId!);
            }

            var tx = TestUtils.GetRandomTransaction();
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(tx.Transaction.From, Money.Parse("1000"));

            var result = txPool.Add(tx);
            Assert.AreEqual(OperatingError.Ok, result);

            result = txPool.Add(tx);
            Assert.AreEqual(OperatingError.AlreadyExists, result);

            var tx2 = TestUtils.GetRandomTransaction();
            _stateManager.LastApprovedSnapshot.Balances.AddBalance(tx2.Transaction.From, Money.Parse("1000"));

            tx2.Transaction.Nonce++;
            result = txPool.Add(tx2);
            Assert.AreEqual(OperatingError.InvalidNonce, result);

            /* TODO: maybe we should fix this strange behaviour */
            var tx3 = TestUtils.GetRandomTransaction();
            tx3.Transaction.From = UInt160Utils.Zero;
            result = txPool.Add(tx3);
            Assert.AreEqual(OperatingError.Ok, result);
        }
    }
}