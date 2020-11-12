using CryptoBot.Model.Domain.Market;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.Model.Domain.Account
{
    [Table("ApiSettings", Schema = "Account")]
    public class ApiSetting
    {
        public long ApiSettingId { get; set; }
        [Required]
        public User User { get; set; }
        [Required]
        public Exchange Exchange { get; set; }
        [Required]
        public string Url { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
        public string Passphrase { get; set; }
        public double ComissionRate { get; set; }
        public string SocketUrl { get; set; }
        public bool Simulated { get; set; }
    }
}
