using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WCFBankingAudit : ChannelFactory<IBankingAudit>, IBankingAudit, IDisposable
    {
        IBankingAudit factory;

        public WCFBankingAudit(NetTcpBinding binding, EndpointAddress address)   //kreira se kanal ka bankingAuditu, vezuje server sa tim projektom
            : base(binding, address)
        {

            factory = this.CreateChannel();
        }

        public void Notify(string bankName, string accountName, DateTime detectionTime, List<string> logs)
        {
            try
            {
                factory.Notify(bankName, accountName, detectionTime, logs);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TestCommunication()
        {
            try
            {
                factory.TestCommunication();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
