﻿using Phorkus.Core.Blockchain;
using Phorkus.Core.Network;
using Phorkus.Proto;

namespace Phorkus.Core.Messaging.Handlers
{
    public class HandshakeResponseMessageHandler : IMessageHandler
    {
        private readonly IMessageFactory _messageFactory;
        private readonly IBlockchainContext _blockchainContext;

        public HandshakeResponseMessageHandler(IMessageFactory messageFactory,
            IBlockchainContext blockchainContext)
        {
            _messageFactory = messageFactory;
            _blockchainContext = blockchainContext;
        }

        public void HandleMessage(IPeer peer, Message message)
        {
            var handshakeRequest = message.HandshakeResponse;
            if (handshakeRequest is null)
                throw new InvalidMessageException();
            /* request headers if we don't have */
            var myHeight = _blockchainContext.CurrentBlockHeaderHeight;
            if (myHeight < handshakeRequest.Node.BlockHeight)
                peer.Send(_messageFactory.GetBlocksMessage(myHeight));
        }
    }
}