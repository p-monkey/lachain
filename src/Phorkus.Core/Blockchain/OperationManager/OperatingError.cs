﻿namespace Phorkus.Core.Blockchain.OperationManager
{
    public enum OperatingError : byte
    {
        Ok,
        HashMismatched,
        InvalidNonce,
        UnsupportedTransaction,
        InvalidSignature,
        InvalidTransaction,
        AlreadyExists,
        PrevBlockHashMismatched,
        InvalidBlock,
        QuorumNotReached,
        InvalidState,
        TransactionLost,
        InvalidMultisig,
        BlockAlreadyExists,
        InvalidMerkeRoot,
        ContractFailed,
        ContractNotFound,
        InvalidContract,
        OutOfGas,
        InvalidGasLimit,
        InsufficientBalance,
        InvalidInput,
    }
}