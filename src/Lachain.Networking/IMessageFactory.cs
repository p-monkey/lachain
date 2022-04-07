﻿using System.Collections.Generic;
using Lachain.Proto;

namespace Lachain.Networking
{
    public interface IMessageFactory
    {
        ECDSAPublicKey GetPublicKey();

        NetworkMessage Ack(ulong messageId);
        NetworkMessage PingReply(ulong timestamp, ulong blockHeight);
        NetworkMessage ConsensusMessage(ConsensusMessage message);
        NetworkMessage GetPeersRequest();
        NetworkMessage GetPeersReply(Peer[] peers);
        NetworkMessage SyncPoolRequest(IEnumerable<UInt256> hashes);
        NetworkMessage SyncPoolReply(IEnumerable<TransactionReceipt> transactions);
        NetworkMessage SyncBlocksRequest(ulong fromHeight, ulong toHeight);
        NetworkMessage RootHashByTrieNameRequest(ulong block, string trieName);
        NetworkMessage BlockBatchRequest(List<ulong> blockNumbers);
        NetworkMessage TrieNodeByHashRequest(List<UInt256> nodeHashes);

        MessageBatch MessagesBatch(IEnumerable<NetworkMessage> messages);

        byte[] SignCommunicationHubInit(byte[] hubKey);
    }
}