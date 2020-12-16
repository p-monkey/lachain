using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Google.Protobuf;
using Lachain.Consensus;
using Lachain.Consensus.ThresholdKeygen.Data;
using Lachain.Core.Blockchain.Error;
using Lachain.Core.Blockchain.Interface;
using Lachain.Core.Blockchain.Operations;
using Lachain.Core.Blockchain.Pool;
using Lachain.Core.Blockchain.SystemContracts;
using Lachain.Core.Blockchain.SystemContracts.ContractManager;
using Lachain.Core.Blockchain.SystemContracts.Interface;
using Lachain.Core.Blockchain.VM;
using Lachain.Core.ValidatorStatus;
using Lachain.Core.DI;
using Lachain.Core.Vault;
using Lachain.Crypto;
using Lachain.Crypto.Misc;
using Lachain.Networking;
using Lachain.Proto;
using Lachain.Storage.State;
using Lachain.Utility;
using Lachain.Utility.Utils;
using NUnit.Framework;
using Secp256k1Net;

namespace Lachain.CoreTest
{
    [TestFixture]
    public class GovernanceEventsTest
    {
        private static readonly ICrypto Crypto = CryptoProvider.GetCrypto();
        private static readonly ITransactionSigner Signer = new TransactionSigner();

        private IBlockManager _blockManager = null!;
        private ITransactionBuilder _transactionBuilder = null!;
        private ITransactionPool _transactionPool = null!;
        private IStateManager _stateManager = null!;
        private IPrivateWallet _wallet = null!;
        private IContainer? _container;
        private Dictionary<UInt256, ByteString> _eventData = null!;
        
        [SetUp]
        public void Setup()
        {
            _eventData = new Dictionary<UInt256, ByteString>();
            TestUtils.DeleteTestChainData();
            Directory.CreateDirectory("./ChainLachain"); // TODO: this is some dirty hack, hub creates file not in correct place
            var containerBuilder = TestUtils.GetContainerBuilder(
                Path.Join(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "config.json")
            ); 
            _container = containerBuilder.Build();
            _blockManager = _container.Resolve<IBlockManager>();
            _transactionBuilder = _container.Resolve<ITransactionBuilder>();
            _stateManager = _container.Resolve<IStateManager>();
            _wallet = _container.Resolve<IPrivateWallet>();
            _transactionPool = _container.Resolve<ITransactionPool>();
        }

        [TearDown]
        public void Teardown()
        {
            TestUtils.DeleteTestChainData();
            _container?.Dispose();
        }

        [Test]
        public void Test_EventFormat()
        {
            _blockManager.TryBuildGenesisBlock();

            for (int i = 0; i < 52; i++)
            {
                GenerateBlocks(1);

                var lastblock = _stateManager.CurrentSnapshot.Blocks.GetBlockByHeight(_stateManager.CurrentSnapshot.Blocks.GetTotalBlockHeight());
                foreach (UInt256 tx in lastblock?.TransactionHashes)
                {
                    var event_count = _stateManager.CurrentSnapshot.Events.GetTotalTransactionEvents(tx);
                    Console.WriteLine($"{event_count} events");
                    for(uint j = 0; j < event_count; j++)
                    {
                        var event_obj = _stateManager.CurrentSnapshot.Events.GetEventByTransactionHashAndIndex(tx, j);
                        Assert.AreEqual(event_obj.TransactionHash, tx);
                        if (_eventData.TryGetValue(event_obj.TransactionHash, out ByteString storedData))
                        {
                            Assert.IsTrue(event_obj.Data.Equals(storedData));
                        }
                    }
                }
            }
        }

        private void GenerateBlocks(int blockNum)
        {
            for (int i = 0; i < blockNum; i++)
            {
                var txes = GetCurrentPoolTxes();
                var height = _stateManager.LastApprovedSnapshot.Blocks.GetTotalBlockHeight();
                if (height % 50 == 49) // next block is last in cycle
                {
                    var new_txes = new TransactionReceipt[txes.Length + 1];
                    txes.CopyTo(new_txes, 0);
                    new_txes[txes.Length] = MakeDistributeCycleRewardsAndPenaltiesTxReceipt();
                    txes = new_txes;
                    Console.WriteLine($"Tx with a contract call, {txes.Length} txes");
                }
                if (height % 50 == 0) 
                {
                    var new_txes = new TransactionReceipt[txes.Length + 1];
                    txes.CopyTo(new_txes, 0);
                    new_txes[txes.Length] = MakeCommitTransaction();
                    //new_txes[txes.Length + 1] = MakeKeygenSendValuesTxReceipt();
                    txes = new_txes;
                    Console.WriteLine($"Tx with a contract call, {txes.Length} txes");
                }
                if (height % 50 == 1)
                {
                    var new_txes = new TransactionReceipt[txes.Length + 1];
                    txes.CopyTo(new_txes, 0);
                    new_txes[txes.Length] = MakeNextValidatorsTxReceipt();
                    txes = new_txes;
                    Console.WriteLine($"Tx with a contract call, {txes.Length} txes");
                }
                

                var block = BuildNextBlock(txes);
                var result = ExecuteBlock(block, txes);
                Assert.AreEqual(OperatingError.Ok, result);
            }
        }

        private TransactionReceipt[] GetCurrentPoolTxes()
        {
            return _transactionPool.Peek(1000, 1000).ToArray();
        }

        private Block BuildNextBlock(TransactionReceipt[] receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };

            var merkleRoot = UInt256Utils.Zero;

            if (receipts.Any())
                merkleRoot = MerkleTree.ComputeRoot(receipts.Select(tx => tx.Hash).ToArray()) ??
                             throw new InvalidOperationException();

