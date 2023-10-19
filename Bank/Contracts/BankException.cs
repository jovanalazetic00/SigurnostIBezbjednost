using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    [DataContract]
    public class BankException
    {
        string reason;
        public BankException(string reason)
        {
            this.reason = reason;
        }

        [DataMember]
        public string Reason { get => reason; set => reason = value; }

    }
}
