using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuturesAssistantWPF.Windows
{
    /// <summary>
    /// ImportCooperateData.xaml 的交互逻辑
    /// </summary>
    public partial class ImportCooperateDataDialog : DialogBase
    {
        private Guid accountId;
        private DateTime startDate, tradeDate;
        private Account account;
        List<Stock> stocks;
        MainWindow mainWindowHandler;

        private delegate void FormControlInvoker();
        private FundStatus fundStatus;
        private decimal volume = 0;
        private bool haveNew = false;

        public ImportCooperateDataDialog(Guid accountId, MainWindow handler)
            : this(accountId)
        {
            //InitializeComponent();
            mainWindowHandler = handler;
            //this.accountId = accountId;
            ////this._datePicker日期.SelectedDate = DateTime.Today;
            ////this._datePicker日期.Text = DateTime.Today.ToShortDateString();
            //Initialize(accountId);
        }
        public ImportCooperateDataDialog(Guid accountId)
        {
            InitializeComponent();
            this.accountId = accountId;
            _datePicker日期.SelectedDate = DateTime.Today;
            //this._datePicker日期.Text = DateTime.Today.ToShortDateString();
            Initialize(accountId);
        }

        private void Initialize(Guid accountId)
        {

            using (StatementContext statement = new StatementContext(typeof(Stock), typeof(Account)))
            {
                account = statement.Accounts.FirstOrDefault(m => m.Id == accountId);
                stocks = statement.Stocks.Where(m => m.AccountId == accountId).OrderBy(m => m.Date).ToList();
                var firstStock = stocks.FirstOrDefault();
                var lastStock = stocks.LastOrDefault();
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    if (firstStock != null)
                    {
                        _datePicker日期.DisplayDateStart = startDate = firstStock.Date;
                    }
                    _datePicker日期.DisplayDateEnd = DateTime.Today.AddDays(1);
                    _datePicker日期.SelectedDate = DateTime.Today.AddDays(1);
                    //if (lastStock != null && lastStock.Date.Date < DateTime.Today)
                    //{
                    //    this._datePicker日期.SelectedDate = lastStock.Date.Date.AddDays(1);
                    //    if (lastStock.Date.DayOfWeek == DayOfWeek.Friday)
                    //    {
                    //        this._datePicker日期.SelectedDate = lastStock.Date.Date.AddDays(3);
                    //    }
                    //}
                    foreach (var stock in stocks)
                    {
                        _datePicker日期.BlackoutDates.Add(new CalendarDateRange(stock.Date.Date));
                    }
                }));
            }
        }

        private void _btn结算文件_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ImportFiles(dialog.FileNames);
            }

        }

        public void ImportFiles(Array fileNames)
        {
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _btn结算文件.IsEnabled = false;
            }));
                    mainWindowHandler.ProgressInfoVisible(fileNames.Length * 7 + 1);
                    foreach (string fileName in fileNames)
                    {
                        fundStatus = null;
                        volume = 0;
                        var reader = new StreamReader(File.OpenRead(fileName.ToString()), Encoding.Default);
                        string content = reader.ReadToEnd();
                        reader.Close();
                        if (content.Contains("交易结算单"))
                        {
                            if (content.Contains("众期结算单"))
                            {
                                mainWindowHandler.SetProgressText(string.Concat("正在处理快期结算文件 ", System.IO.Path.GetFileName(fileName.ToString())));
                                parse快期(content);
                            }
                            else
                            {
                                mainWindowHandler.SetProgressText(string.Concat("正在处理博易结算文件 ", System.IO.Path.GetFileName(fileName.ToString())));
                                parse博易(content);
                            }

                            mainWindowHandler.SetProgressText("正在处理补齐数据... ");
                            using (StatementContext context = new StatementContext())
                            {
                                var lastFund = context.FundStatus.Where(m => m.Date < tradeDate && m.AccountId == accountId).OrderByDescending(m => m.Date).ToList().FirstOrDefault();
                                if (lastFund != null && fundStatus != null && lastFund.TodayBalance == fundStatus.YesterdayBalance)
                                {
                                    AutoFillNoTradeData(lastFund.Date, fundStatus.Date, false);
                                }
                            }
                        }
                        else
                        {
                            Dispatcher.Invoke(new FormControlInvoker(() =>
                            {
                                System.Windows.MessageBox.Show("无法识别的结算单数据！目前只支持快期和博易结算单数据。");
                            }));
                        }
                    }
                    if (haveNew)
                    {
                        mainWindowHandler.InitializeUserControlsThreadStart();
                    }
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }));
                }
                finally
                {
                    mainWindowHandler.ProgressInfoHidden();
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _btn结算文件.IsEnabled = true;
                    }));
                }
            })).Start();
        }

        private void parse快期(string content)
        {
            try
            {
                using (StatementContext statement = new StatementContext())
                {
                    // 信息
                    int startIndex = content.IndexOf("交易结算单");
                    int endIndex = content.IndexOf("资金状况");
                    if (Validate快期(account.CustomerName, account.AccountNumber, content.Substring(startIndex, endIndex - startIndex)))
                    {
                        haveNew = true;
                        // 资金状况
                        //mainWindowHandler.SetProgressText(string.Concat("快期数据：", tradeDate.ToString("MM月dd日"), " 资金状况"));
                        startIndex = content.IndexOf("资金状况");
                        endIndex = content.IndexOf("出入金明细");
                        ParseFundStatus快期(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 出入金明细
                        //mainWindowHandler.SetProgressText(string.Concat("快期数据：", tradeDate.ToString("MM月dd日"), " 出入金明细"));
                        startIndex = content.IndexOf("出入金明细");
                        endIndex = content.IndexOf("成交明细");
                        ParseRemitance快期(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 成交记录
                        //mainWindowHandler.SetProgressText(string.Concat("快期数据：", tradeDate.ToString("MM月dd日"), " 成交明细"));
                        startIndex = content.IndexOf("成交明细");
                        endIndex = content.IndexOf("平仓明细");
                        ParseTrades快期(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 平仓明细
                        //mainWindowHandler.SetProgressText(string.Concat("快期数据：", tradeDate.ToString("MM月dd日"), " 平仓明细"));
                        startIndex = content.IndexOf("平仓明细");
                        endIndex = content.IndexOf("持仓明细");
                        ParseClosedTradeDetails快期(content.Substring(startIndex, endIndex - startIndex), statement);

                        // 持仓明细
                        //mainWindowHandler.SetProgressText(string.Concat("快期数据：", tradeDate.ToString("MM月dd日"), " 持仓明细"));
                        startIndex = content.IndexOf("持仓明细");
                        ParsePositionDetails快期(content.Substring(startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);


                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 蜡烛图表"));
                        var allStocks = statement.Stocks.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                        var afterStocks = allStocks.Where(m => m.Date > tradeDate);
                        var allFundStatus = statement.FundStatus.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                        var afterFundStatus = allFundStatus.Where(m => m.Date > tradeDate);

                        decimal afterRemittance = 0;
                        if (afterFundStatus.Count() > 0)
                        {
                            foreach (var af in afterFundStatus)
                            {
                                afterRemittance += af.Remittance;
                            }
                        }

                        var profit = fundStatus.ClosedProfit + fundStatus.FloatingProfit - fundStatus.Commission;
                        Stock stock = new Stock();
                        stock.Open = fundStatus.YesterdayBalance + afterRemittance;
                        stock.Close = stock.Open + profit;
                        stock.High = stock.Open < stock.Close ? stock.Close : stock.Open;
                        stock.Low = stock.Open > stock.Close ? stock.Close : stock.Open;
                        stock.Date = tradeDate;
                        stock.Volume = volume;
                        stock.AccountId = accountId;



                        //
                        statement.AddStock(stock);

                        //
                        //if (afterStocks.Count() != 0 && stock.Close != stock.Open)
                        //{
                        //    foreach (var st in afterStocks)
                        //    {
                        //        st.Open += profit;
                        //        st.Close += profit;
                        //        st.High += profit;
                        //        st.Low += profit;
                        //    }
                        //    statement.UpdateStocks(afterStocks);
                        //}

                        if (fundStatus.Remittance != 0)
                        {
                            var stocks = statement.Stocks.Where(m => m.AccountId == accountId && m.Date <= tradeDate);
                            foreach (var st in stocks)
                            {
                                st.Open += fundStatus.Remittance;
                                st.Close += fundStatus.Remittance;
                                st.High += fundStatus.Remittance;
                                st.Low += fundStatus.Remittance;
                            }
                            statement.UpdateStocks(stocks);
                        }
                        mainWindowHandler.AddProgressValue(1);

                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 保存数据"));
                        statement.SaveChanged();
                        mainWindowHandler.AddProgressValue(1);

                        Initialize(accountId);
                        mainWindowHandler.ReflushStocks();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 检验：客户号是否相符，账号是否相符，此账号的此数据是否已经存在，格式是否支持
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="accountNumber"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool Validate快期(string customerName, string accountNumber, string info)
        {
            try
            {
                // 客户号
                info = info.Substring(info.IndexOf("客户号"));
                string number = info.Substring(4, info.IndexOf("客户名称") - 4).Trim();
                // 客户名称
                info = info.Substring(info.IndexOf("客户名称"));
                string name = info.Substring(5, info.IndexOf("\r") - 5).Trim();
                // 日期
                info = info.Substring(info.IndexOf("日期"));
                info = info.Substring(info.IndexOf("：") + 1).Trim();
                string[] date = info.Split('/');
                int year = Convert.ToInt32(date[0]);
                int month = Convert.ToInt32(date[1]);
                int day = Convert.ToInt32(date[2]);

                tradeDate = new DateTime(year, month, day);
                if (!customerName.Equals(name))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("客户名不匹配：当前客户名【", customerName, "】，待导入的客户名【", name, "】。"));
                    }));
                    return false;
                }
                if (!accountNumber.Equals(number))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("账号不匹配：当前账号【", accountNumber, "】，待导入的账号【", number, "】。"));
                    }));
                    return false;
                }
                if (stocks.FirstOrDefault(m => m.AccountId == accountId && m.Date.Date == tradeDate.Date) != null)
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("账号【", account.AccountNumber, "】 ", tradeDate.ToString("yyyy年MM月dd日"), " 的数据已经存在。"));
                    }));
                    return false;
                }
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    System.Windows.MessageBox.Show(string.Concat("与快期数据格式不匹配！"));
                }));
                return false;
            }
            return true;
        }
        private void ParseFundStatus快期(string str, StatementContext statement)
        {
            try
            {
                FundStatus fundStatus = new FundStatus();
                fundStatus.AccountId = accountId;
                fundStatus.Date = tradeDate;
                fundStatus.SettlementType = Types.SettlementType.date;

                str = str.Substring(str.IndexOf("期初结存"));
                string value = str.Substring(5, str.IndexOf("持仓盈亏") - 5).Trim();
                fundStatus.YesterdayBalance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("持仓盈亏"));
                value = str.Substring(5, str.IndexOf("出入金") - 5).Trim();
                fundStatus.FloatingProfit = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("出入金"));
                value = str.Substring(4, str.IndexOf("\r") - 4).Trim();
                fundStatus.Remittance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("期末结存"));
                value = str.Substring(5, str.IndexOf("平仓盈亏") - 5).Trim();
                fundStatus.TodayBalance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("平仓盈亏"));
                value = str.Substring(5, str.IndexOf("手续费") - 5).Trim();
                fundStatus.ClosedProfit = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("手续费"));
                value = str.Substring(4, str.IndexOf("\r") - 4).Trim();
                fundStatus.Commission = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("可用资金"));
                value = str.Substring(5, str.IndexOf("保证金") - 5).Trim();
                fundStatus.FreeMargin = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("保证金"));
                value = str.Substring(4, str.IndexOf("\r") - 4).Trim();
                fundStatus.Margin = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("客户权益"));
                value = str.Substring(5, str.IndexOf("可取资金") - 5).Trim();
                fundStatus.CustomerRights = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("可取资金"));
                value = str.Substring(5, str.IndexOf("风险度") - 5).Trim();
                fundStatus.FreeMargin = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("风险度"));
                value = str.Substring(4, str.IndexOf("%") - 4).Trim();
                fundStatus.VentureFactor = Double.Parse(value);


                statement.AddFundStatus(fundStatus);
                this.fundStatus = fundStatus;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParseRemitance快期(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    Remittance remittance = new Remittance();
                    try
                    {
                        remittance.AccountId = accountId;

                        string[] strs = values[i].Trim().Split('|');
                        string Date = strs[1].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(5, 2));
                        int day = Convert.ToInt32(Date.Substring(8, 2));
                        int hour = Convert.ToInt32(Date.Substring(11, 2));
                        int minite = Convert.ToInt32(Date.Substring(14, 2));
                        int second = Convert.ToInt32(Date.Substring(17, 2));

                        remittance.Date = new DateTime(year, month, day, hour, minite, second);
                        decimal money = Decimal.Parse(strs[2].Trim());
                        if (money > 0)
                            remittance.Deposit = money;
                        else
                            remittance.WithDrawal = money;
                        remittance.Summary = strs[4].Trim();
                        //
                        statement.AddRemittance(remittance);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParseTrades快期(string tradeStr, StatementContext statement)
        {
            try
            {
                string[] values = tradeStr.Split('\n');
                List<CommoditySummarization> commSums = new List<CommoditySummarization>();
                //
                for (int i = 0; i < values.Length; i++)
                {
                    Trade trade = new Trade();
                    TradeDetail tradeDetail = new TradeDetail();
                    try
                    {
                        tradeDetail.AccountId = trade.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('|');

                        string Date = strs[1].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(5, 2));
                        int day = Convert.ToInt32(Date.Substring(8, 2));
                        string time = strs[10].Trim();
                        int hour = Convert.ToInt32(time.Substring(0, 2));
                        int minite = Convert.ToInt32(time.Substring(3, 2));
                        int second = Convert.ToInt32(time.Substring(6, 2));
                        tradeDetail.ActualTime = trade.Date = new DateTime(year, month, day, hour, minite, second);
                        tradeDetail.Item = trade.Item = strs[2].Trim();
                        tradeDetail.BS = trade.BS = strs[3].Trim();
                        tradeDetail.OC = trade.OC = strs[4].Trim();
                        tradeDetail.Price = trade.Price = Decimal.Parse(strs[5].Trim());
                        tradeDetail.Size = trade.Size = Int32.Parse(strs[6].Trim());
                        tradeDetail.Amount = trade.Amount = Decimal.Parse(strs[7].Trim());
                        tradeDetail.Commission = trade.Commission = Decimal.Parse(strs[8].Trim());
                        tradeDetail.ClosedProfit = trade.ClosedProfit = Decimal.Parse(strs[9].Trim());
                        tradeDetail.Ticket = strs[11].Trim();
                        tradeDetail.SH = trade.SH = strs[12].Trim();
                        volume += trade.Amount;

                        // 品种统计
                        string commodity = trade.Item.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
                        var cs = commSums.FirstOrDefault(m => m.Commodity.ToLower().Equals(commodity.ToLower()));
                        if (cs == null)
                        {
                            cs = new CommoditySummarization();
                            cs.AccountId = accountId;
                            cs.ClosedProfit = trade.ClosedProfit;
                            cs.Commission = trade.Commission;
                            cs.Commodity = commodity;
                            cs.Date = trade.Date;
                            cs.Size = trade.Size;
                            commSums.Add(cs);
                            if (statement.Commoditys.FirstOrDefault(m => m.Code.ToLower().Equals(cs.Commodity.ToLower())) == null)
                            {
                                Commodity cd = new Commodity();
                                cd.Code = cs.Commodity;
                                cd.Name = cs.Commodity;
                                statement.AddCommodity(cd);
                            }
                        }
                        else
                        {
                            cs.ClosedProfit += trade.ClosedProfit;
                            cs.Commission += trade.Commission;
                            cs.Size += trade.Size;
                        }
                        //
                        statement.AddTrade(trade);
                        statement.AddTradeDetail(tradeDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
                statement.AddCommoditySummarizations(commSums);
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParseClosedTradeDetails快期(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    ClosedTradeDetail closedTradeDetail = new ClosedTradeDetail();
                    try
                    {
                        closedTradeDetail.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('|');

                        string Date = strs[1].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(5, 2));
                        int day = Convert.ToInt32(Date.Substring(8, 2));
                        string time = strs[2].Trim();
                        int hour = Convert.ToInt32(time.Substring(0, 2));
                        int minite = Convert.ToInt32(time.Substring(3, 2));
                        int second = Convert.ToInt32(time.Substring(6, 2));
                        closedTradeDetail.ActualDate = new DateTime(year, month, day, hour, minite, second);
                        closedTradeDetail.Item = strs[3].Trim();
                        closedTradeDetail.TicketForOpen = strs[5].Trim();
                        closedTradeDetail.BS = strs[6].Trim();
                        closedTradeDetail.Size = Int32.Parse(strs[7].Trim());
                        closedTradeDetail.PriceForOpen = Decimal.Parse(strs[8].Trim());
                        closedTradeDetail.PriceForClose = Decimal.Parse(strs[9].Trim());
                        closedTradeDetail.ClosedProfit = Decimal.Parse(strs[10].Trim());
                        closedTradeDetail.TicketForClose = strs[11].Trim();
                        //
                        statement.AddClosedTradeDetail(closedTradeDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParsePositionDetails快期(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    PositionDetail positionDetail = new PositionDetail();
                    try
                    {
                        positionDetail.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('|');


                        string Date = strs[1].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(5, 2));
                        int day = Convert.ToInt32(Date.Substring(8, 2));
                        string time = strs[2].Trim();
                        int hour = Convert.ToInt32(time.Substring(0, 2));
                        int minite = Convert.ToInt32(time.Substring(3, 2));
                        int second = Convert.ToInt32(time.Substring(6, 2));
                        positionDetail.DateForPosition = tradeDate;
                        positionDetail.DateForActual = new DateTime(year, month, day, hour, minite, second);

                        positionDetail.Item = strs[3].Trim();
                        if (strs[4].Trim().Equals("买"))
                        {
                            positionDetail.BuyPrice = Decimal.Parse(strs[6].Trim());
                            positionDetail.BuySize = Int32.Parse(strs[5].Trim());
                        }
                        else
                        {
                            positionDetail.SalePrice = Decimal.Parse(strs[6].Trim());
                            positionDetail.SaleSize = Int32.Parse(strs[5].Trim());
                        }
                        positionDetail.YesterdaySettlementPrice = Decimal.Parse(strs[7].Trim());
                        positionDetail.Profit = Decimal.Parse(strs[8].Trim());
                        positionDetail.SH = strs[11].Trim();
                        //
                        statement.AddPositionDetail(positionDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void parse博易(string content)
        {
            try
            {
                using (StatementContext statement = new StatementContext())
                {
                    // 信息
                    int startIndex = content.IndexOf("交易结算单");
                    int endIndex = content.IndexOf("资金状况");
                    if (Validate博易(account.CustomerName, account.AccountNumber, content.Substring(startIndex, endIndex - startIndex)))
                    {
                        haveNew = true;
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日")));
                        // 资金状况
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 资金状况"));
                        startIndex = content.IndexOf("资金状况");
                        endIndex = content.IndexOf("成交记录");
                        ParseFundStatus博易(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 成交记录
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 成交记录"));
                        startIndex = content.IndexOf("成交记录");
                        endIndex = content.IndexOf("平仓明细");
                        ParseTrades博易(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 平仓明细
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 平仓明细"));
                        startIndex = content.IndexOf("平仓明细");
                        endIndex = content.IndexOf("持仓明细");
                        ParseClosedTradeDetails博易(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 持仓明细
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 持仓明细"));
                        startIndex = content.IndexOf("持仓明细");
                        endIndex = content.IndexOf("持仓汇总");
                        ParsePositionDetails博易(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);

                        // 持仓汇总
                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 持仓汇总"));
                        startIndex = content.IndexOf("持仓汇总");
                        endIndex = content.IndexOf("制表日期");
                        ParsePosition博易(content.Substring(startIndex, endIndex - startIndex), statement);
                        mainWindowHandler.AddProgressValue(1);



                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 蜡烛图表"));
                        var allStocks = statement.Stocks.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                        var afterStocks = allStocks.Where(m => m.Date > tradeDate);
                        var allFundStatus = statement.FundStatus.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                        var afterFundStatus = allFundStatus.Where(m => m.Date > tradeDate);

                        decimal afterRemittance = 0;
                        if (afterFundStatus.Count() > 0)
                        {
                            foreach (var af in afterFundStatus)
                            {
                                afterRemittance += af.Remittance;
                            }
                        }

                        var profit = fundStatus.ClosedProfit + fundStatus.FloatingProfit - fundStatus.Commission;
                        Stock stock = new Stock();
                        stock.Open = fundStatus.YesterdayBalance + afterRemittance;
                        stock.Close = stock.Open + profit;
                        stock.High = stock.Open < stock.Close ? stock.Close : stock.Open;
                        stock.Low = stock.Open > stock.Close ? stock.Close : stock.Open;
                        stock.Date = tradeDate;
                        stock.Volume = volume;
                        stock.AccountId = accountId;

                        //
                        statement.AddStock(stock);


                        if (fundStatus.Remittance != 0)
                        {
                            var stocks = statement.Stocks.Where(m => m.AccountId == accountId && m.Date <= tradeDate);
                            foreach (var st in stocks)
                            {
                                st.Open += fundStatus.Remittance;
                                st.Close += fundStatus.Remittance;
                                st.High += fundStatus.Remittance;
                                st.Low += fundStatus.Remittance;
                            }
                            statement.UpdateStocks(stocks);
                        }
                        mainWindowHandler.AddProgressValue(1);

                        //mainWindowHandler.SetProgressText(string.Concat("博易数据：", tradeDate.ToString("MM月dd日"), " 保存数据"));
                        statement.SaveChanged();
                        mainWindowHandler.AddProgressValue(1);

                        Initialize(accountId);
                        mainWindowHandler.ReflushStocks();
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        /// <summary>
        /// 检验：客户号是否相符，账号是否相符，此账号的此数据是否已经存在，格式是否支持
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="accountNumber"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private bool Validate博易(string customerName, string accountNumber, string info)
        {
            try
            {
                // 客户号
                info = info.Substring(info.IndexOf("客户号"));
                string number = info.Substring(4, info.IndexOf("客户名称") - 4).Trim();
                // 客户名称
                info = info.Substring(info.IndexOf("客户名称"));
                string name = info.Substring(5, info.IndexOf("\r") - 5).Trim();
                // 日期
                info = info.Substring(info.IndexOf("日  期"));
                info = info.Substring(info.IndexOf(":"));
                string date = info.Substring(1, info.IndexOf("\r") - 1).Trim();
                int year = Convert.ToInt32(date.Substring(0, 4));
                int month = Convert.ToInt32(date.Substring(4, 2));
                int day = Convert.ToInt32(date.Substring(6, 2));

                tradeDate = new DateTime(year, month, day);
                if (!customerName.Equals(name))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("客户名不匹配：当前客户名【", customerName, "】，待导入的客户名【", name, "】。"));
                    }));
                    return false;
                }
                if (!accountNumber.Equals(number))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("账号不匹配：当前账号【", accountNumber, "】，待导入的账号【", number, "】。"));
                    }));
                    return false;
                }
                if (stocks.FirstOrDefault(m => m.AccountId == accountId && m.Date.Date == tradeDate.Date) != null)
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        System.Windows.MessageBox.Show(string.Concat("账号【", account.AccountNumber, "】 ", tradeDate.ToString("yyyy年MM月dd日"), " 的数据已经存在。"));
                    }));
                    return false;
                }
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    System.Windows.MessageBox.Show(string.Concat("与博易大师数据格式不匹配！"));
                }));
                return false;
            }
            return true;
        }
        private void ParseFundStatus博易(string str, StatementContext statement)
        {
            try
            {

                FundStatus fundStatus = new FundStatus();
                fundStatus.AccountId = accountId;
                fundStatus.Date = tradeDate;
                fundStatus.SettlementType = Types.SettlementType.date;

                str = str.Substring(str.IndexOf("上日结存"));
                string value = str.Substring(5, str.IndexOf("当日结存") - 5).Trim();
                fundStatus.YesterdayBalance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("当日结存"));
                value = str.Substring(5, str.IndexOf("可用资金") - 5).Trim();
                fundStatus.TodayBalance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("可用资金"));
                value = str.Substring(5, str.IndexOf("\r") - 5).Trim();
                fundStatus.FreeMargin = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("出入金"));
                value = str.Substring(4, str.IndexOf("客户权益") - 4).Trim();
                fundStatus.Remittance = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("客户权益"));
                value = str.Substring(5, str.IndexOf("风险度") - 5).Trim();
                fundStatus.CustomerRights = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("风险度"));
                value = str.Substring(4, str.IndexOf("%") - 4).Trim();
                fundStatus.VentureFactor = Double.Parse(value);

                str = str.Substring(str.IndexOf("手续费"));
                value = str.Substring(4, str.IndexOf("保证金占用") - 4).Trim();
                fundStatus.Commission = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("保证金占用"));
                value = str.Substring(6, str.IndexOf("\r") - 6).Trim();
                fundStatus.Margin = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("平仓盈亏"));
                value = str.Substring(5, str.IndexOf("持仓盈亏") - 5).Trim();
                fundStatus.ClosedProfit = Decimal.Parse(value);

                str = str.Substring(str.IndexOf("持仓盈亏"));
                value = str.Substring(5, str.IndexOf("总盈亏") - 5).Trim();
                fundStatus.FloatingProfit = Decimal.Parse(value);


                statement.AddFundStatus(fundStatus);
                this.fundStatus = fundStatus;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParsePosition博易(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    Position position = new Position();
                    try
                    {
                        position.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('│');
                        position.Item = strs[1].Trim();
                        position.BuySize = Int32.Parse(strs[2].Trim());
                        position.BuyAveragePrice = Decimal.Parse(strs[3].Trim());
                        position.SaleSize = Int32.Parse(strs[4].Trim());
                        position.SaleAveragePrice = Decimal.Parse(strs[5].Trim());
                        position.Date = tradeDate;
                        position.YesterdaySettlementPrice = Decimal.Parse(strs[6].Trim());
                        position.TodaySettlementPrice = Decimal.Parse(strs[7].Trim());
                        position.Profit = Decimal.Parse(strs[8].Trim());
                        position.Margin = Decimal.Parse(strs[9].Trim());
                        position.SH = strs[10].Trim();
                        //
                        statement.AddPosition(position);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParsePositionDetails博易(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    PositionDetail positionDetail = new PositionDetail();
                    try
                    {
                        positionDetail.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('│');

                        positionDetail.Item = strs[1].Trim();
                        positionDetail.SH = strs[3].Trim();
                        if (strs[2].Trim().Equals("买"))
                        {
                            positionDetail.BuyPrice = Decimal.Parse(strs[5].Trim());
                            positionDetail.BuySize = Int32.Parse(strs[4].Trim());
                        }
                        else
                        {
                            positionDetail.SalePrice = Decimal.Parse(strs[5].Trim());
                            positionDetail.SaleSize = Int32.Parse(strs[4].Trim());
                        }
                        string Date = strs[6].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(4, 2));
                        int day = Convert.ToInt32(Date.Substring(6, 2));
                        positionDetail.DateForPosition = tradeDate;
                        positionDetail.DateForActual = new DateTime(year, month, day);
                        positionDetail.YesterdaySettlementPrice = Decimal.Parse(strs[7].Trim());
                        positionDetail.TodaySettlementPrice = Decimal.Parse(strs[8].Trim());
                        positionDetail.Profit = Decimal.Parse(strs[9].Trim());
                        //
                        statement.AddPositionDetail(positionDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParseClosedTradeDetails博易(string str, StatementContext statement)
        {
            try
            {
                string[] values = str.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    ClosedTradeDetail closedTradeDetail = new ClosedTradeDetail();
                    try
                    {
                        closedTradeDetail.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('│');

                        string Date = strs[0].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(4, 2));
                        int day = Convert.ToInt32(Date.Substring(6, 2));
                        closedTradeDetail.ActualDate = new DateTime(year, month, day);
                        closedTradeDetail.Item = strs[2].Trim();
                        closedTradeDetail.BS = strs[3].Trim();
                        closedTradeDetail.PriceForClose = Decimal.Parse(strs[5].Trim());
                        closedTradeDetail.Size = Int32.Parse(strs[6].Trim());
                        closedTradeDetail.PriceForOpen = Decimal.Parse(strs[8].Trim());
                        closedTradeDetail.YesterdaySettlementPrice = Decimal.Parse(strs[9].Trim());
                        closedTradeDetail.ClosedProfit = Decimal.Parse(strs[10].Trim());
                        //
                        statement.AddClosedTradeDetail(closedTradeDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void ParseTrades博易(string tradeStr, StatementContext statement)
        {
            try
            {
                string[] values = tradeStr.Split('\n');

                List<CommoditySummarization> commSums = new List<CommoditySummarization>();
                //
                for (int i = 0; i < values.Length; i++)
                {
                    Trade trade = new Trade();
                    TradeDetail tradeDetail = new TradeDetail();
                    try
                    {
                        tradeDetail.AccountId = trade.AccountId = accountId;
                        string[] strs = values[i].Trim().Split('│');

                        string Date = strs[0].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(4, 2));
                        int day = Convert.ToInt32(Date.Substring(6, 2));
                        tradeDetail.ActualTime = trade.Date = new DateTime(year, month, day);
                        tradeDetail.Item = trade.Item = strs[2].Trim();
                        tradeDetail.BS = trade.BS = strs[3].Trim();
                        tradeDetail.OC = trade.OC = strs[4].Trim();
                        tradeDetail.SH = trade.SH = strs[5].Trim();
                        tradeDetail.Price = trade.Price = Decimal.Parse(strs[6].Trim());
                        tradeDetail.Size = trade.Size = Int32.Parse(strs[7].Trim());
                        tradeDetail.Amount = trade.Amount = Decimal.Parse(strs[8].Trim());
                        tradeDetail.Commission = trade.Commission = Decimal.Parse(strs[9].Trim());
                        tradeDetail.ClosedProfit = trade.ClosedProfit = Decimal.Parse(strs[10].Trim());
                        tradeDetail.Ticket = strs[11].Trim();
                        volume += trade.Amount;

                        // 品种统计
                        string commodity = trade.Item.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "").Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "");
                        var cs = commSums.FirstOrDefault(m => m.Commodity.ToLower().Equals(commodity.ToLower()));
                        if (cs == null)
                        {
                            cs = new CommoditySummarization();
                            cs.AccountId = accountId;
                            cs.ClosedProfit = trade.ClosedProfit;
                            cs.Commission = trade.Commission;
                            cs.Commodity = commodity;
                            cs.Date = trade.Date;
                            cs.Size = trade.Size;
                            commSums.Add(cs);
                            if (statement.Commoditys.FirstOrDefault(m => m.Code.ToLower().Equals(cs.Commodity.ToLower())) == null)
                            {
                                Commodity cd = new Commodity();
                                cd.Code = cs.Commodity;
                                cd.Name = cs.Commodity;
                                statement.AddCommodity(cd);
                            }
                        }
                        else
                        {
                            cs.ClosedProfit += trade.ClosedProfit;
                            cs.Commission += trade.Commission;
                            cs.Size += trade.Size;
                        }
                        //
                        statement.AddTrade(trade);
                        statement.AddTradeDetail(tradeDetail);
                    }
                    catch (Exception)
                    {
                    }
                }
                statement.AddCommoditySummarizations(commSums);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void _btn退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _btn自动填充数据_Click(object sender, RoutedEventArgs e)
        {
            using (StatementContext statement = new StatementContext())
            {
                DateTime date = DateTime.Today;
                DateTime endate = DateTime.Today;
                var fundStatus = statement.FundStatus.Where(m => m.AccountId == accountId).OrderBy(m => m.Date).FirstOrDefault();
                var lastFundStatus = statement.FundStatus.Where(m => m.AccountId == accountId).OrderBy(m => m.Date).LastOrDefault();
                if (fundStatus != null)
                {
                    endate = lastFundStatus.Date;
                    date = fundStatus.Date;
                    AutoFillNoTradeData(date, endate, true);
                }
            }
        }
        /// <summary>
        /// 自动配资账户添加没有交易的数据。
        /// </summary>
        private void AutoFillNoTradeData(DateTime startDate, DateTime endDate, bool isProgress)
        {
            using (StatementContext statement = new StatementContext())
            {
                DateTime date = startDate;
                DateTime endate = endDate;
                if (isProgress)
                {
                    _progress填充进度.Maximum = (endate - date).Days;
                    _progress填充进度.Value = 0;
                    _progress填充进度.Visibility = System.Windows.Visibility.Visible;
                }

                var stock = statement.Stocks.FirstOrDefault(m => m.Date == date && m.AccountId == accountId);

                while (date <= endate)
                {
                    if (statement.FundStatus.FirstOrDefault(m => m.Date == date && m.AccountId == accountId) == null
                        && statement.FundStatus.FirstOrDefault(m => m.Date == date && m.AccountId != accountId) != null)
                    {
                        FundStatus newFundStatus = new FundStatus();
                        newFundStatus.Id = Guid.NewGuid();
                        newFundStatus.AccountId = accountId;
                        newFundStatus.AdditionalMargin = 0;
                        newFundStatus.ClosedProfit = 0;
                        newFundStatus.Commission = 0;
                        newFundStatus.CustomerRights = fundStatus.CustomerRights;
                        newFundStatus.Date = date;
                        newFundStatus.FloatingProfit = 0;
                        newFundStatus.FreeMargin = fundStatus.FreeMargin;
                        newFundStatus.Margin = fundStatus.Margin;
                        newFundStatus.MatterDeposit = fundStatus.MatterDeposit;
                        newFundStatus.Remittance = 0;
                        newFundStatus.SettlementType = fundStatus.SettlementType;
                        newFundStatus.TodayBalance = fundStatus.TodayBalance;
                        newFundStatus.VentureFactor = fundStatus.VentureFactor;
                        newFundStatus.YesterdayBalance = fundStatus.TodayBalance;
                        statement.AddFundStatus(newFundStatus);

                        Stock newStock = new Stock();
                        newStock.AccountId = accountId;
                        newStock.Date = date;
                        newStock.High = stock.Close;
                        newStock.Id = Guid.NewGuid();
                        newStock.Low = stock.Close;
                        newStock.Close = stock.Close;
                        newStock.Low = stock.Close;
                        newStock.Open = stock.Close;
                        newStock.Volume = 0;
                        statement.AddStock(newStock);


                        fundStatus = newFundStatus;
                        stock = newStock;
                    }
                    else
                    {
                        if (statement.FundStatus.FirstOrDefault(m => m.Date == date && m.AccountId != accountId) != null)
                        {
                            fundStatus = statement.FundStatus.FirstOrDefault(m => m.Date == date && m.AccountId == accountId);
                            var st = statement.Stocks.FirstOrDefault(m => m.Date == date && m.AccountId == accountId);

                            if (fundStatus != null && st != null)
                            {
                                stock = statement.Stocks.FirstOrDefault(m => m.Date == date && m.AccountId == accountId);
                            }
                            else if (fundStatus != null && st == null)
                            {
                                var profit = fundStatus.ClosedProfit + fundStatus.FloatingProfit - fundStatus.Commission;
                                Stock sss = new Stock();
                                sss.Open = stock.Close;
                                sss.Close = sss.Open + profit;
                                sss.High = sss.Open < sss.Close ? sss.Close : sss.Open;
                                sss.Low = sss.Open > sss.Close ? sss.Close : sss.Open;
                                sss.Date = date;
                                sss.Volume = volume;
                                sss.AccountId = accountId;
                                statement.AddStock(sss);
                                stock = sss;
                            }
                        }
                    }
                    date = date.AddDays(1);
                    if (isProgress)
                    {
                        _progress填充进度.Value++;
                        _progress填充进度._Refresh();
                    }
                }
                statement.SaveChanged();
                if (isProgress)
                    _progress填充进度.Visibility = System.Windows.Visibility.Hidden;
            }

        }

    }
}
