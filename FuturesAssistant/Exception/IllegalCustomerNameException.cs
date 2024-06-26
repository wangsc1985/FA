using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Helpers
{
    class IllegalCustomerNameException : Exception
    {
        public IllegalCustomerNameException()
            : base("用户不合法！")
        {

        }
    }
}
