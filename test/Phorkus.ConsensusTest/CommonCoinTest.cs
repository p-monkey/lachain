using System.Linq;
using NUnit.Framework;
using Phorkus.Consensus;
using Phorkus.Consensus.CommonCoin;
using Phorkus.Consensus.Messages;
using Phorkus.Crypto.MCL.BLS12_381;
using Phorkus.Crypto.ThresholdSignature;
using Phorkus.Proto;

namespace Phorkus.ConsensusTest
{
    public class CommonCoinTest
    {
        private const int N = 7, F = 2;
        private IConsensusProtocol[] _coins;
        private IConsensusBroadcaster[] _broadcasters;
        private DeliveryService _deliveryService;
        private ProtocolInvoker<CoinId, CoinResult>[] _resultInterceptors;
        private IPrivateConsensusKeySet[] _wallets;
        private IPublicConsensusKeySet _publicKeys;

        public void SetUp()
        {
            Mcl.Init();
            var keygen = new TrustedKeyGen(N, F);
            var shares = keygen.GetPrivateShares().ToArray();
            var pubKeys = new PublicKeySet(shares.Select(share => share.GetPublicKeyShare()), F);
            _deliveryService = new DeliveryService();
            _coins = new IConsensusProtocol[N];
            _broadcasters = new IConsensusBroadcaster[N];
            _resultInterceptors = new ProtocolInvoker<CoinId, CoinResult>[N];
            _wallets = new IPrivateConsensusKeySet[N];
            _publicKeys = new PublicConsensusKeySet(
                N, F, null, null,
                pubKeys, Enumerable.Empty<ECDSAPublicKey>()
            );
            for (var i = 0; i < N; ++i)
            {
                _resultInterceptors[i] = new ProtocolInvoker<CoinId, CoinResult>();
                _wallets[i] = new PrivateConsensusKeySet(null, null, shares[i]);
                _broadcasters[i] = new BroadcastSimulator(i, _publicKeys, _wallets[i], _deliveryService, false);
                _coins[i] = new CommonCoin(
                    new CoinId(0, 0, 0), _publicKeys, shares[i], _broadcasters[i]
                );
                _broadcasters[i].RegisterProtocols(new[] {_coins[i], _resultInterceptors[i]});
            }
        }

        private void Run()
        {
            for (var i = 0; i < N; ++i)
            {
                _broadcasters[i].InternalRequest(
                    new ProtocolRequest<CoinId, object>(_resultInterceptors[i].Id, (CoinId) _coins[i].Id, null)
                );
            }

            for (var i = 0; i < N; ++i)
            {
                _coins[i].WaitFinish();
            }

            _deliveryService.WaitFinish();

            var results = new CoinResult[N];
            for (var i = 0; i < N; ++i)
            {
                Assert.IsTrue(_coins[i].Terminated, $"protocol {i} did not terminate");
                Assert.AreEqual(_resultInterceptors[i].ResultSet, 1, $"protocol {i} emitted result not once");
                results[i] = _resultInterceptors[i].Result;
            }

            Assert.AreEqual(1, results.Distinct().Count(), "all guys should get same coin");
        }

        [Test]
        [Repeat(100)]
        public void TestAllHonest()
        {
            SetUp();
            Run();
        }

        [Test]
        [Repeat(100)]
        public void TestAllHonestWithRepeat()
        {
            SetUp();
            _deliveryService.RepeatProbability = 0.9;
            Run();
        }
    }
}