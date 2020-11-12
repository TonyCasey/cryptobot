using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Trading;

namespace Api.CryptoBot.Models.Extensions
{
    public static class PositionExtensions
    {

        public static IQueryable<Position> FilterByBotId(this IQueryable<Position> records, long? botId)
        {
            if (botId == null) return records;

            return records.Where(x =>x.BotId == botId);
        }

        public static IQueryable<Position> FilterBySide(this IQueryable<Position> records, Enumerations.OrderSideEnum? side)
        {
            if (side == null) return records;

            return records.Where(x =>x.Side == side);
        }
    }
}
