using Newtonsoft.Json;
using System.Collections.Generic;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class ConfigResponseDto 
    {
        [JsonProperty("supports_search")]
        public bool SupportsSearch { get; set; }
        [JsonProperty("supports_group_request")]
        public bool SupportsGroupRequest { get; set; }
        [JsonProperty("supports_marks")]
        public bool SupportsMarks { get; set; }
        [JsonProperty("supports_timescale_marks")]
        public bool SupportsTimescaleMarks { get; set; }
        [JsonProperty("supports_time")]
        public bool SupportsTime { get; set; }
        [JsonProperty("exchanges")]
        public List<ChartExchange> Exchanges { get; set; }
        [JsonProperty("symbols_types")]
        public List<SymbolType> SymbolTypes { get; set; }
        [JsonProperty("supported_resolutions")]
        public string[] Resolutions { get; set; }
    }

    public class ChartExchange
    {
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("desc")]
        public string Description { get; set; }
    }

    public class SymbolType
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }

}
