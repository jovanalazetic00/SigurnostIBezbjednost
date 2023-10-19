using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
	public class SecretKey
	{
		public static string GenerateKey()  //za simetricne algoritme generise secret key
		{
			SymmetricAlgorithm symmAlgorithm = TripleDESCryptoServiceProvider.Create();

			return symmAlgorithm == null ? String.Empty : ASCIIEncoding.ASCII.GetString(symmAlgorithm.Key);
		}

		public static void StoreKey(string secretKey, string outFile)  //da se upise kljuc u fajl, npr u pera da se upise secret key
		{ 
			FileStream file = new FileStream(outFile, FileMode.OpenOrCreate, FileAccess.Write);
			byte[] buffer = Encoding.ASCII.GetBytes(secretKey);

			try
			{
				file.Write(buffer, 0, buffer.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine("Greska prilikom snimanja kljuca. ERROR {0}", e.Message);
			}
			finally
			{
				file.Close();
			}
		}

		public static string LoadKey(string inFile)   //da iscita secret key iz fajla
		{
			FileStream file = new FileStream(inFile, FileMode.Open, FileAccess.Read);
			byte[] buffer = new byte[(int)file.Length];

			try
			{
				file.Read(buffer, 0, (int)file.Length);
			}
			catch (Exception e)
			{
				Console.WriteLine("Greska prilikom ucitavanja kljuca. ERROR {0}", e.Message);
			}
			finally
			{
				file.Close();
			}

			return ASCIIEncoding.ASCII.GetString(buffer);
		}
	}
}
