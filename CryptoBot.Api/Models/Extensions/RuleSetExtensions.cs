using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;

namespace Api.CryptoBot.Models.Extensions
{
    public static class RuleSetExtensions
    {
        public static IQueryable<RuleSet> FilterByIndicatorId(this IQueryable<RuleSet> records, long? inidcatorId)
        {
            if (inidcatorId == null) return records;

            return records.Where(x =>x.IndicatorId == inidcatorId);
        }
    }
}
