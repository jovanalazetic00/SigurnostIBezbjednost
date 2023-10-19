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

namespace Client
{
	public class WCFCert : ChannelFactory<ICert>, ICert, IDisposable  //za instaliranje sertifikata
	{
		ICert factory;

		public WCFCert(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{

			factory = this.CreateChannel();
		}

		public void TestCommunication()
		{
			try
			{
				factory.TestCommunication();
			}
			catch (Exception e)
			{
				Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
			}
		}  
        //otvaranje racuna
		public string CardRequest()
		{
			string pin = String.Empty;
			try
			{
				string encryptedMessage = factory.CardRequest();

                Console.WriteLine("Molimo Vas da instalirate sertifikate. Nakon toga pritisnite <Enter>.");

				Console.ReadKey();

				string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

				// Dekripcija poslatog pina i tajnog kljuca

				X509Certificate2 cert =
					CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientName);

				string decryptedMessage = Manager.RSA.Decrypt(encryptedMessage, cert.GetRSAPrivateKey().ToXmlString(true));

				pin = decryptedMessage.Substring(decryptedMessage.Length - 4, 4);
				string secretKey = decryptedMessage.Substring(0, decryptedMessage.Length - 4);

				SecretKey.StoreKey(secretKey, clientName);

				Console.WriteLine("Racun na ime {0} je uspesno kreiran. Vas pin je: {1}\n", clientName, pin);
			}
			catch (FaultException<CertException> exp)
			{
				Console.WriteLine("[CardRequest] " + exp.Detail.Reason);
			}
			catch (Exception e)
			{
				Console.WriteLine("[CardRequest] ERROR = {0}", e.Message);
			}

			return pin;
		}
		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        public void RevokeRequest(byte[] message)
        {
            try
            {
                factory.RevokeRequest(message);

                Console.WriteLine("Molimo Vas da instalirate sertifikate. Nakon toga pritisnite <Enter>.");

                Console.ReadKey();

                Console.WriteLine("Pokrenite ponovo aplikaciju kako bi promene bile vidljive.");

            }
            catch (FaultException<CertException> exp)
            {
                Console.WriteLine("[RevokeRequest] " + exp.Detail.Reason);

                throw new Exception();
            }
            catch (Exception e)
            {
                Console.WriteLine("[RevokeRequest] ERROR = {0}", e.Message);
            }
        }
    }
}
