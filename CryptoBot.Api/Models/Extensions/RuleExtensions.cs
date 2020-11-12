using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Domain.Bot;

namespace Api.CryptoBot.Models.Extensions
{
    public static class RuleExtensions
    {
        public static IQueryable<Rule> FilterByIndicatorRuleTypeId(this IQueryable<Rule> records, int? indicatorRuleTypeId)
        {
            if (indicatorRuleTypeId == null) return records;

            return records.Where(x =>x.IndicatorRuleTypeId == indicatorRuleTypeId);
        }
    }
}
