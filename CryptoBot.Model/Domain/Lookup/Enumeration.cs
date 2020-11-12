using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoBot.Model.Domain.Lookup
{
    [Table("Enumerations", Schema = "Lookup")]
    public class Enumeration
    {
        public int EnumerationId { get; set; }
        public string Group { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
