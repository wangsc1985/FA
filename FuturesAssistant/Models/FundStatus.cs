using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    /// <summary>
    /// 资金状况类
    /// </summary>
    [Serializable]
    [Table(name: "FundStatus")]
    public class FundStatus
    {
        public FundStatus()
        {
            Id = Guid.NewGuid().ToString();
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 上日结存
        /// </summary>
        public decimal YesterdayBalance { get; set; }
        /// <summary>
        /// 客户权益
        /// </summary>
        public decimal CustomerRights { get; set; }
        /// <summary>
        /// 当日存取合计
        /// </summary>
        public decimal Remittance { get; set; }
        /// <summary>
        /// 质押金
        /// </summary>
        public decimal MatterDeposit { get; set; }
        /// <summary>
        /// 逐笔对冲：平仓盈亏 / 逐日盯市：当日盈亏
        /// </summary>
        public decimal ClosedProfit { get; set; }
        /// <summary>
        /// 浮动盈亏（逐日盯市值null）
        /// </summary>
        public decimal FloatingProfit { get; set; }
        public string Increase
        {
            get
            {
                if (YesterdayBalance == 0)
                {
                    return "";
                }
                else
                {
                    decimal tmp = (ClosedProfit + FloatingProfit - Commission) / YesterdayBalance * 100;
                    if (tmp == 0)
                        return "";
                    else
                        return YesterdayBalance <= 0 ? "" : tmp.ToString("0.00") + "%";
                }
            }
        }
        public string IncreaseColor
        {
            get
            {
                if (ClosedProfit + FloatingProfit - Commission > 0)
                    return "RED";
                else if (ClosedProfit + FloatingProfit - Commission == 0)
                    return "BLACK";
                else
                    return "CYAN";
            }
        }

        /// <summary>
        /// 保证金占用
        /// </summary>
        public decimal Margin { get; set; }
        public string PositionRatio
        {
            get
            {
                if (TodayBalance == 0)
                    return "";
                else
                    return Margin / TodayBalance == 0 ? "" : (Margin / TodayBalance * 100).ToString("0.00") + "%";
            }
        }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 可用保证金
        /// </summary>
        public decimal FreeMargin { get; set; }
        /// <summary>
        /// 当日结存
        /// </summary>
        public decimal TodayBalance { get; set; }
        /// <summary>
        /// 风险度
        /// </summary>
        public double VentureFactor { get; set; }
        /// <summary>
        /// 追加保证金
        /// </summary>
        public decimal AdditionalMargin { get; set; }
        /// <summary>
        /// 结算方式（true：逐日盯市，false：逐笔对冲）
        /// </summary>
        [Required(ErrorMessage = "结算方式不能为空！")]
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
            return Id == ((FundStatus)obj).Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
