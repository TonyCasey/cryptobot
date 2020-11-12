using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Domain.Market;

namespace Api.CryptoBot.Models.Extensions
{
    public static class CoinExtensions
    {
        public static IQueryable<Coin> FilterByCode(this IQueryable<Coin> records, string code)
        {
            if (String.IsNullOrEmpty(code)) return records;

            return records.Where(x =>x.Code.Contains(code));
        }
    }
}
