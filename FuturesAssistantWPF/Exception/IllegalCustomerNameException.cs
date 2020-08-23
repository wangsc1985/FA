using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Helpers
{
    class IllegalCustomerNameException : Exception
    {
        public IllegalCustomerNameException()
            : base("用户不合法！")
        {

        }
    }
}
