using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.Core.Extensions
{
    public static class Extensions
    {
        public static string ToStringPreservePrecision(this double input)
        {
            return input.ToString("0." + new string('#', 339));
        }
    }
}
