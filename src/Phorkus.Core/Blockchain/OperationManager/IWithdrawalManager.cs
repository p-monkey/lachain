using Phorkus.Crypto;
using Phorkus.Proto;

namespace Phorkus.Core.Blockchain.OperationManager
{
    public interface IWithdrawalManager
    {
        void ConfirmWithdrawal(KeyPair keyPair, ulong nonce);

        void ExecuteWithdrawal(ThresholdKey thresholdKey, KeyPair keyPair, ulong nonce);
        
        void Start(ThresholdKey thresholdKey, KeyPair keyPair);

        void Stop();
    }
}