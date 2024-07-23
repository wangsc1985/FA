using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    //public class Account : System.Collections.ObjectModel.ObservableCollection<Account>
    [Serializable]
    [Table(name: "Account")]
    public class Account
    {
        public Account()
        {
            Id = Guid.NewGuid().ToString();
            AccountNumber = Password = CustomerName = FuturesCompanyName = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 账户类型：
        /// 1 - 交易账户
        /// 10 - 废弃的交易账户
        /// 2 - 融资账户
        /// 20 - 废弃的融资账户
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 是否允许下载数据
        /// </summary>
        public bool IsAllowLoad { get; set; }
        /// <summary>
        /// 资金账号
        /// </summary>
        public string AccountNumber { get; set; }
        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 期货公司名称
        /// </summary>
        public string FuturesCompanyName { get; set; }

        [Required]
        public string UserId { get; set; }

        public User User { get; set; }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return AccountNumber.Equals(((Account)obj).AccountNumber);
        }

        public override int GetHashCode()
        {
            return AccountNumber.GetHashCode();
        }
    }

}
