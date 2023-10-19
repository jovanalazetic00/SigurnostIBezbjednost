using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IReplicate
    {
        [OperationContract]
        void TestCommunication();

        [OperationContract]
        void AddAccount(string username, string pin, string secretKey);

        [OperationContract]
        void RevokeCertificateUpdate(string serialNumber);

        [OperationContract]
        void UpdateAccountPin(string username, string newPin);

        [OperationContract]
        void UpdateAccountBalance(string username, float amount);
    }
}
