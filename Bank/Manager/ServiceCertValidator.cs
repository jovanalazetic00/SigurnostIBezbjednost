using Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Manager
{
	public class ServiceCertValidator : X509CertificateValidator
	{
		/// <summary>
		/// Implementation of a custom certificate validation on the service side.
		/// Service should consider certificate valid if its issuer is the same as the issuer of the service.
		/// If validation fails, throw an exception with an adequate message.
		/// </summary>
		/// <param name="certificate"> certificate to be validate </param>
		public override void Validate(X509Certificate2 certificate)
		{
			/// This will take service's certificate from storage
			X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, "bankservice"); //povuce pfx sertifikat za bank service sto smo pravili


            List<string> serialNumbers = TXTHelper.ReadSerialNumbers();

            //provjerava je li sertifikat sa tim serijskim brojem,koji je klijent poslao povucen

            if (serialNumbers.Contains(certificate.SerialNumber))
            {
                throw new Exception("Sertifikat je povucen.");
            }

            //provjerava da li je server izdao klijentov sertifikat
            if (!certificate.Issuer.Equals(srvCert.Issuer))
			{
				throw new Exception("Certificate is not from the valid issuer.");
			}
		}
	}
}
