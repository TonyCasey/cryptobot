using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;
using CryptoBot.Model.Domain.Trading;

namespace Api.CryptoBot.Models.Extensions
{
    public static class OrderExtensions
    {
        public static IQueryable<Order> FilterByBotId(this IQueryable<Order> records, long? Id)
        {
            if (Id == null) return records;

            return records.Where(x =>x.BotId == Id);
        }

        public static IQueryable<Order> FilterByPositionId(this IQueryable<Order> records, long? Id)
        {
            if (Id == null) return records;

            return records.Where(x =>x.BotId == Id);
        }

        public static IQueryable<Order> FilterBySide(this IQueryable<Order> records, Enumerations.OrderSideEnum? side)
        {
            if (side == null) return records;

            return records.Where(x =>x.Side == side);
        }
    }
}
