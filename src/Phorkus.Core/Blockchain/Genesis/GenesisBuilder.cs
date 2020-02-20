﻿using System;
using System.Linq;
using Google.Protobuf.WellKnownTypes;
using Phorkus.Core.Blockchain.OperationManager;
using Phorkus.Core.Config;
using Phorkus.Proto;
using Phorkus.Core.Utils;
using Phorkus.Crypto;
using Phorkus.Utility.Utils;

namespace Phorkus.Core.Blockchain.Genesis
{
    public class GenesisBuilder : IGenesisBuilder
    {
        public const ulong GenesisConsensusData = 2083236893UL;

        private readonly IConfigManager _configManager;
        private readonly ICrypto _crypto = CryptoProvider.GetCrypto();
        private readonly ITransactionManager _transactionManager;

        public GenesisBuilder(
            IConfigManager configManager,
            ITransactionManager transactionManager)
        {
            _transactionManager = transactionManager;
            _configManager = configManager;
        }

        private BlockWithTransactions? _genesisBlock;

        public BlockWithTransactions Build()
        {
            if (_genesisBlock != null)
                return _genesisBlock;

            var genesisConfig = _configManager.GetConfig<GenesisConfig>("genesis");
            if (genesisConfig?.PrivateKey is null)
            {
                throw new ArgumentNullException(nameof(genesisConfig.PrivateKey),
                    "You must specify private key in genesis config section"
                );
            }

            var keyPair = new ECDSAKeyPair(genesisConfig.PrivateKey.HexToBytes().ToPrivateKey(), _crypto);
            var address = _crypto.ComputeAddress(keyPair.PublicKey.Buffer.ToByteArray()).ToUInt160();

            var txsBefore = new Transaction[] { };
            var genesisTransactions = txsBefore.ToArray();

            var nonce = 0ul;
            foreach (var tx in genesisTransactions)
            {
                tx.From = address;
                tx.Nonce = nonce++;
            }

            var signed = genesisTransactions.Select(tx => _transactionManager.Sign(tx, keyPair));
            var acceptedTransactions = signed as TransactionReceipt[] ?? signed.ToArray();
            var txHashes = acceptedTransactions.Select(tx => tx.Hash).ToArray();

            var header = new BlockHeader
            {
                PrevBlockHash = UInt256Utils.Zero,
                MerkleRoot = MerkleTree.ComputeRoot(txHashes) ?? UInt256Utils.Zero,
                Index = 0,
                StateHash = UInt256Utils.Zero,
                Nonce = GenesisConsensusData
            };

            var result = new Block
            {
                Hash = header.ToHash256(),
                TransactionHashes = {txHashes},
                Header = header
            };

            _genesisBlock = new BlockWithTransactions(result, acceptedTransactions.ToArray());
            return _genesisBlock;
        }
    }
}