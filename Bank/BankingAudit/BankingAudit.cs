using Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingAudit
{
    public class BankingAudit : IBankingAudit
    {
        public void Notify(string bankName, string accountName, DateTime detectionTime, List<string> amounts) //ako se izvrse 3 isplate sa racuna, upise u fajl 
        {
            string path = "logs.txt";
            string text;

            text = String.Format("Source name: {0}\n", bankName);

            text += String.Format("Account name: {0}\n", accountName);

            text += String.Format("Detection time: {0}\n", detectionTime);

            text += "Amounts: ";

            foreach (string amount in amounts)
            {

                text += String.Format("{0} ", amount);
            }

            text += "\n\n";

            using (StreamWriter file = new StreamWriter(path, true))
            {
                file.WriteLine(text);
            }

            Console.WriteLine("Upisan novi log!");
        }

        public void TestCommunication()
        {
            Console.WriteLine("[Banking Audit] Communication established.");
        }
    }
}
