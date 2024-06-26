using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    public class Commodity
    {
        public Commodity()
        {
            Code = Name = "";
        }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