            var height = _stateManager.LastApprovedSnapshot.Blocks.GetTotalBlockHeight();
            var predecessor =
                _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(height);
                
            var (header, multisig) =
                BuildHeaderAndMultisig(merkleRoot, predecessor, _stateManager.LastApprovedSnapshot.StateHash);

            return new Block
            {
                Header = header,
                Hash = header.Keccak(),
                Multisig = multisig,
                TransactionHashes = { receipts.Select(tx => tx.Hash)},
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
                keyPair.PrivateKey.Encode()
            ).ToSignature();

            var multisig = new MultiSig
            {
                Quorum = 1,
                Validators = {_wallet.EcdsaKeyPair.PublicKey},
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

        private OperatingError ExecuteBlock(Block block, TransactionReceipt[] receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };

            var (_, _, stateHash, _) = _blockManager.Emulate(block, receipts);

            var height = _stateManager.LastApprovedSnapshot.Blocks.GetTotalBlockHeight();
            var predecessor =
                _stateManager.LastApprovedSnapshot.Blocks.GetBlockByHeight(height);
            var (header, multisig) = BuildHeaderAndMultisig(block.Header.MerkleRoot, predecessor, stateHash);

            block.Header = header;
            block.Multisig = multisig;
            block.Hash = header.Keccak();

            var status = _blockManager.Execute(block, receipts, true, true);
            return status;
        }

        private OperatingError EmulateBlock(Block block, TransactionReceipt[] receipts = null)
        {
            receipts ??= new TransactionReceipt[] { };
            var (status, _, _, _) = _blockManager.Emulate(block, receipts);
            return status;
        }

        private TransactionReceipt MakeDistributeCycleRewardsAndPenaltiesTxReceipt()
        {
            var res = BuildSystemContractTxReceipt(ContractRegisterer.GovernanceContract,
                GovernanceInterface.MethodDistributeCycleRewardsAndPenalties);
            _eventData.Add(res.Hash, 
                ByteString.CopyFrom(ContractEncoder.Encode(
                    GovernanceInterface.EventDistributeCycleRewardsAndPenalties, 
                    Money.Parse("500.0").ToUInt256())));
            return res;
        }

        private TransactionReceipt MakeNextValidatorsTxReceipt()
        {
            var sk = Crypto.GeneratePrivateKey();
            var pk = Crypto.ComputePublicKey(sk, false);
            var tx = _transactionBuilder.InvokeTransactionWithGasPrice(
                _wallet.EcdsaKeyPair.PublicKey.GetAddress(),
                ContractRegisterer.GovernanceContract,
                Money.Zero,
                GovernanceInterface.MethodChangeValidators,
                0,
                (pk)
            );
            var res = Signer.Sign(tx, _wallet.EcdsaKeyPair);
            _eventData.Add(res.Hash, 
                ByteString.CopyFrom(ContractEncoder.Encode(GovernanceInterface.EventChangeValidators, (pk))));
            return res;
        }
        
        private TransactionReceipt MakeKeygenSendValuesTxReceipt()
        {
            var proposer = new BigInteger(0).ToUInt256();
            var value = new Byte[0];
            var tx = _transactionBuilder.InvokeTransactionWithGasPrice(
                _wallet.EcdsaKeyPair.PublicKey.GetAddress(),
                ContractRegisterer.GovernanceContract,
                Money.Zero,
                GovernanceInterface.MethodKeygenSendValue,
                0,
                proposer, (value)
            );
            var res =  Signer.Sign(tx, _wallet.EcdsaKeyPair);
            _eventData.Add(res.Hash, 
                ByteString.CopyFrom(ContractEncoder.Encode(GovernanceInterface.EventKeygenSendValue, 
                    proposer, (value))));
            return res;
        }
        
        private TransactionReceipt MakeFinishCircleTxReceipt()
        {
            var res = BuildSystemContractTxReceipt(ContractRegisterer.GovernanceContract,
                GovernanceInterface.MethodFinishCycle);
            _eventData.Add(res.Hash, 
                ByteString.CopyFrom(ContractEncoder.Encode(GovernanceInterface.EventFinishCycle)));
            return res;
        }

        private TransactionReceipt MakeCommitTransaction()
        {
            var biVarPoly = BiVarSymmetricPolynomial.Random(0);
            var commitment = biVarPoly.Commit().ToBytes();
            var row = new Byte[0];
            var tx = _transactionBuilder.InvokeTransactionWithGasPrice(
                _wallet.EcdsaKeyPair.PublicKey.GetAddress(),
                ContractRegisterer.GovernanceContract,
                Money.Zero,
                GovernanceInterface.MethodKeygenCommit,
                0,
                commitment, 
                new byte[][]{row}
        );

        var res = Signer.Sign(tx, _wallet.EcdsaKeyPair);
            _eventData.Add(res.Hash, 
                ByteString.CopyFrom(ContractEncoder.Encode(GovernanceInterface.EventKeygenCommit, 
                    commitment, new byte[][]{row})));
            return res;
        }

        private TransactionReceipt BuildSystemContractTxReceipt(UInt160 contractAddress, string mehodSignature)
        {
            var transaction =  _transactionBuilder.InvokeTransactionWithGasPrice(
                UInt160Utils.Zero, 
                contractAddress,
                Money.Zero, 
                mehodSignature, 
                0
                );
            return new TransactionReceipt
            {
                Hash = transaction.FullHash(SignatureUtils.Zero),
                Status = TransactionStatus.Pool,
                Transaction = transaction,
                Signature = SignatureUtils.Zero,
            };
        }
    }
}