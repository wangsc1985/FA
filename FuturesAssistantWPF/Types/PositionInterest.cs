using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Types
{
    class PositionInterest
    {
        public string Commodity { get; set; }

        public int Night { get; set; }

        public override bool Equals(object obj)
        {
            PositionInterest pi = obj as PositionInterest;
            return Commodity.Equals(pi.Commodity);
        }

        public override int GetHashCode()
        {
            return Commodity.GetHashCode();
        }
    }
}
