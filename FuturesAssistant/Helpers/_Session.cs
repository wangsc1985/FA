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

        public static Dictionary<string, string> Commoditys;


        static _Session()
        {
            Logs = new List<Log>();
            Commoditys = new Dictionary<string, string>();
            Commoditys.Add("tf", "五债");
            Commoditys.Add("t", "十债");
            Commoditys.Add("au", "金");
            Commoditys.Add("ag", "银");
            Commoditys.Add("cu", "铜");
            Commoditys.Add("al", "铝");
            Commoditys.Add("zn", "锌");
            Commoditys.Add("pb", "铅");
            Commoditys.Add("ni", "镍");
            Commoditys.Add("sn", "锡");
            Commoditys.Add("j", "焦炭");
            Commoditys.Add("jm", "焦煤");
            Commoditys.Add("zc", "郑煤");
            Commoditys.Add("i", "铁矿");
            Commoditys.Add("rb", "螺纹");
            Commoditys.Add("hc", "热卷");
            Commoditys.Add("sf", "硅铁");
            Commoditys.Add("sm", "锰硅");
            Commoditys.Add("fg", "玻璃");
            Commoditys.Add("fu", "燃油");
            Commoditys.Add("ru", "橡胶");
            Commoditys.Add("l", "塑料");
            Commoditys.Add("ta", "PTA");
            Commoditys.Add("v", "PVC");
            Commoditys.Add("pp", "PP");
            Commoditys.Add("ma", "郑醇");
            Commoditys.Add("bu", "沥青");
            Commoditys.Add("a", "豆一");
            Commoditys.Add("c", "玉米");
            Commoditys.Add("wh", "郑麦");
            Commoditys.Add("ri", "早稻");
            Commoditys.Add("lr", "晚稻");
            Commoditys.Add("jr", "粳稻");
            Commoditys.Add("rs", "菜籽");
            Commoditys.Add("ml", "豆粕");
            Commoditys.Add("rm", "菜粕");
            Commoditys.Add("y", "豆油");
            Commoditys.Add("oi", "郑油");
            Commoditys.Add("p", "棕榈");
            Commoditys.Add("cf", "郑棉");
            Commoditys.Add("sr", "白糖");
            Commoditys.Add("cy", "棉纱");
            Commoditys.Add("jd", "鸡蛋");
            Commoditys.Add("cs", "淀粉");
            Commoditys.Add("fb", "纤板");
            Commoditys.Add("bb", "胶板");
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
