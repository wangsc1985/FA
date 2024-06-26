using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    public class Remittance
    {
        public Remittance()
        {
            Id = Guid.NewGuid();
            Type = Summary = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 入金
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 出金
        /// </summary>
        public decimal WithDrawal { get; set; }
        /// <summary>
        /// 方式
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 摘要 
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        [Required(ErrorMessage = "账户外键不能为空！")]
        public Guid AccountId { get; set; }
        /// <summary>
        /// 所属账号
        /// </summary>
        public Account Account { get; set; }

        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return Id == ((Remittance)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
