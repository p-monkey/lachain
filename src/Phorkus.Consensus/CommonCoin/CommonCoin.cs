﻿using System;
using System.Linq;
using Google.Protobuf;
using Phorkus.Consensus.CommonCoin.ThresholdCrypto;
using Phorkus.Consensus.Messages;
using Phorkus.Proto;
using Signature = Phorkus.Consensus.CommonCoin.ThresholdCrypto.Signature;

namespace Phorkus.Consensus.CommonCoin
{
    public class CommonCoin : AbstractProtocol
    {
        private readonly IThresholdSigner _thresholdSigner;
        private readonly PublicKeySet _publicKeySet;
        private readonly CoinId _coinId;
        private readonly IConsensusBroadcaster _broadcaster;
        private bool? _result;
        private bool _requested;

        public override IProtocolIdentifier Id => _coinId;

        public CommonCoin(
            PublicKeySet publicKeySet, PrivateKeyShare privateKeyShare,
            CoinId coinId, IConsensusBroadcaster broadcaster
        )
        {
            _publicKeySet = publicKeySet ?? throw new ArgumentNullException(nameof(publicKeySet));
            _coinId = coinId ?? throw new ArgumentNullException(nameof(coinId));
            _broadcaster = broadcaster ?? throw new ArgumentNullException(nameof(broadcaster));
            _thresholdSigner = new ThresholdSigner(_coinId.ToByteArray(), privateKeyShare, publicKeySet);
        }

        public override void ProcessMessage(MessageEnvelope envelope)
        {
            if (envelope.External)
            {
                var message = envelope.ConsensusMessage;
                // These checks are somewhat redundant, but whatever
                if (message.PayloadCase != ConsensusMessage.PayloadOneofCase.Coin)
                    throw new ArgumentException(
                        $"consensus message of type {message.PayloadCase} routed to CommonCoin protocol");
                if (message.Validator.Era != _coinId.Era ||
                    message.Coin.Agreement != _coinId.Agreement ||
                    message.Coin.Epoch != _coinId.Epoch)
                    throw new ArgumentException("era, agreement or epoch of message mismatched");

                if (_result != null)
                {
                    if (_requested)
                    {
                        _broadcaster.InternalResponse(new ProtocolResult<CoinId, bool>(_coinId, (bool) _result));
                    }

                    return;
                }

                var signatureShare = SignatureShare.FromBytes(message.Coin.SignatureShare.ToByteArray());
                if (!_thresholdSigner.AddShare(_publicKeySet[(int) message.Validator.ValidatorIndex], signatureShare,
                    out var signature))
                    return; // potential fault evidence

                if (signature == null) return;
                _result = signature.Parity();
            }
            else
            {
                var message = envelope.InternalMessage;
                switch (message)
                {
                    case ProtocolRequest<CoinId, object> _:
                        var signatureShare = _thresholdSigner.Sign();
                        _requested = true;
                        _broadcaster.Broadcast(CreateCoinMessage(signatureShare));
                        break;
                    case ProtocolResult<CoinId, bool> _:
                        Terminated = true;
                        break;
                    default:
                        throw new InvalidOperationException(
                            $"Binary broadcast protocol handles messages of type {message.GetType()}");
                }
            }
        }

        private ConsensusMessage CreateCoinMessage(Signature share)
        {
            var shareBytes = share.ToBytes().ToArray();
            var message = new ConsensusMessage
            {
                Validator = new Validator
                {
                    // TODO: somehow fill validator field
                    ValidatorIndex = _broadcaster.GetMyId(),
                    Era = _coinId.Era
                },
                Coin = new CommonCoinMessage
                {
                    Agreement = _coinId.Agreement,
                    Epoch = _coinId.Epoch,
                    SignatureShare = ByteString.CopyFrom(shareBytes, 0, shareBytes.Length)
                }
            };
            return new ConsensusMessage(message);
        }
    }
}