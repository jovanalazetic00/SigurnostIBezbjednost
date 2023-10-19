using Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static WCFCert bankCert = null;
        private static WCFTransaction bankTransaction = null;

        static void Main(string[] args)
        {
            // Proxy za sertifikate

            CertProxy();

            Console.WriteLine("Da li zelite da kreirate racun u banci? [Y/N]");
            string answer = Console.ReadLine();

            if(answer.Equals("Y") || answer.Equals("y"))
            {
                string pin = bankCert.CardRequest();
            }

            try
            {
                BankProxy();  //povezuje se sa endpointom za transakcije

                Menu();

                bankCert.Close();
                bankTransaction.Close();
            }
            catch (Exception)
            {

            }

            Console.WriteLine("\nPress <enter> to stop ...");
            Console.ReadLine();
        }

        private static void CertProxy()  //poveze se sa certom
        {
            NetTcpBinding binding = new NetTcpBinding();
            string address = "net.tcp://localhost:17002/Cert";

            binding.Security.Mode = SecurityMode.Transport;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;


            EndpointAddress endpointAddress = new EndpointAddress(new Uri(address));

            bankCert = new WCFCert(binding, endpointAddress);

            bankCert.TestCommunication();
        }

        private static void BankProxy()
        {
            string srvCertCN = "bankservice";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, srvCertCN);

            EndpointAddress endpointAddress = new EndpointAddress(new Uri("net.tcp://localhost:17001/BankTransaction"),
                                      new X509CertificateEndpointIdentity(srvCert));

            bankTransaction = new WCFTransaction(binding, endpointAddress);

            bankTransaction.TestCommunication();
        }

        private static void Menu()
        {
            bool end = false;

            do {

                Console.WriteLine("1. Zahtev za novim MasterCard sertifikatom");
                Console.WriteLine("2. Uplata");
                Console.WriteLine("3. Isplata");
                Console.WriteLine("4. Promena PIN-a");
                Console.WriteLine("5. Izlazak");

                Console.WriteLine("-> ");
                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        {
                            Console.WriteLine("PIN: ");

                            string pin = Console.ReadLine();

                            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);  //trenutni korisnik

                            string secretKey = SecretKey.LoadKey(clientName);  //ucitavam secret key

                            byte[] pinBuffer = System.Text.Encoding.UTF8.GetBytes(pin);

                            byte[] encrypted = TripleDES.Encrypt(pinBuffer, secretKey);
                            try
                            {
                                bankCert.RevokeRequest(encrypted);  //da se povuce sertifikat
                                end = true;
                            }
                            catch (Exception)
                            {
                            }
                        }
                        break;

                    /*Cini se da je ovaj blok koda dio naredbe switch koja obradjuje razlicite slucajeve. cini se da slučaj "2" obradjuje 
                     depozitnu transakciju. Kod prvo dobija ime trenutnog korisnika i oblikuje ga metodom "Formatter.ParseName". 
                     Zatim trazi od korisnika da unese "PIN" i "Iznos" (vjerovatno znaci "iznos" na drugom jeziku) i povezuje ih u jedan niz poruka. 
                     Zatim pretvara niz poruke u niz bajtova. Zatim dohvata sertifikat za potpisivanje za trenutnog korisnika iz skladista certifikata
                     lokalnog racunara i koristi ga za stvaranje digitalnog potpisa za poruku.
                     Niz bajtova potpisa i poruke spojeni su u jedan niz bajtova otvorenog teksta. Zatim ucitava tajni kljuc za trenutnog korisnika 
                     i koristi ga za sifrovanje niza bajtova otvorenog teksta koristeci TripleDES enkripciju. Na kraju, poziva metodu "Deposit" 
                     objekta "bankTransaction" i prosljedjuje sifrovano polje bajtova kao argument.*/
                     //uplata
                    case "2":
                        {
                            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                            Console.WriteLine("PIN: ");

                            string pin = Console.ReadLine();

                            Console.WriteLine("Iznos: ");

                            string amount = Console.ReadLine();

                            string message = pin + "-" + amount;

                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);  //pretvara niz poruk u niz bajtova

                            X509Certificate2 signCert =
                                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientName + "_sign");

                            byte[] signedMessage = DigitalSignature.Create(message, signCert);

                            byte[] plaintext = new byte[256 + buffer.Length];

                            Buffer.BlockCopy(signedMessage, 0, plaintext, 0, 256);
                            Buffer.BlockCopy(buffer, 0, plaintext, 256, buffer.Length);

                            string secretKey = SecretKey.LoadKey(clientName);

                            byte[] encrypted = TripleDES.Encrypt(plaintext, secretKey);

                            bankTransaction.Deposit(encrypted);
                        }
                        break;

                    /*Pocinje izdvajanjem imena klijenta iz trenutnog Windows identiteta. Zatim od korisnika trazi PIN i iznos koji treba podici.
                     PIN i iznos spojeni su u jedan niz i kodirani u niz bajtova. Potpisni certifikat klijenta zatim se dohvata iz skladista
                     certifikata lokalne masine. Poruka se potpisuje pomocu certifikata, a potpis se dodaje otvorenoj tekstualnoj poruci.
                     Poruka otvorenog teksta se zatim sifruje koriscenjem tajnog kljuca klijenta, a dobijeni sifrovani tekst prosljedjuje se metodi
                     'Povlacenje'.*/
                     //isplata
                    case "3":
                        {
                            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                            Console.WriteLine("PIN: ");

                            string pin = Console.ReadLine();

                            Console.WriteLine("Iznos: ");

                            string amount = Console.ReadLine();

                            string message = pin + "-" + amount;

                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);

                            X509Certificate2 signCert =
                                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientName + "_sign");

                            byte[] signedMessage = DigitalSignature.Create(message, signCert);

                            byte[] plaintext = new byte[256 + buffer.Length];  //na 256 bita stavljam potpisanu poruku, a na duzinu bafera stavljam sto akorisnik zeli da posalje

                            Buffer.BlockCopy(signedMessage, 0, plaintext, 0, 256);
                            //na ostatak je pin+iznos
                            Buffer.BlockCopy(buffer, 0, plaintext, 256, buffer.Length);

                            string secretKey = SecretKey.LoadKey(clientName);

                            byte[] encrypted = TripleDES.Encrypt(plaintext, secretKey);

                            bankTransaction.Withdraw(encrypted);
                        }
                        break;
                    case "4":
                        {
                            string clientName = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);

                            Console.WriteLine("PIN: ");

                            string pin = Console.ReadLine();

                            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(pin);

                            X509Certificate2 signCert =
                                CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, clientName + "_sign");

                            byte[] signedMessage = DigitalSignature.Create(pin, signCert);

                            string secretKey = SecretKey.LoadKey(clientName);

                            byte[] plaintext = new byte[256 + buffer.Length];

                            Buffer.BlockCopy(signedMessage, 0, plaintext, 0, 256);
                            Buffer.BlockCopy(buffer, 0, plaintext, 256, buffer.Length);

                            byte[] encrypted = TripleDES.Encrypt(plaintext, secretKey);

                            bankTransaction.ResetPin(encrypted);
                        }
                        break;
                    case "5":
                        end = true;
                        break;
                    default:
                        Console.WriteLine("Nepoznata komanda!\n");
                        break;
                }
            } while (!end);
        }
    }
}
