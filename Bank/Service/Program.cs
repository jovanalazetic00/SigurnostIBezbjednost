using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    class Program
    {
        public static WCFReplicator replicatorProxy = null;
        public static WCFBankingAudit bankingAuditProxy = null;
        static void Main(string[] args)
        {
            ReplicatorProxy();  //na pocetku se pozivaju metode za konekciju sa bankingAudit i replicator

            BankingAuditProxy();

            // Endpoint za transakcije

            NetTcpBinding binding1 = new NetTcpBinding();
            string address1 = "net.tcp://localhost:17001/BankTransaction";

            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            binding1.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;   //na ovom endpointu se koristi autentifikacija sertifikatima

            ServiceHost host1 = new ServiceHost(typeof(Bank));
            host1.AddServiceEndpoint(typeof(IBank), binding1, address1);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
			host1.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host1.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host1.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            host1.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "bankservice");

            // Endpoint za sertifikate

            NetTcpBinding binding2 = new NetTcpBinding();
            string address2 = "net.tcp://localhost:17002/Cert";

            binding2.Security.Mode = SecurityMode.Transport;
            binding2.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;   //na ovom endpointu se koristi Windows autentifikacija
            binding2.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;

            ServiceHost host2 = new ServiceHost(typeof(Cert));
            host2.AddServiceEndpoint(typeof(ICert), binding2, address2);


            try
            {
                host1.Open();
                Console.WriteLine("[BANK] Running...");
                host2.Open();
                Console.WriteLine("[CERT] Running...");
                Console.WriteLine("\nPress <enter> to stop ...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
            }
            finally
            {
                host1.Close();
                host2.Close();
            }

        }

        private static void ReplicatorProxy()   //da se server konektuje na replicator
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:17003/Replicator";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;


            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            replicatorProxy = new WCFReplicator(binding, endpointAddress);

            replicatorProxy.TestCommunication();
        }

        private static void BankingAuditProxy()   //da se server konektuje na bankingAudit
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:17004/BankingAudit";  

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;


            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            bankingAuditProxy = new WCFBankingAudit(binding, endpointAddress);

            bankingAuditProxy.TestCommunication();
        }
    }
}
