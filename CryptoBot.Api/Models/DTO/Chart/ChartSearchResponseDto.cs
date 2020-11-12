using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class ChartSearchResponseDto : SearchResponseBase< ChartResponseDto > // must extend SearchResponseBase
    {
        public string Description { get; set; }
    }
}
