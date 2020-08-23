using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Types
{
    class DayPosition
    {
        public DateTime PositionDate { get; set; }

        public decimal BuyMargin { get; set; }

        public decimal SaleMargin { get; set; }

        public override bool Equals(object obj)
        {
            DayPosition dp = obj as DayPosition;
            return PositionDate == dp.PositionDate;
        }

        public override int GetHashCode()
        {
            return PositionDate.GetHashCode();
        }
    }
}
