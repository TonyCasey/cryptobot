using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Bot
{
    public class BotSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long Id { get; set; }
    }
}
