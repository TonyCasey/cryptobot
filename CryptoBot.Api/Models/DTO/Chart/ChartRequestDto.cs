using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class ChartRequestDto : RequestBaseDto // must extend RequestBaseDto
    {
        public long Id { get; set; }
        public string Description { get; set; }
    }
}
