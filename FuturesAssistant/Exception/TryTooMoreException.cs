using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.FAException
{
    /// <summary>
    /// 您错误尝试超过3次
    /// </summary>
    class TryTooMoreException : Exception
    {
        public TryTooMoreException() : base("您错误尝试超过3次！") { }
    }
}
