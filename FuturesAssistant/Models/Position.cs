using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    [Table(name: "Position")]
    public class Position
    {
        public Position()
        {
            Id = Guid.NewGuid().ToString();
            Item = SH = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 持仓日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 合约
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 买持仓
        /// </summary>
        public int BuySize { get; set; }
        /// <summary>
        /// 买均价
        /// </summary>
        public decimal BuyAveragePrice { get; set; }
        /// <summary>
        /// 卖持仓
        /// </summary>
        public int SaleSize { get; set; }
        /// <summary>
        /// 卖均价
        /// </summary>
        public decimal SaleAveragePrice { get; set; }
        /// <summary>
        /// 昨结算价
        /// </summary>
        public decimal YesterdaySettlementPrice { get; set; }
        /// <summary>
        /// 今结算价
        /// </summary>
        public decimal TodaySettlementPrice { get; set; }
        /// <summary>
        ///  持仓盈亏（浮动盈亏）
        /// </summary>
        public decimal Profit { get; set; }
        /// <summary>
        /// 交易保证金
        /// </summary>
        public decimal Margin { get; set; }
        /// <summary>
        /// 投机/套保  Speculate/Hedging
        /// </summary>
        public string SH { get; set; }
        /// <summary>
        /// 结算方式（true：逐日盯市，false：逐笔对冲）
        /// </summary>
        [Required(ErrorMessage="结算方式不能为空！")]
        public FuturesAssistant.Types.SettlementType SettlementType { get; set; }
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
            return Id == ((Position)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
