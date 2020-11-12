using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace Api.CryptoBot.Models.DTO.Position
{
    public class PositionRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long BotId { get; set; }

        // buy or sell
        public Enumerations.OrderSideEnum Side { get; set; }
    }
}
