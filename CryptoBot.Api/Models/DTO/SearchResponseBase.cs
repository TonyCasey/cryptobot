using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Api.CryptoBot.Models.DTO
{
    public abstract class SearchResponseBase<T> : SearchBase
        where T : class 
    {

        public DateTime? LastUpdateTime { get; set; }

        public string LastUpdateUser { get; set; }

        public int RecordCount { get; set; }

        public int TotalRecordCount { get; set; } // why is this required? A better structure would be IReadOnlyCollection<T>, for Data, which has a Count property

        [JsonProperty(Order = 100)]
        public IEnumerable<T> Data { get; set; }

    }
}
