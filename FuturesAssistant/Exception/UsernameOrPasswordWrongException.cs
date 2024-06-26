using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.FAException
{
    /// <summary>
    /// 用户名或密码错误
    /// </summary>
    class UsernameOrPasswordWrongException : Exception
    {
        public UsernameOrPasswordWrongException() : base("用户名或密码错误！") { }
    }
}
