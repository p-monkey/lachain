using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Phorkus.Consensus;
using Phorkus.Consensus.BinaryAgreement;
using Phorkus.Consensus.CommonCoin;
using Phorkus.Consensus.CommonSubset;
using Phorkus.Consensus.HoneyBadger;
using Phorkus.Consensus.Messages;
using Phorkus.Consensus.ReliableBroadcast;
using Phorkus.Consensus.RootProtocol;
using Phorkus.Consensus.TPKE;
using Phorkus.Proto;

namespace Phorkus.ConsensusTest
{
    public class BroadcastSimulator : IConsensusBroadcaster
    {
        private readonly int _sender;

        public Dictionary<IProtocolIdentifier, IConsensusProtocol> Registry { get; } =
            new Dictionary<IProtocolIdentifier, IConsensusProtocol>();

        private readonly Dictionary<IProtocolIdentifier, IProtocolIdentifier> _callback =
            new Dictionary<IProtocolIdentifier, IProtocolIdentifier>();

        private readonly DeliveryService _deliveryService;

        private readonly IPublicConsensusKeySet _wallet;
        private readonly IPrivateConsensusKeySet _privateKeys;

        private readonly ISet<int> _silenced;

        private bool Terminated { set; get; } = false;

        public IConsensusProtocol GetProtocolById(IProtocolIdentifier id)
        {
            return Registry[id];
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Terminate()
        {
            Terminated = true;
        }

        private bool MixMessages { get; }

        public BroadcastSimulator(
            int sender, IPublicConsensusKeySet wallet, IPrivateConsensusKeySet privateKeys,
            DeliveryService deliveryService, bool mixMessages
        )
        {
            _sender = sender;
            _deliveryService = deliveryService;
            _deliveryService.AddPlayer(GetMyId(), this);
            _wallet = wallet;
            _privateKeys = privateKeys;
            _silenced = new HashSet<int>();
            MixMessages = mixMessages;
        }

        public void Silent(int id)
        {
            _silenced.Add(id);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RegisterProtocols(IEnumerable<IConsensusProtocol> protocols)
        {
            foreach (var protocol in protocols)
            {
                if (Registry.ContainsKey(protocol.Id))
                    throw new InvalidOperationException($"Protocol with id ({protocol.Id}) already registered");
                Registry[protocol.Id] = protocol;
            }
        }

        public void Broadcast(ConsensusMessage message)
        {
            message.Validator = new Validator {Era = 0};
            _deliveryService.BroadcastMessage(GetMyId(), message);
        }

        public void SendToValidator(ConsensusMessage message, int index)
        {
            message.Validator = new Validator {Era = 0};
            _deliveryService.SendToPlayer(GetMyId(), index, message);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void CheckRequest(IProtocolIdentifier id)
        {
            if (Registry.ContainsKey(id)) return;
            Console.Error.WriteLine($"{_sender}: creating protocol {id} on demand.");
            if (Terminated)
            {
                Console.Error.WriteLine($"{_sender}: but already terminated.");
                return;
            }

            switch (id)
            {
                case BinaryBroadcastId bbId:
                    RegisterProtocols(new[]
                    {
                        new BinaryBroadcast(bbId, _wallet, this)
                    });
                    break;
                case CoinId coinId:
                    RegisterProtocols(new[]
                    {
                        new CommonCoin(coinId, _wallet, _privateKeys.ThresholdSignaturePrivateKeyShare, this),
                    });
                    break;
                case ReliableBroadcastId rbcId:
                    RegisterProtocols(new[]
                    {
                        //new MockReliableBroadcast(rbcId, _wallet, this),
                        new ReliableBroadcast(rbcId, _wallet, this)
                    });
                    break;
                case BinaryAgreementId baId:
                    RegisterProtocols(new[]
                    {
                        new BinaryAgreement(baId, _wallet, this),
                    });
                    break;
                case CommonSubsetId acsId:
                    RegisterProtocols(new[]
                    {
                        new CommonSubset(acsId, _wallet, this),
                    });
                    break;
                case RootProtocolId rootId:
                    RegisterProtocols(new[]
                    {
                        new RootProtocol(rootId, _wallet, _privateKeys.EcdsaKeyPair.PrivateKey, this),
                    });
                    break;
                default:
                    throw new Exception($"Unknown protocol type {id}");
            }

            Console.Error.WriteLine($"{_sender}: created protocol {id}.");
        }

        public void Dispatch(ConsensusMessage message, int from)
        {
            if (_silenced.Contains(from))
                return;
            if (_silenced.Contains(GetMyId()))
                return;

            switch (message.PayloadCase)
            {
                case ConsensusMessage.PayloadOneofCase.Bval:
                    var idBval = new BinaryBroadcastId(message.Validator.Era, message.Bval.Agreement,
                        message.Bval.Epoch);
                    CheckRequest(idBval);
                    Registry[idBval]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.Aux:
                    var idAux = new BinaryBroadcastId(message.Validator.Era, message.Aux.Agreement, message.Aux.Epoch);
                    CheckRequest(idAux);
                    Registry[idAux]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.Conf:
                    var idConf = new BinaryBroadcastId(message.Validator.Era, message.Conf.Agreement,
                        message.Conf.Epoch);
                    CheckRequest(idConf);
                    Registry[idConf]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.Coin:
                    var idCoin = new CoinId(message.Validator.Era, message.Coin.Agreement, message.Coin.Epoch);
                    CheckRequest(idCoin);
                    Registry[idCoin]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.TpkeKeys:
                    var idTpkeKeys = new TPKESetupId((int) message.Validator.Era);
                    CheckRequest(idTpkeKeys);
                    Registry[idTpkeKeys]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.PolynomialValue:
                    var idPolynomialValue = new TPKESetupId((int) message.Validator.Era);
                    CheckRequest(idPolynomialValue);
                    Registry[idPolynomialValue]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.HiddenPolynomial:
                    var idHiddenPolynomial = new TPKESetupId((int) message.Validator.Era);
                    CheckRequest(idHiddenPolynomial);
                    Registry[idHiddenPolynomial]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.ConfirmationHash:
                    var idConfirmationHash = new TPKESetupId((int) message.Validator.Era);
                    CheckRequest(idConfirmationHash);
                    Registry[idConfirmationHash]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.Decrypted:
                    var hbbftId = new HoneyBadgerId((int) message.Validator.Era);
                    CheckRequest(hbbftId);
                    Registry[hbbftId]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.ValMessage:
                    var reliableBroadcastId = new ReliableBroadcastId(from, (int) message.Validator.Era);
                    CheckRequest(reliableBroadcastId);
                    Registry[reliableBroadcastId]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.EchoMessage:
                    var rbIdEchoMsg = new ReliableBroadcastId(from, (int) message.Validator.Era);
                    CheckRequest(rbIdEchoMsg);
                    Registry[rbIdEchoMsg]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.EncryptedShare:
                    var idEncryptedShare =
                        new ReliableBroadcastId(message.EncryptedShare.Id, (int) message.Validator.Era);
                    CheckRequest(idEncryptedShare);
                    Registry[idEncryptedShare]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                case ConsensusMessage.PayloadOneofCase.SignedHeaderMessage:
                    var idRoot = new RootProtocolId(message.Validator.Era);
                    CheckRequest(idRoot);
                    Registry[idRoot]?.ReceiveMessage(new MessageEnvelope(message, from));
                    break;
                default:
                    throw new InvalidOperationException($"Unknown message type {message.PayloadCase}");
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void InternalRequest<TId, TInputType>(ProtocolRequest<TId, TInputType> request)
            where TId : IProtocolIdentifier
        {
            if (request.From != null)
            {
                if (_callback.ContainsKey(request.To))
                    throw new InvalidOperationException(
                        "Cannot have two requests from different protocols to one protocol");
                _callback[request.To] = request.From;
            }

            Console.Error.WriteLine($"Party {GetMyId()} received internal request from {request.From}");
            CheckRequest(request.To);
            Registry[request.To]?.ReceiveMessage(new MessageEnvelope(request, GetMyId()));
        }

        public void InternalResponse<TId, TResultType>(ProtocolResult<TId, TResultType> result)
            where TId : IProtocolIdentifier
        {
            if (_callback.TryGetValue(result.From, out var senderId))
            {
                Registry[senderId]?.ReceiveMessage(new MessageEnvelope(result, GetMyId()));
            }

            // message is also delivered to self
            Registry[result.From]?.ReceiveMessage(new MessageEnvelope(result, GetMyId()));
        }

        public int GetMyId()
        {
            return _sender;
        }
    }
}