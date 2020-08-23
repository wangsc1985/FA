using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Controls.Types
{
    public class ClosedTradeStatistics
    {
        public string Item { get; set; }
        public int Size { get; set; }
        public decimal Profit { get; set; }
        public decimal Loss { get; set; }
        public decimal Total { get; set; }
    }
}
