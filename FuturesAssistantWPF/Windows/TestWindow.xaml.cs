using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;

namespace FuturesAssistantWPF.Windows
{
    public class Movie
    {
        public DateTime Date { set; get; }
        public string Title { get; set; }
    }
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : System.Windows.Window
    {
        private delegate void FormControlInvoker();
        public TestWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ParseTxtStatement();
        }
        public void ParseTxtStatement() {
            var files = Directory.GetFiles("e:\\80900");
            foreach (var file in files) {
                Console.WriteLine(file);
                var reader = new StreamReader(file, Encoding.GetEncoding("gb2312"));
                //var ff = File.OpenText(file);
                var text = reader.ReadToEnd();
                //Console.WriteLine(content);

                /**
                 * 
                 * 
                 * 制表时间 Creation Date：20200908
                   资金状况  币种：人民币  Account Summary  Currency：CNY 
----------------------------------------------------------------------------------------------------
期初结存 Balance b/f：                      780.04  基础保证金 Initial Margin：                20.00
出 入 金 Deposit/Withdrawal：             19000.00  期末结存 Balance c/f：                  19871.99
平仓盈亏 Realized P/L：                       0.00  质 押 金 Pledge Amount：                    0.00
持仓盯市盈亏 MTM P/L：                      100.00  客户权益 Client Equity：：              19871.99
期权执行盈亏 Exercise P/L：                   0.00  货币质押保证金占用 FX Pledge Occ.：         0.00
手 续 费 Commission：                         8.05  保证金占用 Margin Occupied：            18620.00
行权手续费 Exercise Fee：                     0.00  交割保证金 Delivery Margin：                0.00
交割手续费 Delivery Fee：                     0.00  多头期权市值 Market value(long)：           0.00
货币质入 New FX Pledge：                      0.00  空头期权市值 Market value(short)：          0.00
货币质出 FX Redemption：                      0.00  市值权益 Market value(equity)：         19871.99
质押变化金额 Chg in Pledge Amt：              0.00  可用资金 Fund Avail.：                   1251.99
权利金收入 Premium received：                 0.00  风 险 度 Risk Degree：                    93.70%
权利金支出 Premium paid：                     0.00  应追加资金 Margin Call：                    0.00
货币质押变化金额 Chg in FX Pledge:            0.00


                Date                
                     SettlementType  AccountId
                 * 
                 */

                var date = Regex.Match(text, "(?<=制表时间 Creation Date：).*").Value.Trim();

                var start = text.IndexOf("资金状况");
                var end = text.IndexOf("货币质押变化金额");
                var content = text.Substring(start, end - start).Replace("\r\n", " ");
                var YesterdayBalance = Regex.Match(content, "(?<=期初结存 Balance b/f：).*(?=基础保证金 Initial Margin：)").Value.Trim();
                var CustomerRights = Regex.Match(content, "(?<=客户权益 Client Equity：：).*(?=期权执行盈亏 Exercise P/L：)").Value.Trim();
                var Remittance = Regex.Match(content, "(?<=出 入 金 Deposit/Withdrawal：).*(?=期末结存 Balance c/f：)").Value.Trim();
                var MatterDeposit = Regex.Match(content, "(?<=质 押 金 Pledge Amount：).*(?=持仓盯市盈亏 MTM P/L：)").Value.Trim();
                var ClosedProfit = Regex.Match(content, "(?<=平仓盈亏 Realized P/L：).*(?=质 押 金 Pledge Amount：)").Value.Trim();
                var FloatingProfit = Regex.Match(content, "(?<=持仓盯市盈亏 MTM P/L：).*(?=客户权益 Client Equity：：)").Value.Trim();
                // 涨幅 没有
                var Margin = Regex.Match(content, "(?<=保证金占用 Margin Occupied：).*(?=行权手续费 Exercise Fee：)").Value.Trim();
                // 持仓比例 没有
                var Commission = Regex.Match(content, "(?<=手 续 费 Commission：).*(?=保证金占用 Margin Occupied：)").Value.Trim();
                var FreeMargin = Regex.Match(content, "(?<=可用资金 Fund Avail.：).*(?=权利金收入 Premium received：)").Value.Trim();
                var TodayBalance = Regex.Match(content, "(?<=期末结存 Balance c/f：).*(?=平仓盈亏 Realized P/L：)").Value.Trim();
                var VentureFactor = Regex.Match(content, "(?<=风 险 度 Risk Degree：).*(?=权利金支出 Premium paid：)").Value.Trim();
                var AdditionalMargin = Regex.Match(content, "(?<=应追加资金 Margin Call：).*(?=货币质押变化金额 Chg in FX Pledge)").Value.Trim();

                //Console.WriteLine(content);
                //Console.Write("日期：");
                //Console.WriteLine(Date);
                //Console.Write("上日结存：");
                //Console.WriteLine(YesterdayBalance);
                //Console.Write("客户权益：");
                //Console.WriteLine(CustomerRights);
                //Console.Write("当日存取合计：");
                //Console.WriteLine(Remittance);
                //Console.Write("质押金：");
                //Console.WriteLine(MatterDeposit);
                //Console.Write("当日盈亏：");
                //Console.WriteLine(ClosedProfit);
                //Console.Write("浮动盈亏：");
                //Console.WriteLine(FloatingProfit);
                //Console.Write("保证金占用：");
                //Console.WriteLine(Margin);
                //Console.Write("当日手续费：");
                //Console.WriteLine(Commission);
                //Console.Write("可用资金：");
                //Console.WriteLine(FreeMargin);
                //Console.Write("当日结存：");
                //Console.WriteLine(TodayBalance);
                //Console.Write("风险度：");
                //Console.WriteLine(VentureFactor);
                //Console.Write("追加保证金：");
                //Console.WriteLine(AdditionalMargin);


                /* 日期
                 * 入金
                 * 出金
                 * 方式
                 * 摘要
                                                          出入金明细 Deposit/Withdrawal 
----------------------------------------------------------------------------------------------------------------
|发生日期|       出入金类型       |      入金      |      出金      |                   说明                   |
|  Date  |          Type          |    Deposit     |   Withdrawal   |                   Note                   |
----------------------------------------------------------------------------------------------------------------
|20200923|银期转账                |        19000.00|            0.00|银期转账自动凭证(入金)19000               |
----------------------------------------------------------------------------------------------------------------
|共   1条|                        |        19000.00|            0.00|                                          |
----------------------------------------------------------------------------------------------------------------
出入金---Deposit/Withdrawal     银期转账---Bank-Futures Transfer    银期换汇---Bank-Futures FX Exchange

                 */

                start = text.IndexOf("出入金明细");
                end = text.IndexOf("银期换汇");
                if (start > 0) {
                    content = text.Substring(start, end - start);
                    var lines = content.Split(new char[] { '\r', '\n' });
                    foreach (var ll in lines) {
                        if (ll.Contains("银期转账自动凭证")) {
                            var values = ll.Split('|');
                            var date1 = values[1];
                            var type = values[2];
                            var deposite = values[3];
                            var withdrawal = values[4];
                            var note = values[5];

                            //Console.Write("日期：");
                            //Console.WriteLine(date);
                            //Console.Write("入金：");
                            //Console.WriteLine(deposite);
                            //Console.Write("出金：");
                            //Console.WriteLine(withdrawal);
                            //Console.Write("方式：");
                            //Console.WriteLine(type);
                            //Console.Write("摘要：");
                            //Console.WriteLine(note);
                        }
                    }
                }




                /* 成交汇总
                * 日期
                * 合约
                * 买卖
                * 投机套保
                * 成交价
                * 手数
                * 成交额
                * 开平
                * 手续费
                * 平仓盈亏
                * 


                * 
                */

                /* 持仓汇总
                 * 
                * 持仓日期
                * 合约
                * 买持仓
                * 卖持仓
                * 买均价
                * 卖均价
                * 昨结算价
                * 今结算价
                * 持仓盈亏
                * 交易保证金
                * 投机套保
                * 
                                                                         持仓汇总 Positions
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|       品种       |      合约      |    买持     |    买均价   |     卖持     |    卖均价    |  昨结算  |  今结算  |持仓盯市盈亏|  保证金占用   |  投/保     |   多头期权市值   |   空头期权市值    |
|      Product     |   Instrument   |  Long Pos.  |Avg Buy Price|  Short Pos.  |Avg Sell Price|Prev. Sttl|Sttl Today|  MTM P/L   |Margin Occupied|    S/H     |Market Value(Long)|Market Value(Short)|
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|     聚氯乙烯     |     v2101      |            0|        0.000|             4|      6655.000|  6670.000|  6650.000|      100.00|       18620.00|投机        |              0.00|               0.00|
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|共     1条        |                |            0|             |             4|              |          |          |      100.00|       18620.00|            |              0.00|               0.00|
------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                * 
                */

                start = text.IndexOf("持仓汇总");
                end = text.IndexOf("尊敬的客户");
                if (start > 0) {
                    content = text.Substring(start, end - start);
                    var lines = content.Split(new char[] { '\r', '\n' });
                    foreach (var ll in lines) {
                        if (ll.Contains("投机")) {
                            var values = ll.Split('|');

                            var date1 = date;
                            var item = values[2];
                            var buy = values[3];
                            var buyPrice = values[4];
                            var sale = values[5];
                            var salePrice = values[6];
                            var yes = values[7];
                            var today = values[8];
                            var profit = values[9];
                            var margin = values[10];
                            var type = "投机";


                            //Console.Write("持仓日期：");
                            //Console.WriteLine(date1);
                            //Console.Write("合约：");
                            //Console.WriteLine(item);
                            //Console.Write("买持仓：");
                            //Console.WriteLine(buy);
                            //Console.Write("卖持仓：");
                            //Console.WriteLine(sale);
                            //Console.Write("买均价：");
                            //Console.WriteLine(buyPrice);
                            //Console.Write("卖均价：");
                            //Console.WriteLine(salePrice);
                            //Console.Write("昨结算价：");
                            //Console.WriteLine(yes);
                            //Console.Write("今结算价：");
                            //Console.WriteLine(today);
                            //Console.Write("持仓盈亏：");
                            //Console.WriteLine(profit);
                            //Console.Write("交易保证金：");
                            //Console.WriteLine(margin);
                            //Console.Write("投机套保：");
                            //Console.WriteLine(type);
                        }
                    }
                }
                /* 成交明细
                 * 
                * 实际成交时间
                * 合约
                * 成交序号
                * 买卖
                * 投机套保
                * 成交价
                * 手数
                * 成交额
                * 开平
                * 手续费
                * 平仓盈亏
                * 
                * 
                                                                           成交记录 Transaction Record 
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|成交日期| 交易所 |       品种       |      合约      |买/卖|   投/保    |  成交价  | 手数 |   成交额   |       开平       |  手续费  |  平仓盈亏  |     权利金收支      |  成交序号  |  成交类型  |
|  Date  |Exchange|     Product      |   Instrument   | B/S |    S/H     |   Price  | Lots |  Turnover  |       O/C        |   Fee    |Realized P/L|Premium Received/Paid|  Trans.No. | trade type |
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|20200923|大商所  |聚氯乙烯          |     v2101      |   卖|投机        |  6655.000|     4|   133100.00|开                |      8.05|        0.00|                 0.00|60521343    |  普通成交  |
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|共   1条|        |                  |                      |            |          |     4|   133100.00|                  |      8.05|        0.00|                 0.00|            |            |
----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
能源中心---INE  上期所---SHFE   中金所---CFFEX  大商所---DCE   郑商所---CZCE
买---Buy   卖---Sell
投机---Speculation  套保---Hedge  套利---Arbitrage  一般---General  交易---Trade  做市商---Market Maker
开---Open 平---Close 平今---Close Today 强平---Forced Liquidation 平昨---Close Prev. 强减---Forced Reduction 本地强平---Local Forced Liquidation 
                */

                start = text.IndexOf("成交记录");
                end = text.IndexOf("能源中心");
                if (start > 0) {
                    content = text.Substring(start, end - start);
                    var lines = content.Split(new char[] { '\r', '\n' });
                    foreach (var ll in lines) {
                        if (ll.Contains("投机")) {
                            var values = ll.Split('|');

                            var date1 = values[1];
                            var item = values[4];
                            var no = values[14];
                            var tradeType = values[5];
                            var type = values[6];
                            var price = values[7];
                            var amount = values[8];
                            var turnover = values[9];
                            var oc = values[10];
                            var comm = values[11];
                            var profit = values[12];

                            Console.Write("实际成交时间：");
                            Console.WriteLine(date1);
                            Console.Write("合约：");
                            Console.WriteLine(item);
                            Console.Write("成交序号：");
                            Console.WriteLine(no);
                            Console.Write("买卖：");
                            Console.WriteLine(tradeType);
                            Console.Write("投机套保：");
                            Console.WriteLine(type);
                            Console.Write("成交价：");
                            Console.WriteLine(price);
                            Console.Write("手数：");
                            Console.WriteLine(amount);
                            Console.Write("成交额：");
                            Console.WriteLine(turnover);
                            Console.Write("开平：");
                            Console.WriteLine(oc);
                            Console.Write("手续费：");
                            Console.WriteLine(comm);
                            Console.Write("平仓盈亏：");
                            Console.WriteLine(profit);
                        }
                    }
                }

                /* 平仓明细
                * 实际成交日期
                * 合约
                * 成交序号
                * 买卖
                * 成交价
                * 开仓价
                * 手数
                * 昨结算价
                * 平仓盈亏
                * 原成交序号
                */



                /* 
                * 持仓明细
                * 
                * 持仓日期
                * 成交日期
                * 合约
                * 成交序号
                * 买持仓
                * 买入价
                * 卖持仓
                * 卖出价
                * 昨结算价
                * 今结算价
                * 持仓盈亏
                * 投机套保
                * 交易编码
                * 
                * 
                                              持仓明细 Positions Detail
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
| 交易所 |       品种       |      合约      |开仓日期 |   投/保    |买/卖|持仓量 |    开仓价     |     昨结算     |     结算价     |  浮动盈亏  |  盯市盈亏 |  保证金   |       期权市值       |
|Exchange|     Product      |   Instrument   |Open Date|    S/H     | B/S |Positon|Pos. Open Price|   Prev. Sttl   |Settlement Price| Accum. P/L |  MTM P/L  |  Margin   | Market Value(Options)|
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
| 大商所 |     聚氯乙烯     |     v2101      | 20200923|投机        |   卖|      4|       6655.000|        6670.000|        6650.000|      100.00|     100.00|   18620.00|                  0.00|
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
|共   1条|                  |                |         |            |     |      4|               |                |                |      100.00|     100.00|   18620.00|                  0.00|
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
能源中心---INE  上期所---SHFE   中金所---CFFEX  大商所---DCE   郑商所---CZCE
买---Buy   卖---Sell  
投机---Speculation  套保---Hedge  套利---Arbitrage  一般---General  交易---Trade  做市商---Market Maker
                * 
                * 
                */

                start = text.IndexOf("持仓明细");
                if (start > 0) {
                    var aaa = text.Substring(start);
                    end = aaa.IndexOf("能源中心");
                    content = aaa.Substring(0, end);

                    var lines = content.Split(new char[] { '\r', '\n' });
                    foreach (var ll in lines) {
                        if (ll.Contains("投机")) {
                            var values = ll.Split('|');

                            var date1 = date;
                            var openDate = values[4];
                            var item = values[3];
                            var no = "";
                            var bs = values[6];


                            var buy = "";
                            var buyPrice = "";
                            var sale = "";
                            var salePrice = "";
                            if (bs.Equals("买")) {

                                buy = values[7];
                                buyPrice = values[8];
                            } else {
                                sale = values[7];
                                salePrice = values[8];
                            }

                            var yes = values[9];
                            var today = values[10];
                            var profit = values[11];
                            var type = "投机";
                            var bianma = "";
                        }
                    }


                }
                /* 
                * 品种汇总
                * 日期
                * 品种
                * 手数
                * 成交额
                * 手续费
                * 平仓盈亏
                */

            }
        }
    }
}