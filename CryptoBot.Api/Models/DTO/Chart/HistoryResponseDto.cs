using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class HistoryResponseDto
    {
        [JsonProperty(PropertyName = "s")]
        public string Status { get; set; }

        //// doesn't work on serialization, might be a "tab" thing?
        //[System.Runtime.Serialization.DataMember(Name="t")]
        [JsonProperty(PropertyName = "t")] 
        public long[] TimeStamps { get; set; }
        
        [JsonProperty(PropertyName = "o")]
        public decimal[] Opens { get; set; }
        [JsonProperty(PropertyName = "h")]
        public decimal[] Highs { get; set; }
        [JsonProperty(PropertyName = "l")]
        public decimal[] Lows { get; set; }
        [JsonProperty(PropertyName = "c")]
        public decimal[] Closes { get; set; }
        [JsonProperty(PropertyName = "v")]
        public decimal[] Volumes { get; set; }
    }
}
