using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    [Table(name: "Trade")]
    public class Trade
    {
        public Trade()
        {
            Id = Guid.NewGuid().ToString();
            Item = BS = OC = SH = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 交易日期
        /// </summary>
        public string DateStr { 
            get { 
                if(Date>new DateTime(1900, 1, 1))
                {
                    return Date.ToShortDateString();
                }
                else
                {
                    return "合计";
                }
            } 
        }
        /// <summary>
        /// 合约
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 买/卖 Buy/Sale
        /// </summary>
        public string BS { get; set; }
        /// <summary>
        /// 开/平 Open/Close
        /// </summary>
        public string OC { get; set; }
        /// <summary>
        /// 成交价
        /// </summary>
        public decimal Price { get; set; }
        public string PriceStr
        {
            get
            {
                if (Date > new DateTime(1900, 1, 1))
                {
                    return Price.ToString("n");
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 手数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Amount { get; set; }
        public string AmountStr
        {
            get
            {
                if (Date > new DateTime(1900, 1, 1))
                {
                    return Amount.ToString("n");
                }
                else
                {
                    return "";
                }
            }
        }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 投机/套保 speculate['spekjʊleɪt]/hedging['hedʒɪŋ] 
        /// </summary>
        public string SH { get; set; }
        /// <summary>
        /// 平仓盈亏（未平仓的交易值null）
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 平仓盈亏+手续费（未平仓的交易值null）
        /// </summary>
        public decimal TotalProfit { get { return ClosedProfit - Commission; } }
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
            return Id == ((Trade)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
