using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Types
{
    class TradeInterest
    {
        public string Commodity { get; set; }

        public decimal Amount { get; set; }

        public override bool Equals(object obj)
        {
            TradeInterest ti = obj as TradeInterest;
            return Commodity.Equals(ti.Commodity);
        }

        public override int GetHashCode()
        {
            return Commodity.GetHashCode();
        }
    }
}
