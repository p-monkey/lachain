using System;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC;
using Phorkus.Proto;

namespace Phorkus.CrossChain.Ethereum
{
    public class EthereumTransactionService : ITransactionService
    {
        private readonly EthApiService _ethApiService;

        internal EthereumTransactionService()
        {
            _ethApiService = new EthApiService(new RpcClient(new Uri(EthereumConfig.RpcUri)));
        }

        public AddressFormat AddressFormat { get; } = AddressFormat.Ripmd160;

        public ulong BlockGenerationTime { get; } = 15 * 1000;

        public ulong CurrentBlockHeight
        {
            get
            {
                var getBlockHeight = _ethApiService.Blocks.GetBlockNumber.SendRequestAsync();
                getBlockHeight.Wait();

                if (getBlockHeight.IsFaulted)
                    throw new BlockchainNotAvailableException($"Unable to determine current block height");

                return (ulong) getBlockHeight.Result.Value;
            }
        }

        public BigInteger GetNonce(string address)
        {
            var getNonce = _ethApiService.Transactions.GetTransactionCount.SendRequestAsync(address);
            getNonce.Wait();
            if (getNonce.IsFaulted)
                throw new BlockchainNotAvailableException(
                    $"Unable to calculate nonce for address ({address}) from Ethereum network");
            return getNonce.Result.Value;
        }

        public BigInteger GetGasPrice()
        {
            var getGasPrice = _ethApiService.GasPrice.SendRequestAsync();
            getGasPrice.Wait();
            if (getGasPrice.IsFaulted)
                throw new BlockchainNotAvailableException("Unable to fetch current gas price from Ethereum network");
            return getGasPrice.Result.Value;
        }

        public BigInteger GetBalance(string address)
        {
            var getBalance = _ethApiService.GetBalance.SendRequestAsync(address);
            getBalance.Wait();
            if (getBalance.IsFaulted)
                throw new BlockchainNotAvailableException(
                    $"Unable to fetch balance for address {address} from Ethereum network");
            return getBalance.Result.Value;
        }

        public IEnumerable<IContractTransaction> GetTransactionsAtBlock(byte[] recipient, ulong blockHeight)
        {
            var getTransactions =
                _ethApiService.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(blockHeight));
            getTransactions.Wait();
            if (getTransactions.IsFaulted)
                throw new BlockchainNotAvailableException($"Unable to get tranasction at block ({blockHeight})");

            var address = Utils.ConvertByteArrayToString(recipient);
            var transactions = new List<EthereumContractTransaction>();
            foreach (var tx in getTransactions.Result.Transactions)
            {
                if (tx.To != address)
                    continue;
                var stringTx = tx.ToString();
                var from = stringTx.Substring(
                    tx.From.Length + tx.To.Length + tx.Nonce.HexValue.Length + tx.Gas.HexValue.Length +
                    tx.GasPrice.HexValue.Length + tx.Value.HexValue.Length,
                    stringTx.Length - EthereumConfig.SignatureLength).Substring(0, EthereumConfig.AddressLength);
                var ethereumTx = new EthereumContractTransaction(Utils.ConvertHexStringToByteArray(from),
                    tx.Value.Value, Utils.ConvertHexStringToByteArray(tx.TransactionHash.ToString()),
                    (ulong) Utils.ConvertHexToLong(getTransactions.Result.Timestamp.ToString()));
                transactions.Add(ethereumTx);
            }

            return transactions;
        }

        public byte[] BroadcastTransaction(ITransactionData transactionData)
        {
            var sendTransaction =
                _ethApiService.Transactions.SendRawTransaction.SendRequestAsync(
                    Utils.ConvertByteArrayToString(transactionData.RawTransaction));
            sendTransaction.Wait();
            if (sendTransaction.IsFaulted)
                throw new BlockchainNotAvailableException("Unable to broadcast transaction to Ethereum network");
            var txHash = sendTransaction.Result;
            return Utils.ConvertHexStringToByteArray(txHash);
        }
    }
}