﻿using System;
using System.Collections;
using System.Collections.Generic;
using Phorkus.Networking;
using Phorkus.Proto;

namespace Phorkus.Core.Network
{
    public interface IBlockSynchronizer
    {
        uint WaitForTransactions(IEnumerable<UInt256> transactionHashes, TimeSpan timeout);
        
        uint HandleTransactionsFromPeer(IEnumerable<TransactionReceipt> transactions, IRemotePeer remotePeer);

        uint WaitForBlocks(IEnumerable<UInt256> blockHashes, TimeSpan timeout);
        
        void HandleBlockFromPeer(Block block, IRemotePeer remotePeer, TimeSpan timeout);

        void HandlePeerHasBlocks(ulong blockHeight, IRemotePeer remotePeer);

        bool IsSynchronizingWith(IEnumerable<ECDSAPublicKey> peers);

        void SynchronizeWith(IEnumerable<ECDSAPublicKey> peers);
        
        void Start();
    }
}