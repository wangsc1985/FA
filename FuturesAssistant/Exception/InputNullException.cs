using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.FAException
{
    /// <summary>
    /// 输入信息不能为空
    /// </summary>
    class InputNullException : Exception
    {
        public InputNullException() : base("输入信息不能为空！") { }
    }
}
