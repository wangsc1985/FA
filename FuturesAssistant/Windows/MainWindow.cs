using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuturesAssistantWPF.Windows
{
    public class AccountListModel
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string FuturesCompanyName { get; set; }
        public string LatestDataDate { get; set; }
    }
    public partial class MainWindow : FuturesAssistantWPF.Windows.WindowBase
    {
        //private delegate void FormControlInvoker();
        private BinaryFormatter formatter = new BinaryFormatter();
        //private object locker = new object();

        //public ToolsUserControl()
        //{
        //    InitializeComponent();
        //    InitializeToolsUserControl();
        //}

        private void InitializeToolsUserControl()
        {
            this._checkBox记住密码.IsChecked = bool.Parse(_Helper.GetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString()));
            this._checkBox自动登录.IsChecked = bool.Parse(_Helper.GetParameter(ParameterName.AutoLogin.ToString(), false.ToString()));

            //
            InitializeAccountList();

            //
            this._listBox品种列表.ItemsSource = new StatementContext(typeof(Commodity)).Commoditys.ToList();
        }

        private void _button合并图片_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("MergePicWPF.exe");
        }

        private void _button交易时间_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("TradeTimeWPF.exe");
        }

        private bool SelectTypes(out List<Type> selectedTypes)
        {
            SelectTableNameList st = new SelectTableNameList();
            this.Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
                if (winformWindow != null)
                {
                    new WindowInteropHelper(st)
                    {
                        Owner = winformWindow.Handle
                    };
                }
                st.ShowDialog();
            }));
            selectedTypes = st.SelectedTypes;
            return st.DialogResult.Value;
        }

        private void _buttonFileToSql_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonSqlToFile_Click(object sender, MouseButtonEventArgs e)
        {
        }

        private void _button恢复_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _button备份_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _button导入文华数据_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            FolderDialog sfd = new FolderDialog();
            if (sfd.DisplayDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Thread th = new Thread(new ThreadStart(() => ImportStocksFromWh6MonthStatement(sfd.Path)));
                th.Start();
            }
        }

        /// <summary>
        /// 从文华6月结单解析数据，得到蜡烛图数据
        /// </summary>
        /// <param name="wh6MonthStatementsDirPath">从文华6软件导出的月结单文件夹</param>
        private void ImportStocksFromWh6MonthStatement(string wh6MonthStatementsDirPath)
        {
            try
            {
                List<Wh6Trade> wh6Trades = new List<Wh6Trade>();
                using (StatementContext statement = new StatementContext(typeof(Account), typeof(Stock)))
                {
                    var currentAccount = statement.Accounts.FirstOrDefault(model => model.Id == _Session.SelectedAccountId);
                    var files = Directory.GetFileSystemEntries(wh6MonthStatementsDirPath, "*.*.txt");

                    //初始化状态
                    InitialStatusAndProgressBar(5);

                    //
                    foreach (var file in files)
                    {

                        //更新状态
                        UpdateStatusAndProgressBar(string.Format("正在处理月结算文件{0}", System.IO.Path.GetFileName(file)));

                        //
                        var reader = new StreamReader(File.OpenRead(file), Encoding.GetEncoding("gb2312"));
                        string monthStatement = reader.ReadToEnd();

                        //******************************
                        // 客户信息

                        //更新状态
                        UpdateStatusAndProgressBar("导入客户信息", 1);

                        monthStatement = monthStatement.Substring(monthStatement.IndexOf("交易结算"));
                        string info = monthStatement.Substring(0, monthStatement.IndexOf("资金状况"));

                        Wh6Info wh6Info = ParseWh6Info(info);
                        //******************************


                        //*******************
                        // 检验导入的数据是否与本地选定的账户匹配
                        if (currentAccount != null)
                        {
                            if (!currentAccount.AccountNumber.Contains(wh6Info.AccountNumber))
                            {
                                this.AddLog(string.Format("不允许从<{0}>向<{1}>导入数据，导入数据必须与当前选定账户匹配！", wh6Info.AccountNumber, currentAccount.AccountNumber));
                                this.OverStatusAndProgressBar();
                                return;
                            }
                        }
                        else
                        {
                            this.AddLog("未发现选定账户！");
                            this.OverStatusAndProgressBar();
                            return;
                        }
                        //*******************


                        //******************************
                        // 成交记录

                        //更新状态
                        UpdateStatusAndProgressBar("导入成交记录", 1);

                        monthStatement = monthStatement.Substring(monthStatement.IndexOf("成交记录"));
                        string trades = monthStatement.Substring(0, monthStatement.IndexOf("平仓明细"));

                        wh6Trades.AddRange(ImportWh6Trades(trades));
                        //******************************

                    }

                    //********************************
                    // 得到蜡烛图数据

                    //更新状态
                    UpdateStatusAndProgressBar("生成蜡烛图数据", 1);

                    var firstStock = statement.Stocks.Where(model => model.AccountId == currentAccount.Id).OrderBy(model => model.Date).FirstOrDefault();
                    var firstWh6Trade = wh6Trades.OrderBy(model => model.Date).FirstOrDefault();
                    if (firstStock != null)
                    {
                        if (firstWh6Trade != null)
                        {
                            DateTime firstDate = firstWh6Trade.Date;
                            DateTime lastDate = firstStock.Date.AddDays(-1);
                            decimal open = firstStock.Open, close = firstStock.Close, high = firstStock.High, low = firstStock.Low;
                            while (firstDate <= lastDate)
                            {
                                if (lastDate.DayOfWeek != DayOfWeek.Saturday && lastDate.DayOfWeek != DayOfWeek.Sunday)
                                {
                                    var tempWh6Trades = wh6Trades.Where(model => model.Date == lastDate);
                                    decimal profit = 0, volume = 0;
                                    foreach (var wh6Trade in tempWh6Trades)
                                    {
                                        profit += wh6Trade.ClosedProfit - wh6Trade.Commission;
                                        volume += wh6Trade.Amount;
                                    }
                                    Stock stock = new Stock();
                                    stock.Id = Guid.NewGuid();
                                    stock.Date = lastDate.Date.Date;
                                    stock.Close = open;
                                    stock.Open = stock.Close - profit;
                                    stock.High = stock.Close > stock.Open ? stock.Close : stock.Open;
                                    stock.Low = stock.Close < stock.Open ? stock.Close : stock.Open;
                                    stock.AccountId = currentAccount.Id;
                                    stock.Volume = volume;
                                    statement.AddStock(stock);
                                }
                                lastDate = lastDate.AddDays(-1);
                            }

                            // 更新状态
                            UpdateStatusAndProgressBar("保存数据", 1);

                            this.AddLog(string.Format("从文华6月结单导入数据完毕！", currentAccount.AccountNumber));
                            this.OverStatusAndProgressBar();
                        }
                    }
                    else
                    {
                        this.AddLog(string.Format("本地没有<{0}>的数据，请先从保证金监控中心下载数据，然后再手动导入数据！", currentAccount.AccountNumber));
                        this.OverStatusAndProgressBar();
                    }
                    //********************************

                    UpdateStatusAndProgressBar(progressBarValueIncrease: 1);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.OverStatusAndProgressBar();
            }
        }

        /// <summary>
        /// 初始化状态条和进度条，并显示。
        /// </summary>
        /// <param name="progressMaxValue">进度条最大值</param>
        private void InitialStatusAndProgressBar(int progressMaxValue)
        {
            this.Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                this._textBlock实时状态.Text = "";
                this._textBlock实时状态.Visibility = System.Windows.Visibility.Visible;
                this._progressBar进度.Visibility = System.Windows.Visibility.Visible;
                this._progressBar进度.Maximum = progressMaxValue;
                this._progressBar进度.Value = 0;
                this._progressBar进度._Refresh();
                this._textBlock实时状态._Refresh();
            }));
        }

        /// <summary>
        /// 设置状态条文本
        /// </summary>
        /// <param name="text">状态条文本</param>
        /// <param name="progressBarValueIncrease">进度条需要增加的值</param>
        private void UpdateStatusAndProgressBar(string text = "", int progressBarValueIncrease = 0)
        {
            this.Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    this._textBlock实时状态.Text = text;
                    this._textBlock实时状态._Refresh();
                }
                if (progressBarValueIncrease != 0)
                {
                    if (this._progressBar进度.Value + progressBarValueIncrease > this._progressBar进度.Maximum)
                    {
                        throw new OverflowException("超出进度条允许的最大值！");
                    }
                    this._progressBar进度.Value += progressBarValueIncrease;
                    this._progressBar进度._Refresh();
                }
            }));
        }


        /// <summary>
        /// 状态条和进度条功用完毕，全部隐藏。
        /// </summary>
        private void OverStatusAndProgressBar()
        {
            this.Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                this._textBlock实时状态.Text = "";
                this._textBlock实时状态.Visibility = System.Windows.Visibility.Collapsed;
                this._progressBar进度.Visibility = System.Windows.Visibility.Collapsed;
                this._textBlock实时状态._Refresh();
                this._progressBar进度._Refresh();
            }));
        }

        private Wh6Info ParseWh6Info(string info)
        {
            Wh6Info wh6Info = new Wh6Info();
            // 客户号
            info = info.Substring(info.IndexOf("客户号"));
            string accountNumber = info.Substring(4, info.IndexOf("客户名称") - 4).Trim();
            // 客户名称
            info = info.Substring(info.IndexOf("客户名称"));
            string customerName = info.Substring(5, info.IndexOf("日期") - 5).Trim();

            wh6Info.AccountNumber = accountNumber;
            wh6Info.CustomerName = customerName;
            return wh6Info;
        }

        private List<Wh6Trade> ImportWh6Trades(string tradeStr)
        {
            try
            {
                List<Wh6Trade> trades = new List<Wh6Trade>();
                string[] values = tradeStr.Split('\n');

                //
                for (int i = 0; i < values.Length; i++)
                {
                    try
                    {
                        Wh6Trade trade = new Wh6Trade();
                        string[] strs = values[i].Trim().Split('|');

                        string Date = strs[1].Trim();
                        int year = Convert.ToInt32(Date.Substring(0, 4));
                        int month = Convert.ToInt32(Date.Substring(4, 2));
                        int day = Convert.ToInt32(Date.Substring(6, 2));
                        trade.Date = new DateTime(year, month, day);
                        trade.Exchange = strs[2].Trim();
                        trade.Item = strs[3].Trim();
                        trade.DeliveryDate = strs[4].Trim();
                        trade.BS = strs[5].Trim();
                        trade.SH = strs[6].Trim();
                        trade.Price = Convert.ToDecimal(strs[7].Trim());
                        trade.Size = Convert.ToInt32(strs[8].Trim());
                        trade.Amount = Convert.ToDecimal(strs[9].Trim());
                        trade.OC = strs[10].Trim();
                        trade.Commission = Convert.ToDecimal(strs[11].Trim());
                        trade.ClosedProfit = Convert.ToDecimal(strs[12].Trim());
                        trade.AccountId = _Session.SelectedAccountId;
                        //
                        trades.Add(trade);
                    }
                    catch (Exception)
                    {
                    }
                }
                return trades;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        public void AddLog(string message)
        {
            this.Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                this._listBox运行日志.Items.Add(string.Format("{0}：{1}", DateTime.Now, message));
                this._listBox运行日志._Refresh();
            }));
        }


        private string saveDirPath = System.Windows.Forms.Application.StartupPath + "\\statement";

        private void _button监控中心数据文件_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe ", saveDirPath);
            //using (StatementIO statement = new StatementIO())
            //{
            //    var account = statement.Accounts.FirstOrDefault(acc => acc.Id == _Session.CurrentAccountId);
            //    if (account != null)
            //    {
            //        string path = saveDirPath + "\\" + account.AccountNumber + "\\date";
            //        System.Diagnostics.Process.Start("explorer.exe ", path);
            //    }
            //}
        }

        private void _button修改用户资料_Click(object sender, RoutedEventArgs e)
        {
            ModifyUserInfo mui = new ModifyUserInfo(_Session.LoginedUserId);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(mui)
                {
                    Owner = winformWindow.Handle
                };
            }
            mui.ShowDialog();
        }

        private void _checkBox记住密码_Unchecked(object sender, RoutedEventArgs e)
        {
            this._checkBox自动登录.IsChecked = false;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString());
        }

        private void _checkBox记住密码_Checked(object sender, RoutedEventArgs e)
        {
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Checked(object sender, RoutedEventArgs e)
        {
            this._checkBox记住密码.IsChecked = true;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Unchecked(object sender, RoutedEventArgs e)
        {
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), false.ToString());
        }

        private void _listBox交易账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this._btn删除账号.Visibility = System.Windows.Visibility.Visible;
            this._btn允许下载.Visibility = System.Windows.Visibility.Visible;
            if (this._listBox交易账户列表.SelectedItem != null)
            {
                using (StatementContext context = new StatementContext(typeof(Account), typeof(Parameter)))
                {
                    var tm = this._listBox交易账户列表.SelectedItem as AccountListModel;
                    Account acc = context.Accounts.FirstOrDefault(m => m.Id == tm.Id);
                    this._btn允许下载.Content = acc.IsAllowLoad ? _Session.BUTTON_TEXT_设置为禁止下载 : _Session.BUTTON_TEXT_设置为允许下载;
                }
            }
        }

        //private void _btn删除账号_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this._listBox交易账户列表.SelectedItem != null
        //        && MessageBox.Show("确认要删除当前交易账号吗？", "删除确认", MessageBoxButton.YesNo) == MessageBoxResult.OK)
        //    {
        //        using (StatementContext context = new StatementContext())
        //        {
        //            Account acc = this._listBox交易账户列表.SelectedItem as Account;
        //            context.Accounts.Remove(acc);
        //            context.SaveChanges();
        //            _Session.AccountListOfLoginedUser = context.Accounts.ToList();
        //            this._listBox交易账户列表.ItemsSource = null;
        //            this._listBox交易账户列表.ItemsSource = context.Accounts.ToList();
        //            this._btn删除账号.Visibility = System.Windows.Visibility.Collapsed;
        //        }
        //    }
        //}

        private void _btn允许下载_Click(object sender, RoutedEventArgs e)
        {
            if (this._btn允许下载.Content.Equals(_Session.BUTTON_TEXT_设置为禁止下载))
            {
                MessageBox.Show("禁止每次下载数据的账户，将在超过60天时，跟随下载一次。");
            }
            this._btn允许下载.Visibility = System.Windows.Visibility.Visible;
            if (this._listBox交易账户列表.SelectedItem != null)
            {
                using (StatementContext context = new StatementContext(typeof(Account)))
                {
                    var tm = this._listBox交易账户列表.SelectedItem as AccountListModel;
                    Account acc = context.Accounts.FirstOrDefault(m => m.Id == tm.Id);
                    acc.IsAllowLoad = !acc.IsAllowLoad;
                    context.EditAccount(acc);
                    if (acc.IsAllowLoad)
                    {
                        this._btn允许下载.Content = _Session.BUTTON_TEXT_设置为禁止下载;
                    }
                    else
                    {
                        this._btn允许下载.Content = _Session.BUTTON_TEXT_设置为允许下载;
                    }

                }
            }
        }

        /// <summary>
        /// 初始化账户列表
        /// </summary>
        private void InitializeAccountList()
        {
            using (StatementContext context = new StatementContext(typeof(Account), typeof(Commodity), typeof(FundStatus)))
            {
                List<AccountListModel> accountList = new List<AccountListModel>();
                foreach (var acc in context.Accounts)
                {
                    var fun = context.FundStatus.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).LastOrDefault();
                    accountList.Add(new AccountListModel()
                    {
                        Id = acc.Id,
                        AccountNumber = acc.AccountNumber,
                        CustomerName = acc.CustomerName,
                        FuturesCompanyName = acc.FuturesCompanyName,
                        LatestDataDate = fun == null ? "" : fun.Date.ToShortDateString()
                    });
                }
                this._listBox交易账户列表.ItemsSource = accountList;
                this._combox账户列表.ItemsSource = accountList;
                this._combox账户列表.SelectedItem = accountList.FirstOrDefault(m => m.Id == _Session.SelectedAccountId);
            }
        }
        private void _btn删除数据_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_datePicker删除开始时间.SelectedDate.HasValue)
                {
                    var date = _datePicker删除开始时间.SelectedDate.Value.Date;
                    StatementContext context = new StatementContext(typeof(Account));
                    if (MessageBox.Show(string.Concat("确认要删除账户<",
                        context.Accounts.FirstOrDefault(m => m.Id == _Session.SelectedAccountId).AccountNumber,
                        ">",
                        _datePicker删除开始时间.SelectedDate.Value.Date.ToString("yyyy年MM月dd日"), "以及之后的数据吗？"), "删除数据确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        Thread mythread2 = new Thread(() => DeleteData(date));
                        mythread2.IsBackground = true;
                        //設置為後臺線程,程式關閉后進程也關閉,如果不設置true，則程式關閉,此線程還在內存,不會關閉           
                        mythread2.Start();
                    }

                }
                else
                {
                    MessageBox.Show("请先选择删除数据的起始时间！");
                    _datePicker删除开始时间.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteData(DateTime date)
        {

            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 30, 0)))
            {
                InitialStatusAndProgressBar(11);
                StatementStream stream = new StatementStream();

                UpdateStatusAndProgressBar("(1/11)正在删除平仓明细数据...", 1);
                List<ClosedTradeDetail> ClosedTradeDetails = stream.GetClosedTradeDetails();
                ClosedTradeDetails.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.ActualDate >= date);
                File.Delete(_Session.ClosedTradeDetailFilePath);
                stream.Append(ClosedTradeDetails);

                UpdateStatusAndProgressBar("(2/11)正在删除品种统计数据...", 1);
                List<CommoditySummarization> CommoditySummarizations = stream.GetCommoditySummarizations();
                CommoditySummarizations.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.CommoditySummarizationFilePath);
                stream.Append(CommoditySummarizations);


                UpdateStatusAndProgressBar("(3/11)正在删除结算单数据...", 1);
                List<FundStatus> FundStatus = stream.GetFundStatus();
                FundStatus.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.FundStatusFilePath);
                stream.Append(FundStatus);


                UpdateStatusAndProgressBar("(4/11)正在删除持仓明细数据...", 1);
                List<PositionDetail> PositionDetails = stream.GetPositionDetails();
                PositionDetails.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.DateForPosition >= date);
                File.Delete(_Session.PositionDetailFilePath);
                stream.Append(PositionDetails);


                UpdateStatusAndProgressBar("(5/11)正在删除持仓数据...", 1);
                List<Position> Positions = stream.GetPositions();
                Positions.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.PositionFilePath);
                stream.Append(Positions);


                UpdateStatusAndProgressBar("(6/11)正在删除出入金数据...", 1);
                List<Remittance> Remittances = stream.GetRemittances();
                Remittances.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.RemittanceFilePath);
                stream.Append(Remittances);


                UpdateStatusAndProgressBar("(7/11)正在删除股票图数据...", 1);
                List<Stock> Stocks = stream.GetStocks();
                Stocks.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.StockFilePath);
                stream.Append(Stocks);


                UpdateStatusAndProgressBar("(8/11)正在删除交易明细数据...", 1);
                List<TradeDetail> TradeDetails = stream.GetTradeDetails();
                TradeDetails.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.ActualTime >= date);
                File.Delete(_Session.TradeDetailFilePath);
                stream.Append(TradeDetails);


                UpdateStatusAndProgressBar("(9/11)正在删除交易数据...", 1);
                List<Trade> Trades = stream.GetTrades();
                Trades.RemoveAll(m => m.AccountId == _Session.SelectedAccountId && m.Date >= date);
                File.Delete(_Session.TradeFilePath);
                stream.Append(Trades);

                UpdateStatusAndProgressBar("(10/11)正在校准权益...", 1);
                using (StatementContext statement = new StatementContext(typeof(Stock), typeof(FundStatus), typeof(Account)))
                {
                    var stocks = statement.Stocks.Where(m => m.AccountId == _Session.SelectedAccountId).OrderBy(m => m.Date);
                    var fundStatus = statement.FundStatus.Where(m => m.AccountId == _Session.SelectedAccountId).OrderBy(m => m.Date);
                    if (stocks.Count() > 0 && fundStatus.Count() > 0)
                    {
                        var lastStock = stocks.ToList().LastOrDefault();
                        var lastFundStatus = fundStatus.ToList().LastOrDefault();
                        var diff = lastFundStatus.CustomerRights - lastStock.Close;
                        if (diff != 0)
                        {
                            foreach (var stc in stocks)
                            {
                                stc.Open += diff;
                                stc.High += diff;
                                stc.Low += diff;
                                stc.Close += diff;
                            }
                            statement.Cover(statement.Stocks.ToList());
                        }
                    }
                    UpdateStatusAndProgressBar("(11/11)正在保存数据...", 1);

                    OverStatusAndProgressBar();
                    this.AddLog(string.Concat("账号：<", statement.Accounts.FirstOrDefault(m => m.Id == _Session.SelectedAccountId).AccountNumber,
                        ">", date.ToString("yyyy年MM月dd日"), "以及之后的数据以全部删除，重启软件生效。"));
                }
                ts.Complete();
            }
        }

        private void _btn扫描品种_Click(object sender, RoutedEventArgs e)
        {
            using (StatementContext statement = new StatementContext(typeof(Commodity), typeof(CommoditySummarization)))
            {
                List<Commodity> commodityList = new List<Commodity>();
                var commoditys = statement.Commoditys;
                var css = statement.CommoditySummarizations;
                int count = 0;
                foreach (var cs in css)
                {
                    if (commoditys.FirstOrDefault(m => m.Code.Equals(cs.Commodity)) == null
                        && commodityList.FirstOrDefault(m => m.Code.Equals(cs.Commodity)) == null)
                    {
                        Commodity cd = new Commodity();
                        cd.Code = cs.Commodity;
                        cd.Name = cs.Commodity;
                        commodityList.Add(cd);
                        count++;
                    }
                }
                statement.AddCommoditys(commodityList);

                this._listBox品种列表.ItemsSource = null;
                this._listBox品种列表.ItemsSource = new StatementContext(typeof(Commodity)).Commoditys.ToList();
                MessageBox.Show(string.Concat("扫描完成！新增",count,"个品种。"));
            }
        }

        private void _btn品种更名_Click(object sender, RoutedEventArgs e)
        {
            if (_listBox品种列表.SelectedItem == null)
            {
                MessageBox.Show("请先选择需要修改的品种。");
            }
            using (StatementContext context = new StatementContext())
            {
                Commodity comm = this._listBox品种列表.SelectedItem as Commodity;
                ModifyCommodity mc = new ModifyCommodity(comm);
                mc.ShowDialog();
                this._listBox品种列表.ItemsSource = null;
                this._listBox品种列表.ItemsSource = new StatementContext(typeof(Commodity)).Commoditys.ToList();
            }
        }

        private void _listBox云备份文件列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this._btn恢复.IsEnabled = true;
        }

        private void _btn恢复_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _btn获取列表_Click(object sender, RoutedEventArgs e)
        {
            //POP3 _Popt = new POP3("pop.163.com", 110);
            //System.Data.DataTable _Mail = _Popt.GetMail("outlook163", "mail351489", 1);
        }

        private void _btn云备份_Click(object sender, RoutedEventArgs e)
        {
            Thread th = new Thread(new ThreadStart(() => MailYunBackup()));
            th.IsBackground = true;
            th.Start();
        }

        private void MailYunBackup()
        {
            using (StatementContext statement = new StatementContext(typeof(User)))
            {
                var user = statement.Users.FirstOrDefault(model => model.Id.Equals(_Session.LoginedUserId));
                //
                MessageBoxResult result = MessageBoxResult.Cancel;
                this.Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    result = MessageBox.Show(string.Concat("将要备份本地所有数据至<", user.Email, ">，确认后继续操作！"), "备份确认", MessageBoxButton.OKCancel);
                }));
                if (result == MessageBoxResult.OK)
                {
                    this.Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        this._btn云备份.Content = "正在上传...";
                        this._btn云备份.IsEnabled = false;
                    }));
                    this.AddLog("正在向邮箱备份数据...");
                    //确定smtp服务器地址。实例化一个Smtp客户端
                    System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.163.com");

                    //构造一个发件人地址对象
                    MailAddress from = new MailAddress("outlook163@163.com", "期货助手", Encoding.UTF8);
                    //构造一个收件人地址对象
                    MailAddress to = new MailAddress(user.Email, user.UserName, Encoding.UTF8);

                    //构造一个Email的Message对
                    MailMessage message = new MailMessage(from, to);


                    //添加邮件主题和内容
                    message.Subject = string.Concat("期货助手 - 备份文件(", _Session.LatestFundStatusDate.ToShortDateString(), ")");
                    message.SubjectEncoding = Encoding.UTF8;
                    message.Body = string.Concat("使用方法：将附件中的文件覆盖原数据文件，即可恢复数据！");
                    message.BodyEncoding = Encoding.UTF8;
                    message.Attachments.Add(new Attachment(_Session.CommodityFilePath));
                    message.Attachments.Add(new Attachment(_Session.AccountFilePath));
                    message.Attachments.Add(new Attachment(_Session.ClosedTradeDetailFilePath));
                    message.Attachments.Add(new Attachment(_Session.CommoditySummarizationFilePath));
                    message.Attachments.Add(new Attachment(_Session.FundStatusFilePath));
                    message.Attachments.Add(new Attachment(_Session.ParameterFilePath));
                    message.Attachments.Add(new Attachment(_Session.PositionFilePath));
                    message.Attachments.Add(new Attachment(_Session.PositionDetailFilePath));
                    message.Attachments.Add(new Attachment(_Session.RemittanceFilePath));
                    message.Attachments.Add(new Attachment(_Session.StockFilePath));
                    message.Attachments.Add(new Attachment(_Session.TradeFilePath));
                    message.Attachments.Add(new Attachment(_Session.TradeDetailFilePath));
                    message.Attachments.Add(new Attachment(_Session.UserFilePath));


                    //设置邮件的信息                
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    message.BodyEncoding = System.Text.Encoding.UTF8;
                    message.IsBodyHtml = false;

                    //设置用户名和密码。
                    client.UseDefaultCredentials = false;
                    string username = "outlook163";
                    string passwd = "mail351489";
                    //用户登陆信息
                    NetworkCredential myCredentials = new NetworkCredential(username, passwd);
                    client.Credentials = myCredentials;
                    //发送邮件
                    client.Send(message);

                    //提示发送成功
                    this.Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        this._btn云备份.Content = "云备份";
                        this._btn云备份.IsEnabled = true;
                    }));
                    this.AddLog(string.Concat("本地所有数据已备份至<", user.Email, ">。"));
                }
            }
        }

        /// <summary> 
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法) 
        /// </summary> 
        /// <param name="address">文件上传到的服务器</param> 
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param> 
        /// <param name="saveName">文件上传后的名称</param> 
        /// <param name="progressBar">上传进度条</param> 
        /// <returns>成功返回1，失败返回0</returns> 
        private int Upload_Request(string address, string fileNamePath, string saveName, ProgressBar progressBar)
        {
            int returnValue = 0;

            // 要上传的文件 
            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new BinaryReader(fs);

            //时间戳 
            string strBoundary = "----------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = Encoding.ASCII.GetBytes("\r\n--" + strBoundary + "\r\n");

            //请求头部信息 
            StringBuilder sb = new StringBuilder();
            sb.Append("--");
            sb.Append(strBoundary);
            sb.Append("\r\n");
            sb.Append("Content-Disposition: form-data; name=\"");
            sb.Append("file");
            sb.Append("\"; filename=\"");
            sb.Append(saveName);
            sb.Append("\"");
            sb.Append("\r\n");
            sb.Append("Content-Type: ");
            sb.Append("application/octet-stream");
            sb.Append("\r\n");
            sb.Append("\r\n");
            string strPostHeader = sb.ToString();
            byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);

            // 根据uri创建HttpWebRequest对象 
            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
            httpReq.Method = "POST";

            //对发送的数据不使用缓存 
            httpReq.AllowWriteStreamBuffering = false;

            //设置获得响应的超时时间（300秒） 
            httpReq.Timeout = 300000;
            httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
            long length = fs.Length + postHeaderBytes.Length + boundaryBytes.Length;
            long fileLength = fs.Length;
            httpReq.ContentLength = length;
            try
            {
                progressBar.Maximum = int.MaxValue;
                progressBar.Minimum = 0;
                progressBar.Value = 0;

                //每次上传4k 
                int bufferLength = 4096;
                byte[] buffer = new byte[bufferLength];

                //已上传的字节数 
                long offset = 0;

                //开始上传时间 
                DateTime startTime = DateTime.Now;
                int size = r.Read(buffer, 0, bufferLength);
                Stream postStream = httpReq.GetRequestStream();

                //发送请求头部消息 
                postStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                while (size > 0)
                {
                    postStream.Write(buffer, 0, size);
                    offset += size;
                    progressBar.Value = (int)(offset * (int.MaxValue / length));
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                    string lblTime = "";
                    string lblSpeed = "";
                    string lblState = "";
                    string lblSize = "";
                    lblTime = "已用时：" + second.ToString("F2") + "秒";
                    if (second > 0.001)
                    {
                        lblSpeed = " 平均速度：" + (offset / 1024 / second).ToString("0.00") + "KB/秒";
                    }
                    else
                    {
                        lblSpeed = " 正在连接…";
                    }
                    lblState = "已上传：" + (offset * 100.0 / length).ToString("F2") + "%";
                    lblSize = (offset / 1048576.0).ToString("F2") + "M/" + (fileLength / 1048576.0).ToString("F2") + "M";
                    System.Windows.Forms.Application.DoEvents();
                    size = r.Read(buffer, 0, bufferLength);
                }
                //添加尾部的时间戳 
                postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
                postStream.Close();

                //获取服务器端的响应 
                WebResponse webRespon = httpReq.GetResponse();
                Stream s = webRespon.GetResponseStream();
                StreamReader sr = new StreamReader(s);

                //读取服务器端返回的消息 
                String sReturnString = sr.ReadLine();
                s.Close();
                sr.Close();
                if (sReturnString == "Success")
                {
                    returnValue = 1;
                }
                else if (sReturnString == "Error")
                {
                    returnValue = 0;
                }

            }
            catch
            {
                returnValue = 0;
            }
            finally
            {
                fs.Close();
                r.Close();
            }

            return returnValue;
        }
    }
}
