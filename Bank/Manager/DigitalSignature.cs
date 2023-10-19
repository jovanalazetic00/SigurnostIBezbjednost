using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class DigitalSignature
    {
        public enum HashAlgorithm { SHA1, SHA256 }

        /*Ovaj kod stvara digitalni potpis za datu poruku pomocu dostavljenog X509 certifikata. 
         Privatni kljuc iz certifikata koristi se za izradu potpisa za SHA-1 hash poruke. Poruka se prvo kodira kao Unicode niz, 
         a zatim rastavlja algoritmom SHA1Managed. Rezultujuci hash se zatim potpisuje koriscenjem privatnog kljuca, 
         a algoritam potpisivanja specificira se kao SHA-1 koriscenjem metode CryptoConfig.MapNameToOID. Rezultujuci potpis vraca se kao niz bajtova.*/

        public static byte[] Create(string message, X509Certificate2 certificate)  //ucitavam pfx
        {  //kod transakcija, kod uplate, isplate i promjene pina
            byte[] sign = null;

            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PrivateKey;  

            if (csp == null)
            {
                return sign;
            }

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] buffer = encoding.GetBytes(message);

            SHA1Managed sha256 = new SHA1Managed();
            byte[] hash = sha256.ComputeHash(buffer);

            sign = csp.SignHash(hash, CryptoConfig.MapNameToOID(HashAlgorithm.SHA1.ToString()));

            return sign;
        }

        /*Ovo je C# funkcija koja provjerava digitalni potpis poruke koristeci X509 certifikat. 
         Funkcija prvo stvara instancu onoga ko pruza RSA kriptografske usluge (csp) koristeci javni kljuc iz certifikata. 
         Zatim pretvara poruku u bajtove koristeci Unicode kodiranje, izracunava SHA1 hash poruke i koristi csp za provjeru potpisa
         u odnosu na izracunati hash. Ako je potpis provjeren, funkcija vraca "true", inace vraca "false".*/

        public static bool Verify(string message, byte[] signature, X509Certificate2 certificate)   //ucitavam cer
        {
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] buffer = encoding.GetBytes(message);

            SHA1Managed sha256 = new SHA1Managed();
            byte[] hash = sha256.ComputeHash(buffer);

            return csp.VerifyHash(hash, CryptoConfig.MapNameToOID(HashAlgorithm.SHA1.ToString()), signature);
        }
    }
}
