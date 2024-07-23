using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    [Table(name: "CommoditySummarization")]
    public class CommoditySummarization
    {
        public CommoditySummarization()
        {
            Id = Guid.NewGuid().ToString();
            Commodity = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 品种
        /// </summary>
        public string Commodity { get; set; }
        /// <summary>
        /// 手数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        [Required(ErrorMessage = "账户外键不能为空！")]
        public string AccountId { get; set; }
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
            return Id == ((CommoditySummarization)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
