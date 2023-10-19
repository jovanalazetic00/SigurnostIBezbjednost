using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WCFReplicator : ChannelFactory<IReplicate>, IReplicate, IDisposable
    {
        IReplicate factory;

        public WCFReplicator(NetTcpBinding binding, EndpointAddress address)   //kreira kanal ka replicatoru, vezuje servis sa replicatorom, pozivaju se metode iz interfejsa
            : base(binding, address)
        {

            factory = this.CreateChannel();
        }

        public void AddAccount(string username, string pin, string secretKey)
        {
            try
            {
                factory.AddAccount(username, pin, secretKey);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void RevokeCertificateUpdate(string serialNumber)
        {
            try
            {
                factory.RevokeCertificateUpdate(serialNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TestCommunication()
        {
            factory.TestCommunication();
        }

        public void UpdateAccountBalance(string username, float amount)
        {
            try
            {
                factory.UpdateAccountBalance(username, amount);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateAccountPin(string username, string newPin)
        {
            try
            {
                factory.UpdateAccountPin(username, newPin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
