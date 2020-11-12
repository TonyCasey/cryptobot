using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CryptoBot.Model.Domain.Account
{
    [Table("Users", Schema = "Account")]
    public class User
    {
        public long UserId { get; set; }
       
        [StringLength(250)]
        public string Name { get; set; }
        public List<ApiSetting> ApiSettings { get; set; }
        public List<MessagingApp> MessagingApps { get; set; }
    }
}