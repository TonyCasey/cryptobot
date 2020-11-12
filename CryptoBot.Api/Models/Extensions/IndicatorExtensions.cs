using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Bot;

namespace Api.CryptoBot.Models.Extensions
{
    public static class IndicatorExtensions
    {
        public static IQueryable<Indicator> FilterByType(this IQueryable<Indicator> indicators, Enumerations.IndicatorTypeEnum type)
        {
            return indicators.Where(x =>x.IndicatorType == type);
        }

        public static IQueryable<Indicator> FilterByBotId(this IQueryable<Indicator> indicators, long? botId)
        {
            if (botId == null) return indicators;
            return indicators.Where(x =>x.BotId == botId);
        }
    }
}
