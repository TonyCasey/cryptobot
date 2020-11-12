using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CryptoBot.Model.Domain;

namespace Api.CryptoBot.Models.DTO.Chart
{
    public class Chart : BaseEntity // must extend BaseEntity
    {
        public long Id { get; set; }
    }
}
