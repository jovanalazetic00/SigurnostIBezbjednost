using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
    //koristi se za razmjenu poruka izmedju klijenta i banke
    public class TripleDES   //gotova klasa sa vjezbi
    {
        public static byte[] Encrypt(byte[] input, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            // Za hashovan kljuc
            byte[] keyBuffer;

            // Za kriptovanu poruku
            byte[] byteBuff;
            byteBuff = input;

            keyBuffer = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));  //hasuje se key bafer

            desCryptoProvider.Key = keyBuffer;   //postavimo kljuc 
            desCryptoProvider.Mode = CipherMode.ECB;   //postavimo ECB mod, trazeno u zadatku

            byte[] encrypted =
                desCryptoProvider.CreateEncryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);  //poziva se metoda za enkriptovanje

            return encrypted;
        }

        public static byte[] Decrypt(byte[] input, string key)
        {
            TripleDESCryptoServiceProvider desCryptoProvider = new TripleDESCryptoServiceProvider();
            MD5CryptoServiceProvider hashMD5Provider = new MD5CryptoServiceProvider();

            // Za hashovan kljuc
            byte[] keyBuffer;

            // Za dekriptovanu poruku
            byte[] byteBuff;
            byteBuff = input;

            keyBuffer = hashMD5Provider.ComputeHash(Encoding.UTF8.GetBytes(key));
            desCryptoProvider.Key = keyBuffer;
            desCryptoProvider.Mode = CipherMode.ECB;

            byte[] plaintext =
                desCryptoProvider.CreateDecryptor().TransformFinalBlock(byteBuff, 0, byteBuff.Length);  //poziva se metoda za dekriptovanje

            return plaintext;
        }
    }
}
