using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.FAException
{
    /// <summary>
    /// 验证码错误
    /// </summary>
    class IdentifyingCodeMismatchException : Exception
    {
        public IdentifyingCodeMismatchException()
            : base("验证码错误，请重新登录！")
        {
            
        }
    }
}
