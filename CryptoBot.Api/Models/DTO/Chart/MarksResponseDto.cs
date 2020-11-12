using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class MarksResponseDto
    {
        [JsonProperty("id")]
        public int[] Ids { get; set; } = {};
        [JsonProperty("time")]
        public Int32[] TimeStamps { get; set; }
        [JsonProperty("color")]
        public string[] Colors { get; set; } = {};
        [JsonProperty("text")]
        public string[] Texts { get; set; } = {};
        [JsonProperty("label")]
        public string[] Labels { get; set; } = {};
        [JsonProperty("labelFontColor")]
        public string[] LabelFontColors { get; set; } = {};
        [JsonProperty("minSize")]
        public int[] MinSizes { get; set; } = {};
    }
}
