using Contracts;
using Manager;
using Service.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Bank : IBank
    {
/*Cini se da je ovaj kod metoda za rukovanje pologom u bankarskoj aplikaciji. Metoda uzima "poruku" niza bajtova za koju se pretpostavlja 
da je sifrovana. Metoda izvodi nekoliko operacija na ovoj poruci:
Uzima ime klijenta iz sigurnosnog konteksta i koristi ga za ucitavanje tajnog kljuca
Koristi kljuc za desifrovanje poruke
Poruku dijeli na dva dijela: potpis i stvarni iznos depozita
Dohvata klijentov potpisni certifikat iz skladista i koristi ga za provjeru potpisa
Ako je potpis vazeci, izdvaja iznos depozita i PIN iz poruke
Ucitava popis bankovnih racuna iz XML skladista i pronalazi racun koji pripada klijentu
Ako je PIN ispravan, azurira stanje racuna s iznosom depozita i biljezi uspjeh u revizijskom dnevniku
U suprotnom, izbacuje izuzetak i biljezi gresku u dnevnik revizije.*/

        public void Deposit(byte[] message)
        {
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string secretKey = SecretKey.LoadKey(clientName);

            byte[] decrypted = TripleDES.Decrypt(message, secretKey);

            byte[] sign = new byte[256];
            byte[] body = new byte[decrypted.Length - 256];

            Buffer.BlockCopy(decrypted, 0, sign, 0, 256);
            Buffer.BlockCopy(decrypted, 256, body, 0, decrypted.Length - 256);

            string decryptedMessage = System.Text.Encoding.UTF8.GetString(body);

            X509Certificate2 signCert =
                CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName + "_sign");

            if (DigitalSignature.Verify(decryptedMessage, sign, signCert))
            {
                string pin = decryptedMessage.Split('-')[0];
                string amount = decryptedMessage.Split('-')[1];

                List<Racun> racuni = XMLHelper.ReadAllBankAccounts();

                var racun = racuni.Find(x => x.Username.Equals(clientName));

                if (racun.Pin.Equals(HashHelper.HashPassword(pin)))
                {
                    float floatAmount = 0;

                    if (float.TryParse(amount, out floatAmount))
                    {
                        XMLHelper.UpdateBankAccountBalance(clientName, floatAmount);
                        Audit.DepositSuccess(clientName, floatAmount);
                        Program.replicatorProxy.UpdateAccountBalance(clientName, floatAmount);
                    }
                    else
                    {
                        Audit.DepositFailure(clientName, "Kolicina za uplatu mora biti broj");
                        throw new FaultException<BankException>(
                            new BankException("Kolicina za uplatu mora biti broj."));
                    }

                }
                else
                {
                    Audit.DepositFailure(clientName, "Pogresan pin!");
                    throw new FaultException<BankException>(
                        new BankException("Pogresan pin."));
                }
            }
            else
            {
                Audit.DepositFailure(clientName, "Potpis nije validan!");
                throw new FaultException<BankException>(
                    new BankException("Potpis nije validan."));
            }
        }

        /*Ovaj kod je metoda za podizanje novca s bankovnog racuna. Zapocinje izvlacenjem imena klijenta iz poruke i ucitavanjem tajnog kljuca 
         povezanog s tim klijentom. Poruka se zatim dekriptuje koriscenjem tajnog kljuca, a potpis i tijelo poruke se izdvajaju. 
         Potpis se provjerava koriscenjem klijentovog sertifikata za potpisivanje, a desifrovana poruka se rasclanjuje kako bi se izdvojio PIN
         i iznos koji treba povuci. Kod zatim provjerava ima li na racunu dovoljno sredstava i je li pin tacan. Ako je sve u redu, stanje racuna 
         se azurira, isplata se biljezi, a stanje racuna se azurira na serveru replikatora. Ako bilo koja od provjera ne uspije,
         izbacuje se izuzetak i neuspjeh se biljezi.*/

        public void Withdraw(byte[] message)
        {
            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string secretKey = SecretKey.LoadKey(clientName);

            byte[] decrypted = TripleDES.Decrypt(message, secretKey);

            byte[] sign = new byte[256];
            byte[] body = new byte[decrypted.Length - 256];

            Buffer.BlockCopy(decrypted, 0, sign, 0, 256);
            Buffer.BlockCopy(decrypted, 256, body, 0, decrypted.Length - 256);

            string decryptedMessage = System.Text.Encoding.UTF8.GetString(body);

            X509Certificate2 signCert =
                CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName + "_sign");

            if (DigitalSignature.Verify(decryptedMessage, sign, signCert))
            {
                string pin = decryptedMessage.Split('-')[0];
                string amount = decryptedMessage.Split('-')[1];

                List<Racun> racuni = XMLHelper.ReadAllBankAccounts();

                var racun = racuni.Find(x => x.Username.Equals(clientName));

                if (racun.Pin.Equals(HashHelper.HashPassword(pin)))
                {
                    float floatAmount = 0;

                    if (float.TryParse(amount, out floatAmount))
                    {
                        if (racun.Balance - floatAmount >= 0)
                        {
                            XMLHelper.UpdateBankAccountBalance(clientName, -floatAmount);
                            Audit.WithdrawSuccess(clientName, floatAmount);
                            Program.replicatorProxy.UpdateAccountBalance(clientName, -floatAmount);
                            Task.Run(() => LogHelper.CheckLogs(clientName));
                        }
                        else
                        {
                            Audit.WithdrawFailure(clientName, "Nedovoljno sredstava na racunu");
                            throw new FaultException<BankException>(
                                new BankException("Nemate dovoljno sredstava na racunu."));
                        }
                    }
                    else
                    {
                        Audit.WithdrawFailure(clientName, "Kolicina za uplatu mora biti broj!");
                        throw new FaultException<BankException>(
                            new BankException("Kolicina za uplatu mora biti broj."));
                    }

                }
                else
                {
                    Audit.WithdrawFailure(clientName, "Pogresan pin!");
                    throw new FaultException<BankException>(
                        new BankException("Pogresan pin."));
                }
            }
            else
            {
                Audit.WithdrawFailure(clientName, "Potpis nije validan!");
                throw new FaultException<BankException>(
                    new BankException("Potpis nije validan."));
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("[TRANSACTION] Communication established.");
        }

        public byte[] ResetPin(byte[] message)
        {
            byte[] encrypted = null;

            string clientName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            string secretKey = SecretKey.LoadKey(clientName);

            byte[] decrypted = TripleDES.Decrypt(message, secretKey);

            byte[] sign = new byte[256];
            byte[] body = new byte[decrypted.Length - 256];

            Buffer.BlockCopy(decrypted, 0, sign, 0, 256);
            Buffer.BlockCopy(decrypted, 256, body, 0, decrypted.Length - 256);

            string oldPin = System.Text.Encoding.UTF8.GetString(body);

            X509Certificate2 signCert =
                CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, clientName + "_sign");

            if (DigitalSignature.Verify(oldPin, sign, signCert))  //da se generise novi pin
            {

                List<Racun> racuni = XMLHelper.ReadAllBankAccounts();

                var racun = racuni.Find(x => x.Username.Equals(clientName));

                if (racun.Pin.Equals(HashHelper.HashPassword(oldPin)))
                {
                    string newPin = PinHelper.GeneratePin();

                    byte[] pinBuffer = System.Text.Encoding.UTF8.GetBytes(newPin);

                    XMLHelper.UpdateBankAccount(clientName, HashHelper.HashPassword(newPin));

                    X509Certificate2 signBank =
                        CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "bank_sign");

                    byte[] signedMessage = DigitalSignature.Create(newPin, signBank);

                    byte[] plaintext = new byte[256 + pinBuffer.Length];

                    Buffer.BlockCopy(signedMessage, 0, plaintext, 0, 256);
                    Buffer.BlockCopy(pinBuffer, 0, plaintext, 256, pinBuffer.Length);

                    encrypted = TripleDES.Encrypt(plaintext, secretKey);

                    Program.replicatorProxy.UpdateAccountPin(clientName, HashHelper.HashPassword(newPin));
                    Audit.ResetPinSuccess(clientName);
                    return encrypted;
                }
                else
                {
                    Audit.ResetPinFailure(clientName, "Stari pin je pogresan!");
                    throw new FaultException<BankException>(
                        new BankException("Stari pin je pogresan."));
                }
            }
            else
            {
                Audit.ResetPinFailure(clientName, "Potpis nije validan!");
                throw new FaultException<BankException>(
                    new BankException("Potpis nije validan."));
            }
        }
    }
}
