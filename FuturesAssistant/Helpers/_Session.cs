using FuturesAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FuturesAssistant.Helpers
{
    public static class _Session
    {
        /**
         * 测试版本设置
         * 
         * 1、设置合法用户。
         * 2、设置试用开始时间。
         * 3、设置标志置为true
         * 
         * 正式版本设置
         * 
         * 1、设置合法用户。
         * 2、设置标志置为false
         * 
         */
        public static string[] CUSTOMER_NAME = { "王世超", "王世起" };

        public static bool IsT = false;
        public static DateTime testStartDate = new DateTime(2017, 10, 27, 14, 0, 0);










        public static string BUTTON_TEXT_设置为允许下载 = "设置为允许下载";
        public static string BUTTON_TEXT_设置为禁止下载 = "设置为禁止下载";

        public static string DatabaseDirPath = String.Concat(System.Windows.Forms.Application.StartupPath, @"\data\");

        public static string DatabaseFilePath = String.Concat(DatabaseDirPath, "statements.db");

        public static Object obj = new Object();

        static _Session()
        {
            Logs = new List<Log>();
        }

        public static List<Log> Logs { get; set; }

        /// <summary>
        /// 当前会话中正在处理的Account的Id，如果在账户列表选择的是“合并账户”选项，值为Guid.Empty
        /// </summary>
        public static string SelectedAccountId { get; set; }

        /// <summary>
        /// 当前会话中的User
        /// </summary>
        public static string LoginedUserId { get; set; }

        /// <summary>
        /// 当前会话中用户的所有Account信息
        /// </summary>
        //public static List<Account> AccountListOfCurrentUser { get; set; }

        /// <summary>
        /// 数据已更新至日期
        /// </summary>
        public static DateTime LatestFundStatusDate
        {
            get
            {
                using (StatementContext statement = new StatementContext())
                {
                    var fun = statement.FundStatus.Where(m => m.AccountId == _Session.SelectedAccountId).OrderBy(m => m.Date).ToList().LastOrDefault();
                    if (fun != null)
                    {
                        return fun.Date;
                    }
                    else
                    {
                        return DateTime.Now;
                    }
                }
            }
        }
        /// <summary>
        /// 标志是否需要刷新图表
        /// </summary>
        public static bool NeedRefreshStockControl { get; set; }
        public static bool NeedRefreshStatisticsControl { get; set; }
        public static bool NeedRefreshStatementControl { get; set; }
        public static bool NeedRefreshMarginControl { get; set; }

    }
}
