using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.CryptoBot.Models.DTO.Chart.Entities;
using Newtonsoft.Json;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class SymbolSearchResponseDto 
    {
        [JsonProperty("symbol")]
        public new string Name { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("exchange")]
        public string Exchangetraded { get; set; }

        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("type")] // "stock" | "futures" | "bitcoin" | "forex" | "index"
        public string Type { get; set; }
        
    }
}