using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [ServiceContract]
    public interface IBankingAudit  //da upise u event viewer
    {
        [OperationContract]
        void TestCommunication();

        [OperationContract]
        void Notify(string bankName, string accountName, DateTime detectionTime, List<string> logs);
        
    }
}
