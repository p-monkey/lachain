﻿using Phorkus.Core.Blockchain.State;
using Phorkus.Proto;
using Phorkus.Utility;
using Phorkus.Utility.Utils;

namespace Phorkus.Core.Blockchain.OperationManager.TransactionManager
{
    public class DepositTransactionExecuter : ITransactionExecuter
    {
        private readonly IValidatorManager _validatorManager;

        public DepositTransactionExecuter(IValidatorManager validatorManager)
        {
            _validatorManager = validatorManager;
        }

        public OperatingError Execute(Block block, Transaction transaction, IBlockchainSnapshot snapshot)
        {
            var balances = snapshot.Balances;
            var error = Verify(transaction);
            if (error != OperatingError.Ok)
                return error;
            var deposit = transaction.Deposit;
            if (!deposit.Value.IsZero())
            {
                var assetName = deposit.BlockchainType == BlockchainType.Bitcoin
                    ? snapshot.Assets.GetAssetByName("BTC").Hash
                    : snapshot.Assets.GetAssetByName("ETH").Hash;
                snapshot.Assets.AddSupply(assetName, new Money(deposit.Value));
                balances.TransferBalance(transaction.From, deposit.Recipient, assetName, new Money(deposit.Value));
            }

            /* TODO: "invoke smart-contract code here" */
            return OperatingError.Ok;
        }

        public OperatingError Verify(Transaction transaction)
        {
            if (transaction.Type != TransactionType.Deposit)
                return OperatingError.InvalidTransaction;
            var deposit = transaction.Deposit;
            if (deposit?.BlockchainType is null)
                return OperatingError.InvalidTransaction;
            if (deposit?.TransactionHash is null)
                return OperatingError.InvalidTransaction;
            if (deposit?.Timestamp is null)
                return OperatingError.InvalidTransaction;
            if (deposit?.AddressFormat is null)
                return OperatingError.InvalidTransaction;
            if (deposit.Recipient is null)
                return OperatingError.InvalidTransaction;
            if (deposit.Value is null)
                return OperatingError.InvalidTransaction;
            if (!_validatorManager.CheckValidator(transaction.From))
            {
                return OperatingError.InvalidTransaction;
            }

            throw new OperationNotSupportedException();
        }
    }
}