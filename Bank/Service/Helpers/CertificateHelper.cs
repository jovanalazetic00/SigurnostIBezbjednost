using Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    internal static class CertificateHelper
    {
        public static void GenerateCertificateForAuth(string directory, string certCn)
        {
            Process p = new Process();

            string arguments = string.Format("-sv {0}.pvk -iv BankCA.pvk -n \"CN={0}\" -pe -ic BankCA.cer {0}.cer -sr localmachine -ss My -sky exchange", certCn);

            ProcessStartInfo info = new ProcessStartInfo(directory + @"\makecert.exe", arguments);
            info.WorkingDirectory = directory;

            p.StartInfo = info;

            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }

            p.WaitForExit();
            p.Dispose();
        }

        public static void GeneratePvk(string directory, string certCn)
        {
            Process p = new Process();

            string arguments = string.Format("/pvk {0}.pvk /pi 123 /spc {0}.cer /pfx {0}.pfx", certCn);

            ProcessStartInfo info = new ProcessStartInfo(directory + @"\pvk2pfx.exe", arguments);
            info.WorkingDirectory = directory;

            p.StartInfo = info;

            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }

            p.WaitForExit();
            p.Dispose();
        }

        public static void GenerateCertificateForDS(string directory, string certCn)
        {
            Process p = new Process();

            string arguments = string.Format("-sv {0}.pvk -iv BankCA.pvk -n \"CN={0}\" -pe -ic BankCA.cer {0}.cer -sr localmachine -ss My -sky signature", certCn);

            ProcessStartInfo info = new ProcessStartInfo(directory + @"\makecert.exe", arguments);
            info.WorkingDirectory = directory;

            p.StartInfo = info;

            try
            {
                p.Start();
            }
            catch (Exception e)
            {
                throw new FaultException<CertException>(
                      new CertException(e.Message));
            }

            p.WaitForExit();
            p.Dispose();
        }
    }
}
