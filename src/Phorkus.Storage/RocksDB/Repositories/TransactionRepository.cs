﻿using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using Phorkus.Proto;

namespace Phorkus.Storage.RocksDB.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly IRocksDbContext _rocksDbContext;
        
        public TransactionRepository(IRocksDbContext rocksDbContext)
        {
            _rocksDbContext = rocksDbContext ?? throw new ArgumentNullException(nameof(rocksDbContext));
        }
        
        public SignedTransaction GetTransactionByHash(UInt256 txHash)
        {
            var prefix = EntryPrefix.TransactionByHash.BuildPrefix(txHash);
            var raw = _rocksDbContext.Get(prefix);
            return raw == null ? null : SignedTransaction.Parser.ParseFrom(raw);
        }
        
        public void AddTransaction(SignedTransaction transaction)
        {
            /* write transaction to storage */
            var prefixTx = EntryPrefix.TransactionByHash.BuildPrefix(transaction.Hash);
            _rocksDbContext.Save(prefixTx, transaction.ToByteArray());
        }
        
        public TransactionState ChangeTransactionState(UInt256 txHash, TransactionState txState)
        {
            if (txHash is null)
                throw new ArgumentNullException(nameof(txHash));
            var prefix = EntryPrefix.TransactionStateByHash.BuildPrefix(txHash);
            var rawPrevState = _rocksDbContext.Get(prefix);
            _rocksDbContext.Save(prefix, txState.ToByteArray());
            return rawPrevState != null ? TransactionState.Parser.ParseFrom(rawPrevState) : null;
        }

        public TransactionState GetTransactionState(UInt256 txHash)
        {
            if (txHash is null)
                throw new ArgumentNullException(nameof(txHash));
            var prefix = EntryPrefix.TransactionStateByHash.BuildPrefix(txHash);
            var rawPrevState = _rocksDbContext.Get(prefix);
            return rawPrevState != null ? TransactionState.Parser.ParseFrom(rawPrevState) : null;
        }

        public IEnumerable<SignedTransaction> GetTransactionsByHashes(IEnumerable<UInt256> txHashes)
        {
            return txHashes.Select(GetTransactionByHash).Where(tx => tx != null);
        }

        public bool ContainsTransactionByHash(UInt256 txHash)
        {
            var prefix = EntryPrefix.TransactionByHash.BuildPrefix(txHash);
            var raw = _rocksDbContext.Get(prefix);
            return raw != null;
        }
        
        public SignedTransaction GetLatestTransactionByFrom(UInt160 from)
        {
            var prefix = EntryPrefix.TransactionLatestByFrom.BuildPrefix(from);
            var rawHash = _rocksDbContext.Get(prefix);
            if (rawHash is null)
                return null;
            var hash = UInt256.Parser.ParseFrom(rawHash);
            return GetTransactionByHash(hash);
        }
        
        public void CommitTransaction(SignedTransaction signedTransaction)
        {
            /* write latest transaction hash by from */
            var prefixHash = EntryPrefix.TransactionLatestByFrom.BuildPrefix(signedTransaction.Transaction.From);
            _rocksDbContext.Save(prefixHash, signedTransaction.Hash.ToByteArray());
        }

        public uint GetTotalTransactionCount(UInt160 from)
        {
            var latestTx = GetLatestTransactionByFrom(from);
            return (uint) (latestTx?.Transaction.Nonce + 1 ?? 0);
        }
    }
}