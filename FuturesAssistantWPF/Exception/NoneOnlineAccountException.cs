using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.FAException
{
    /// <summary>
    /// 用户名或密码错误
    /// </summary>
    class NoneOnlineAccountException : Exception
    {
        public NoneOnlineAccountException() : base("当前账户未登陆！") { }
    }
}
