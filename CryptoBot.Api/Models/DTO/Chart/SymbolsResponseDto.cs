using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class SymbolsResponseDto
    {
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("exchange-traded")]
            public string Exchangetraded { get; set; }
            [JsonProperty("exchange-listed")]
            public string Exchangelisted { get; set; }
            [JsonProperty("timezone")]
            public string Timezone { get; set; }
            [JsonProperty("minmov")]
            public int Minmov { get; set; }
            [JsonProperty("minmov2")]
            public int Minmov2 { get; set; }
            [JsonProperty("pointvalue")]
            public int Pointvalue { get; set; }
            [JsonProperty("session")]
            public string Session { get; set; }
            [JsonProperty("has_intraday")]
            public bool HasIntraday { get; set; }
            [JsonProperty("has_no_volume")]
            public bool HasNoVolume { get; set; }
            [JsonProperty("description")]
            public string Description { get; set; }
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("supported_resolutions")]
            public string[] SupportedResolutions { get; set; }
            [JsonProperty("pricescale")]
            public int Pricescale { get; set; }
            [JsonProperty("ticker")]
            public string Ticker { get; set; }

    }
}
