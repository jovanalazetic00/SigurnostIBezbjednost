using Contracts;
using Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replicator
{
    public class Replicate : IReplicate  //implementirane metode iz interfejsa IReplicate iz Contracta
    {
        public void AddAccount(string username, string pin, string secretKey)  //dodavanje racuna
        {
            Racun racun = new Racun()
            {
                Username = username,
                Pin = pin,
                Balance = 0
            };

            try
            {
                XMLHelper.AddBankAccount(racun);
                SecretKey.StoreKey(secretKey, username);

                Console.WriteLine("Korisnik {0} repliciran.", racun.Username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void RevokeCertificateUpdate(string serialNumber)  //apdejtuje povucene sertifikate, javlja da je povucen sertifikat
        {
            try
            {
                TXTHelper.SaveSerialNumber(serialNumber);

                Console.WriteLine("Serijski broj {0} repliciran", serialNumber);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void TestCommunication()
        {
            Console.WriteLine("[REPLICATOR] Communication established.");
        }

        public void UpdateAccountBalance(string username, float amount)  //prikazuje apdejtovani bankovni racun
        {
            try
            {
                XMLHelper.UpdateBankAccountBalance(username, amount);

                Console.WriteLine("Korisniku {0} replicirana novo stanje racuna.", username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void UpdateAccountPin(string username, string newPin)  //prikazuje apdejtovani pin korisnika
        {
            try
            {
                XMLHelper.UpdateBankAccount(username, newPin);

                Console.WriteLine("Korisniku {0} replicirana novi pin.", username);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
