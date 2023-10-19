using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Helpers
{
    internal static class PinHelper
    {
        public static string GeneratePin()
        {
            string pin = Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

            pin = pin.Substring(0, 4);

            return pin;
        }
    }
}
