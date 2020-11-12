using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class ChartSearchRequestDto : SearchBase // must extend SearchBase
    {
        public long Id { get; set; }
    }
}
