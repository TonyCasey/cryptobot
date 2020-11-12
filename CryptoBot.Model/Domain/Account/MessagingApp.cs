using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CryptoBot.Model.Common;

namespace CryptoBot.Model.Domain.Account
{
    [Table("MessagingApps", Schema = "Account")]
    public class MessagingApp : BaseEntity
    {
        public long MessagingAppId { get; set; }

        [Required]
        public User User { get; set; }
        [Required]
        public Enumerations.MessagingAppEnum MessagingAppType { get; set; }
        public bool Active { get; set; }
        public List<MessagingAppSettings> MessagingAppSettings { get; set; }
    }
}
