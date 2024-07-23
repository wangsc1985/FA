using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Table(name: "Commodity")]
    public class Commodity
    {
        public Commodity()
        {
            Code = Name = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
