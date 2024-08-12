using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Types
{
    class CommodityProfit
    {
        public string Commodity { get; set; }

        public decimal Profit { get; set; }

        public decimal Commission { get; set; }

        public override bool Equals(object obj)
        {
            CommodityProfit cp = obj as CommodityProfit;
            return Commodity.Equals(cp.Commodity);
        }

        public override int GetHashCode()
        {
            return Commodity.GetHashCode();
        }
    }
}
