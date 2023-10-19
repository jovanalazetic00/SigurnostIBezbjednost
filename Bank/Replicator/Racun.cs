using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Replicator
{
    [Serializable]
    public class Racun
    {
        string username;
        string pin;
        float balance;
        public Racun()
        {
            username = "";
            pin = "";
            balance = 0;
        }

        public string Username { get => username; set => username = value; }
        public string Pin { get => pin; set => pin = value; }
        public float Balance { get => balance; set => balance = value; }
    }
}
