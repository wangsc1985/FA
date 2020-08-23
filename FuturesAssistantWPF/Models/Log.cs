using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Models
{
    public class Log
    {
        public Log()
        {
            Id = Guid.NewGuid();
        }
        public Guid Id { get; set; }

        public DateTime Timestamp { get; set; }

        public string Message { get; set; }

    }
}
