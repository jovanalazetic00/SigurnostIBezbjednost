using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Replicator
{
    static class XMLHelper
    {
        static public void AddBankAccount(Racun racun)
        {
            List<Racun> racuni = ReadAllBankAccounts();
            racuni.Add(racun);

            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextWriter textWriter = new StreamWriter("../../bankAccounts.xml"))
            {
                serializer.Serialize(textWriter, racuni);
            }
        }

        static public List<Racun> ReadAllBankAccounts()
        {
            List<Racun> racuni = new List<Racun>();
            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextReader textReader = new StreamReader("../../bankAccounts.xml"))
            {
                racuni = (List<Racun>)serializer.Deserialize(textReader);
            }
            return racuni;
        }

        static public void UpdateBankAccount(string username, string newPin)
        {
            List<Racun> racuni = ReadAllBankAccounts();

            foreach (var racun in racuni)
            {
                if (racun.Username.Equals(username))
                {
                    racun.Pin = newPin;
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextWriter textWriter = new StreamWriter("../../bankAccounts.xml"))
            {
                serializer.Serialize(textWriter, racuni);
            }
        }

        /*Ovaj kod je metoda koja azurira stanje bankovnog racuna za odredjeno korisnicko ime za odredjeni iznos.
         Zapocinje citanjem svih bankovnih racuna iz XML datoteke pomocu metode 'ReadAllBankAccounts'. Zatim se ponavlja kroz popis bankovnih racuna,
         trazeci racun povezan s datim korisnickim imenom. Kada pronadje racun, azurira stanje dodavanjem zadatog iznosa na njega.
         Zatim stvara XmlSerializer i TextWriter za pisanje azuriranog popisa racuna u XML datoteku "bankAccounts.xml"
         Vazno je napomenuti da ne radi nikakve provjere valjanosti, npr je li iznos negativan ili postoji li racun ili ne. 
         Stoga to treba uciniti prije poziva ove metode.*/

        static public void UpdateBankAccountBalance(string username, float amount)
        {
            List<Racun> racuni = ReadAllBankAccounts();

            foreach (var racun in racuni)
            {
                if (racun.Username.Equals(username))
                {
                    racun.Balance += amount;
                }
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<Racun>));
            using (TextWriter textWriter = new StreamWriter("../../bankAccounts.xml"))
            {
                serializer.Serialize(textWriter, racuni);
            }
        }

    }
}
