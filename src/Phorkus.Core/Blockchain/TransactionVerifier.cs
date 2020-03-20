﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Phorkus.Core.Blockchain.Interface;
using Phorkus.Core.Utils;
using Phorkus.Crypto;
using Phorkus.Logger;
using Phorkus.Proto;
using Phorkus.Utility.Utils;

namespace Phorkus.Core.Blockchain
{
    public class TransactionVerifier : ITransactionVerifier
    {
        private readonly ILogger<TransactionVerifier> _logger = LoggerFactory.GetLoggerForClass<TransactionVerifier>();
        private readonly ICrypto _crypto = CryptoProvider.GetCrypto();

        private readonly IDictionary<UInt160, ECDSAPublicKey> _publicKeyCache
            = new Dictionary<UInt160, ECDSAPublicKey>();

        private readonly Queue<TransactionReceipt> _transactionQueue
            = new Queue<TransactionReceipt>();

        private readonly object _queueNotEmpty = new object();

        public event EventHandler<TransactionReceipt>? OnTransactionVerified;

        public void VerifyTransaction(TransactionReceipt acceptedTransaction, ECDSAPublicKey publicKey)
        {
            var address = _crypto.ComputeAddress(publicKey.Buffer.ToByteArray()).ToUInt160();
            _publicKeyCache.Add(address, publicKey);
            VerifyTransaction(acceptedTransaction);
        }

        public void VerifyTransaction(TransactionReceipt acceptedTransaction)
        {
            if (acceptedTransaction is null)
                throw new ArgumentNullException(nameof(acceptedTransaction));
            lock (_queueNotEmpty)
            {
                _transactionQueue.Enqueue(acceptedTransaction);
                Monitor.PulseAll(_queueNotEmpty);
            }
        }

        public bool VerifyTransactionImmediately(TransactionReceipt transaction, ECDSAPublicKey publicKey)
        {
            try
            {
                /* verify transaction signature */
                var result = _crypto.VerifySignature(transaction.Hash.Buffer.ToByteArray(),
                    transaction.Signature.Buffer.ToByteArray(), publicKey.Buffer.ToByteArray());
                if (!result)
                    return false;
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public bool VerifyTransactionImmediately(TransactionReceipt transaction, bool cacheEnabled = true)
        {
            if (transaction is null)
                throw new ArgumentNullException(nameof(transaction));

            /* validate transaction hash */
            if (!transaction.Hash.Equals(HashUtils.ToHash256(transaction.Transaction)))
                return false;
            
            try
            {
                /* try to verify signature using public key cache to avoid EC recover */
                if (cacheEnabled && _publicKeyCache.TryGetValue(transaction.Transaction.From, out var publicKey))
                    return VerifyTransactionImmediately(transaction, publicKey);
                Console.WriteLine(transaction.Hash.Buffer.ToByteArray().ToHex());
                Console.WriteLine(transaction.Signature.Buffer.ToByteArray().ToHex());
                /* recover EC to get public key from signature to compute address */
                var rawKey = _crypto.RecoverSignatureHashed(transaction.Hash.Buffer.ToByteArray(),
                    transaction.Signature.Buffer.ToByteArray());
                
                Console.WriteLine(rawKey.ToHex());
                var address = _crypto.ComputeAddress(rawKey);

                Console.WriteLine(address.ToHex());
                Console.WriteLine(transaction.Transaction.From.Buffer.ToByteArray().ToHex());
                /* check if recovered address from public key is valid */
                if (rawKey is null || !address.SequenceEqual(transaction.Transaction.From.Buffer.ToByteArray()))
                    return false;

                /* try to remember public key for this address */
                if (cacheEnabled)
                    _publicKeyCache.Add(transaction.Transaction.From, rawKey.ToPublicKey());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private void _Worker()
        {
            while (_working)
            {
                try
                {
                    TransactionReceipt tx;
                    lock (_queueNotEmpty)
                    {
                        while (_transactionQueue.Count == 0)
                            Monitor.Wait(_queueNotEmpty);
                        tx = _transactionQueue.Dequeue();
                    }

                    if (VerifyTransactionImmediately(tx))
                        OnTransactionVerified?.Invoke(this, tx);
                }
                catch (Exception e)
                {
                    _logger.LogError("Transaction verified failed: " + e);
                }
            }
        }

        private bool _working;

        public void Start()
        {
            uint workers = 1;
            if (workers <= 0)
                throw new ArgumentOutOfRangeException(nameof(workers));
            if (_working)
                return;
            _working = true;
            for (var i = 0; i < workers; i++)
                Task.Factory.StartNew(_Worker, TaskCreationOptions.LongRunning);
        }

        public void Stop()
        {
            _working = false;
        }
    }
}