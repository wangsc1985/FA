using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    [Table(name: "ClosedTradeDetail")]
    public class ClosedTradeDetail
    {
        public ClosedTradeDetail()
        {
            Id = Guid.NewGuid().ToString();
            TicketForClose = TicketForOpen = Item = BS = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 实际成交日期（实际平仓日期）
        /// </summary>
        public DateTime ActualDate { get; set; }
        /// <summary>
        /// 合约
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 成交序号（平仓成交序号）
        /// </summary>
        public string TicketForClose { get; set; }
        /// <summary>
        /// 买/卖
        /// </summary>
        public string BS { get; set; }
        /// <summary>
        /// 成交价（平仓价）
        /// </summary>
        public decimal PriceForClose { get; set; }
        /// <summary>
        /// 开仓价
        /// </summary>
        public decimal PriceForOpen { get; set; }
        /// <summary>
        /// 手数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 昨日结算价
        /// </summary>
        public decimal YesterdaySettlementPrice { get; set; }
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 原成交序号（开仓时的成交序号）
        /// </summary>
        public string TicketForOpen { get; set; }
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
            return Id == ((ClosedTradeDetail)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
