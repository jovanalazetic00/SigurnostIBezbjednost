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
    public class WCFTransaction : ChannelFactory<IBank>, IBank, IDisposable
	{
		IBank factory;

		public WCFTransaction(NetTcpBinding binding, EndpointAddress address)
			: base(binding, address)
		{
			/// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
			this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
			this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientCertValidator();
			this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

			/// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
			this.Credentials.ClientCertificate.Certificate =
				CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

			factory = this.CreateChannel();
		}

		public void TestCommunication()
		{
            try
            {
                factory.TestCommunication();
                Console.WriteLine("[TestCommunication] Uspesno ste se autentifikovali uz pomoc sertifikata.\n");
            }
            catch (Exception)
            {
                Console.WriteLine("[TestCommunication] Ne posedujete sertifikat ili vam je sertifikat povucen.\n");

                throw new Exception();
            }
        }

		public void Dispose()
		{
			if (factory != null)
			{
				factory = null;
			}

			this.Close();
		}

        /*Ovaj blok koda definise metodu pod nazivom "Deposit" koja uzima niz bajtova kao argument.
         Metoda prvo poziva metodu "Depozit" objekta koji se zove "fabrika" i prosljedjuje niz bajtova poruke.
         Zatim ispisuje poruku koja pokazuje da je polog uspjesno izvrsen. Ako metoda "Deposit" objekta "factory" izbaci FaultException 
         tipa BankException, ona hvata izuzetak i ispisuje svojstvo "Razlog" objekta "Detail" izuzetka.
         Ako izbaci bilo koji drugi izuzetak, hvata ga i ispisuje svojstvo "Poruka" izuzetka.*/

        public void Deposit(byte[] message)
        {
            try
            {
                factory.Deposit(message);

                Console.WriteLine("Uspesno uplacen depozit.");
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[Deposit] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Deposit] " + e.Message);
            }
        }

        /*Ovaj kod je jos jedna verzija metode povlacenja, cini se da je omotac za prethodno prikazanu metodu povlacenja.
         Kao ulaz prima sifrovanu poruku. Zatim poziva metodu 'factory.Withdraw(message)' koja je vjerovatno prethodno prikazana metoda 'withdraw'.
         Ako metoda povlacenja izbaci izuzetak 'FaultException<BankException>', exp.Detail.Reason ispisuje se na konzoli. 
         Ako se uhvati bilo koji drugi izuzetak, e.Message se ispisuje na konzolu. Ova metoda omotaca je korisna za hvatanje bilo
         kakvih gresaka koje je izbacila metoda 'factory.Withdraw(message)', njihovo biljezenje i zatim pruzanje poruke prilagodjene 
         korisniku konzoli.*/

        public void Withdraw(byte[] message)
        {
            try
            {
                factory.Withdraw(message);

                Console.WriteLine("Uspesno isplacen depozit.");
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[Withdraw] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[Withdraw] " + e.Message);
            }
        }

        public byte[] ResetPin(byte[] message)
        {
            byte[] newPin = null;

            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

            string secretKey = SecretKey.LoadKey(clientName);  //ucitamo secret key korisnika, isto kao uplata/isplata samo se salje pin

            try
            {
                byte[] encrypted = factory.ResetPin(message);

                byte[] decrypted = TripleDES.Decrypt(encrypted, secretKey);  //dekriptujem novi pin koji je banka poslala

                X509Certificate2 signBank =
                    CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, "bank_sign");

                byte[] sign = new byte[256];
                newPin = new byte[decrypted.Length - 256];
                //na prvih 256 bita ide potpisana poruka
                Buffer.BlockCopy(decrypted, 0, sign, 0, 256);
                //na posljednje 4 lokacije ide pin, jer je pin 4 cifre
                Buffer.BlockCopy(decrypted, 256, newPin, 0, decrypted.Length - 256);

                string newPinStr = System.Text.Encoding.UTF8.GetString(newPin);
                //newPinStr je plaintext tog pina i provjerava je li resetovan pin, i ispise novi
                if (DigitalSignature.Verify(newPinStr, sign, signBank))
                {
                    Console.WriteLine("\nUspesno resetovan PIN. Novi PIN: " + newPinStr);
                }
                else
                {
                    Console.WriteLine("Neuspesna verifikacija.");
                }
            }
            catch (FaultException<BankException> exp)
            {
                Console.WriteLine("[ResetPin] " + exp.Detail.Reason);
            }
            catch (Exception e)
            {
                Console.WriteLine("[ResetPin] " + e.Message);
            }

            return newPin;
        }
    }
}
