using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.FAException
{
    class UserNotExistException : Exception
    {
        public UserNotExistException()
            : base("不存在此用户！")
        {
        }
    }
}
