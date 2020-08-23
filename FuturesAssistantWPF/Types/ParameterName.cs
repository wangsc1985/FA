using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuturesAssistantWPF.Types
{
    public enum ParameterName
    {
        StockChart图表开始日期,
        StockChart风格,
        StockChart周期,
        StockChart复权,
        StockCount,

        Statistics风格,
        
        Average1Value,
        Average2Value,
        Average3Value,
        Average1Color,
        Average2Color,
        Average3Color,
        StockChart显示均线,
        Average1Visibility,
        Average2Visibility,
        Average3Visibility,

        LoginImage,
        /// <summary>
        /// 登录界面Image
        /// </summary>
        DefaultAccountId,
        /// <summary>
        /// 记住密码
        /// </summary>
        RememberUserPassword,
        /// <summary>
        /// 自动登录
        /// </summary>
        AutoLogin,
        /// <summary>
        /// 蜡烛图界面是否合并所有账户显示
        /// </summary>
        IsMergingAccount,

        StatisticsQueryCycle,
        StatementQueryCycle,
        /// <summary>
        /// 手续费相对交易所几倍
        /// </summary>
        CommissionRation
    }
}
