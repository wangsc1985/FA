
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    public class Wh6Trade
    {
        /// <summary>
        /// 成交日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }
        /// <summary>
        /// 品种
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 交割期
        /// </summary>
        public string DeliveryDate { get; set; }
        /// <summary>
        /// 买卖
        /// </summary>
        public string BS { get; set; }
        /// <summary>
        /// 成交价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 手数
        /// </summary>
        public int Size { get; set; }
        /// <summary>
        /// 开平
        /// </summary>
        public string OC { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 投保
        /// </summary>
        public string SH { get; set; }
        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string AccountId { get; set; }
    }

    public class Wh6Remittance
    {
        /// <summary>
        /// 发生日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 资金类型
        /// </summary>
        public string MoneyType { get; set; }
        /// <summary>
        /// 出入金类型
        /// </summary>
        public string RemittanceType { get; set; }
        /// <summary>
        /// 银行代码
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// 入金
        /// </summary>
        public decimal Deposit { get; set; }
        /// <summary>
        /// 出金
        /// </summary>
        public decimal WithDrawal { get; set; }
        /// <summary>
        /// 摘要 
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string AccountId { get; set; }
    }

    public class Wh6FundStatus
    {

        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 逐笔对冲：平仓盈亏
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 成交额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string AccountId { get; set; }

    }

    public class Wh6ClosedTradeDetail
    {
        // 成交日期 交易所     品种       交割期    序号     买卖      成交价  开仓日期      开仓价    手数       昨结算         平仓盈亏
        // 20130201 郑州       白糖       1309            0  买        5464.00 20130122      5485.00        1       5426.00       -380.00

        /// <summary>
        /// 实际成交日期（实际平仓日期）
        /// </summary>
        public DateTime ActualDate { get; set; }
        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }
        /// <summary>
        /// 品种
        /// </summary>
        public string Item { get; set; }
        /// <summary>
        /// 交割期
        /// </summary>
        public string DeliveryDate { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string Ticket { get; set; }
        /// <summary>
        /// 买/卖
        /// </summary>
        public string BS { get; set; }
        /// <summary>
        /// 成交价（平仓价）
        /// </summary>
        public decimal PriceForClose { get; set; }
        /// <summary>
        /// 开仓日期
        /// </summary>
        public DateTime DateForOpen { get; set; }
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
        /// 外键
        /// </summary>
        public string AccountId { get; set; }
    }

    public class Wh6Info
    {
        /// <summary>
        /// 客户号
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
    }
}
