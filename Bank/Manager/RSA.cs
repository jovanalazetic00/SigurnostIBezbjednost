using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    public class RSA  //koristimo kada korisniku treba da se dostavi pin i secret key, sa vjezbi uzeto
    {
        public static string Encrypt(string text, string publicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            byte[] byteText = Encoding.UTF8.GetBytes(text);
            byte[] byteEntry = rsa.Encrypt(byteText, false);

            return Convert.ToBase64String(byteEntry);
        }

        public static string Decrypt(string encryptedText, string privateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            byte[] byteEntry = Convert.FromBase64String(encryptedText);
            byte[] byteText = rsa.Decrypt(byteEntry, false);

            return Encoding.UTF8.GetString(byteText);
        }
    }
}
