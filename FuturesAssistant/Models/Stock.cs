using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    public class Stock
    {
        public Stock()
        {
            Id = Guid.NewGuid();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Required(ErrorMessage = "日期不可为空！")]
        public DateTime Date { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public decimal Open { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public decimal High { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public decimal Low { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public decimal Close { get; set; }

        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Volume { get; set; }

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
            return Id == ((Stock)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
