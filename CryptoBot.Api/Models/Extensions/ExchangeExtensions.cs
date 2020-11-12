using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Domain.Market;

namespace Api.CryptoBot.Models.Extensions
{
    public static class ExchangeExtensions
    {
        public static IQueryable<Exchange> FilterByName(this IQueryable<Exchange> records, string name)
        {
            if (String.IsNullOrEmpty(name)) return records;

            return records.Where(x =>x.Name.Contains(name));
        }

        public static IQueryable<Exchange> FilterByCode(this IQueryable<Exchange> records, string code)
        {
            if (String.IsNullOrEmpty(code)) return records;

            return records.Where(x =>x.Code.Contains(code));
        }
    }
}
