using System;
using System.Collections.Generic;
using Lachain.Proto;

namespace Lachain.Consensus.RequestProtocols.Messages.Resends
{
    public class ReadyResend : MessageResendHandler
    {
        public ReadyResend(RequestType type, int validatorCount, int msgPerValidator)
            : base(type, validatorCount, msgPerValidator)
        {

        }

        protected override void HandleSentMessage(int validator, ConsensusMessage msg)
        {
            if (msg.PayloadCase != ConsensusMessage.PayloadOneofCase.ReadyMessage)
                throw new Exception($"{msg.PayloadCase} message routed to Ready resend");
            SaveMessage(validator, 0, msg);
        }

        protected override List<ConsensusMessage?> HandleRequestMessage(int from, RequestConsensusMessage msg)
        {
            if (msg.PayloadCase != RequestConsensusMessage.PayloadOneofCase.RequestReady)
                throw new Exception($"{msg.PayloadCase} routed to Ready Resend");
            var msgs = new List<ConsensusMessage?>();
            var msgIds = new List<int>();
            msgIds.Add(0);
            foreach (var id in msgIds)
            {
                msgs.Add(GetMessage(from, id));
            }
            return msgs;
        }
    }
}