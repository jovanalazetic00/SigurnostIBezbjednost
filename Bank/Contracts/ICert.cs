using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface ICert  //metode za sertifikate
    {
        [OperationContract]
        [FaultContract(typeof(CertException))]
        void TestCommunication();

        [OperationContract]
        [FaultContract(typeof(CertException))]
        string CardRequest();

        [OperationContract]
        [FaultContract(typeof(CertException))]
        void RevokeRequest(byte[] message);

    }
}
