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
    [Table("MessagingAppSettings", Schema = "Account")]
    public class MessagingAppSettings
    {
        public long MessagingAppSettingsId { get; set; }

        [Required]
        public MessagingApp MessagingApp { get; set; }
        
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
