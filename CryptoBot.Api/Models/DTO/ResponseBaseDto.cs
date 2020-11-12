using System;

namespace Api.CryptoBot.Models.DTO
{
    public abstract class ResponseBaseDto
    {
        public DateTime? LastUpdateTime { get; set; }

        public string LastUpdateUser { get; set; }
        
        
    }
}
