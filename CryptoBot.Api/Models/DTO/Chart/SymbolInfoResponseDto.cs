using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class SymbolInfoResponseDto
    {
    
        [JsonProperty("symbol")]
        public string[] Symbol { get; set; }
        [JsonProperty("description")]
        public string[] Description { get; set; }
        [JsonProperty("exchange-listed")]
        public string Exchangelisted { get; set; }
        [JsonProperty("exchange-traded")]
        public string Exchangetraded { get; set; }
        [JsonProperty("minmovement")]
        public int Minmovement { get; set; }
        [JsonProperty("minmovement2")]
        public int Minmovement2 { get; set; }
        [JsonProperty("pricescale")]
        public int[] Pricescale { get; set; }
        [JsonProperty("has-dwm")]
        public bool Hasdwm { get; set; }
        [JsonProperty("has-intraday")]
        public bool Hasintraday { get; set; }
        [JsonProperty("has-no-volume")]
        public bool[] Hasnovolume { get; set; }
        [JsonProperty("type")]
        public string[] Type { get; set; }
        [JsonProperty("ticker")]
        public string[] Ticker { get; set; }
        [JsonProperty("timezone")]
        public string Timezone { get; set; }
        [JsonProperty("session-regular")]
        public string Sessionregular { get; set; }
    }


}
