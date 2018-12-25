﻿using System;
using Phorkus.Core.Blockchain.Genesis;
using Phorkus.Proto;
using Phorkus.Core.Utils;
using Phorkus.Crypto;
using Phorkus.Logger;
using Phorkus.Storage.RocksDB.Repositories;
using Phorkus.Storage.State;
using Phorkus.Utility.Utils;

namespace Phorkus.Core.Blockchain.OperationManager.BlockManager
{
    public class BlockManager : IBlockManager
    {
        private readonly IGlobalRepository _globalRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionManager _transactionManager;
        private readonly ICrypto _crypto;
        private readonly IValidatorManager _validatorManager;
        private readonly IGenesisBuilder _genesisBuilder;
        private readonly IMultisigVerifier _multisigVerifier;
        private readonly ILogger<IBlockManager> _logger;
        private readonly IBlockchainStateManager _blockchainStateManager;

        public BlockManager(
            IGlobalRepository globalRepository,
            IBlockRepository blockRepository,
            ITransactionManager transactionManager,
            ICrypto crypto,
            IValidatorManager validatorManager,
            IGenesisBuilder genesisBuilder,
            IMultisigVerifier multisigVerifier,
            ILogger<IBlockManager> logger,
            IBlockchainStateManager blockchainStateManager)
        {
            _globalRepository = globalRepository;
            _blockRepository = blockRepository;
            _transactionManager = transactionManager;
            _crypto = crypto;
            _validatorManager = validatorManager;
            _genesisBuilder = genesisBuilder;
            _multisigVerifier = multisigVerifier;
            _logger = logger;
            _blockchainStateManager = blockchainStateManager;
        }

        public event EventHandler<Block> OnBlockPersisted;
        public event EventHandler<Block> OnBlockSigned;

        public Block GetByHeight(ulong blockHeight)
        {
            return _blockRepository.GetBlockByHeight(blockHeight);
        }
        
        public Block GetByHash(UInt256 blockHash)
        {
            return _blockRepository.GetBlockByHash(blockHash);
        }

        private bool _IsGenesisBlock(Block block)
        {
            return block.Hash.Equals(_genesisBuilder.Build().Block.Hash);
        }

        public OperatingError Persist(Block block)
        {
            /* verify next block */
            var error = Verify(block);
            if (error != OperatingError.Ok)
                return error;
            /* check next block index */
            var currentBlockHeader = _globalRepository.GetTotalBlockHeight();
            if (!_IsGenesisBlock(block) && currentBlockHeader + 1 != block.Header.Index)
                return OperatingError.InvalidNonce;
            var exists = _blockRepository.GetBlockByHeight(block.Header.Index);
            if (exists != null)
                return OperatingError.BlockAlreadyExists;
            /* check prev block hash */
            var latestBlock = _blockRepository.GetBlockByHeight(currentBlockHeader);
            if (latestBlock != null && !block.Header.PrevBlockHash.Equals(latestBlock.Hash))
                return OperatingError.PrevBlockHashMismatched;
            /* verify block signatures */
            error = VerifySignatures(block);
            if (error != OperatingError.Ok)
                return error;
            /* confirm block transactions */
            foreach (var txHash in block.TransactionHashes)
            {
                if (_transactionManager.GetByHash(txHash) is null)
                    return OperatingError.TransactionLost;
            }

            foreach (var txHash in block.TransactionHashes)
            {
                var snapshot = _blockchainStateManager.NewSnapshot();
                var result = _transactionManager.Execute(block, txHash, snapshot);
                if (result == OperatingError.Ok)
                {
                    _blockchainStateManager.Approve();
                    continue;
                }                    
                /* TODO: "we need block synchronization on transaction lost for example" */
                _logger.LogWarning($"Unable to execute transaction {txHash.Buffer.ToHex()}, {result}");
                /* TODO: "mark transaction as failed to execute here or something else" */
                _blockchainStateManager.Rollback();
            }

            /* write block to database */
            _blockRepository.AddBlock(block);
            _blockchainStateManager.CommitApproved();
            _logger.LogInformation($"Persisted new block {block.Header.Index} with hash {block.Hash}");
            var currentHeaderHeight = _globalRepository.GetTotalBlockHeaderHeight();
            if (block.Header.Index > currentHeaderHeight)
                _globalRepository.SetTotalBlockHeaderHeight(block.Header.Index);
            _globalRepository.SetTotalBlockHeight(block.Header.Index);
            /*logger.LogInformation($"Changed current block height to {block.Header.Index}");*/
            return OperatingError.Ok;
        }

        public Signature Sign(BlockHeader block, KeyPair keyPair)
        {
            return _crypto.Sign(block.ToHash256().Buffer.ToByteArray(), keyPair.PrivateKey.Buffer.ToByteArray())
                .ToSignature();
        }

        public OperatingError VerifySignature(BlockHeader blockHeader, Signature signature, PublicKey publicKey)
        {
            var result = _crypto.VerifySignature(blockHeader.ToHash256().Buffer.ToByteArray(),
                signature.Buffer.ToByteArray(), publicKey.Buffer.ToByteArray());
            return result ? OperatingError.Ok : OperatingError.InvalidSignature;
        }

        public OperatingError VerifySignatures(Block block)
        {
            if (!block.Header.ToHash256().Equals(block.Hash))
                return OperatingError.HashMismatched;
            if (_IsGenesisBlock(block))
                return OperatingError.Ok;
            return _multisigVerifier.VerifyMultisig(block.Multisig, block.Hash);
        }

        public OperatingError Verify(Block block)
        {
            var header = block.Header;
            if (header.Version != 0)
                return OperatingError.InvalidBlock;
            if (block.Header.Index != 0 && header.PrevBlockHash.IsZero())
                return OperatingError.InvalidBlock;
            if (header.MerkleRoot is null || header.MerkleRoot.IsZero())
                return OperatingError.InvalidBlock;
            if (!MerkleTree.ComputeRoot(block.TransactionHashes).Equals(header.MerkleRoot))
                return OperatingError.InvalidMerkeRoot;
            if (header.Timestamp == 0)
                return OperatingError.InvalidBlock;
            return OperatingError.Ok;
        }
    }
}