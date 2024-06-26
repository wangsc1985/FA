using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Types;
using FuturesAssistant.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuturesAssistant.Controls {
    public class AccountListModel {
        public Guid Id { get; set; }
        public int Type { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string Password { get; set; }
        public string FuturesCompanyName { get; set; }
        public string LatestDataDate { get; set; }
        public bool IsAllowLoad { get; set; }
        public string Profit { get; set; }
        public string Balance { get; set; }
        public string ProfitForeground { get; set; }
        public string TypeForeground { get; set; }
        public string State1 { get; set; }
        public string State2 { get; set; }
    }
    /// <summary>
    /// SettingUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class SettingUserControl: UserControl {
        private delegate void FormControlInvoker();
        private BinaryFormatter formatter = new BinaryFormatter();
        private object locker = new object();
        private MainWindow mainWindow;

        public SettingUserControl() {
            InitializeComponent();
        }

        public void Initialize(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
            Dispatcher.Invoke(new FormControlInvoker(() => {
                _checkBox记住密码.IsChecked = true;
                _checkBox记住密码.IsChecked = bool.Parse(_Helper.GetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString()));
                _checkBox自动登录.IsChecked = bool.Parse(_Helper.GetParameter(ParameterName.AutoLogin.ToString(), false.ToString()));
                _datePicker删除开始时间.Text = DateTime.Today.ToShortDateString();
            }));
            //
            InitializeAccountList();
        }

        /// <summary>
        /// 从文华6月结单解析数据，得到蜡烛图数据
        /// </summary>
        /// <param name="wh6MonthStatementsDirPath">从文华6软件导出的月结单文件夹</param>
        private void ImportStocksFromWh6MonthStatement(string wh6MonthStatementsDirPath) {
            try {
                List<Wh6Trade> wh6Trades = new List<Wh6Trade>();
                using (StatementContext statement = new StatementContext(typeof(Account), typeof(Stock))) {
                    var currentAccount = statement.Accounts.FirstOrDefault(model => model.Id == _Session.SelectedAccountId);
                    var files = Directory.GetFileSystemEntries(wh6MonthStatementsDirPath, "*.*.txt");

                    //初始化状态
                    InitialStatusAndProgressBar(5);

                    //
                    foreach (var file in files) {

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
                        if (currentAccount != null) {
                            if (!currentAccount.AccountNumber.Contains(wh6Info.AccountNumber)) {
                                AddLog(string.Format("不允许从<{0}>向<{1}>导入数据，导入数据必须与当前选定账户匹配！", wh6Info.AccountNumber, currentAccount.AccountNumber));
                                OverStatusAndProgressBar();
                                return;
                            }
                        } else {
                            AddLog("未发现选定账户！");
                            OverStatusAndProgressBar();
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
                    if (firstStock != null) {
                        if (firstWh6Trade != null) {
                            DateTime firstDate = firstWh6Trade.Date;
                            DateTime lastDate = firstStock.Date.AddDays(-1);
                            decimal open = firstStock.Open, close = firstStock.Close, high = firstStock.High, low = firstStock.Low;
                            while (firstDate <= lastDate) {
                                if (lastDate.DayOfWeek != DayOfWeek.Saturday && lastDate.DayOfWeek != DayOfWeek.Sunday) {
                                    var tempWh6Trades = wh6Trades.Where(model => model.Date == lastDate);
                                    decimal profit = 0, volume = 0;
                                    foreach (var wh6Trade in tempWh6Trades) {
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

                            AddLog(string.Format("从文华6月结单导入数据完毕！", currentAccount.AccountNumber));
                            OverStatusAndProgressBar();
                        }
                    } else {
                        AddLog(string.Format("本地没有<{0}>的数据，请先从保证金监控中心下载数据，然后再手动导入数据！", currentAccount.AccountNumber));
                        OverStatusAndProgressBar();
                    }
                    //********************************

                    UpdateStatusAndProgressBar(progressBarValueIncrease: 1);
                }
            } catch (Exception) {
                throw;
            } finally {
                OverStatusAndProgressBar();
            }
        }

        /// <summary>
        /// 初始化状态条和进度条，并显示。
        /// </summary>
        /// <param name="progressMaxValue">进度条最大值</param>
        private void InitialStatusAndProgressBar(int progressMaxValue) {
            Dispatcher.Invoke(new FormControlInvoker(() => {
                _textBlock实时状态.Text = "";
                _textBlock实时状态.Visibility = Visibility.Visible;
                _progressBar进度.Visibility = Visibility.Visible;
                _progressBar进度.Maximum = progressMaxValue;
                _progressBar进度.Value = 0;
                _progressBar进度._Refresh();
                _textBlock实时状态._Refresh();
            }));
        }

        /// <summary>
        /// 设置状态条文本
        /// </summary>
        /// <param name="text">状态条文本</param>
        /// <param name="progressBarValueIncrease">进度条需要增加的值</param>
        private void UpdateStatusAndProgressBar(string text = "", int progressBarValueIncrease = 0) {
            Dispatcher.Invoke(new FormControlInvoker(() => {
                if (!string.IsNullOrEmpty(text)) {
                    _textBlock实时状态.Text = text;
                    _textBlock实时状态._Refresh();
                }
                if (progressBarValueIncrease != 0) {
                    if (_progressBar进度.Value + progressBarValueIncrease > _progressBar进度.Maximum) {
                        throw new OverflowException("超出进度条允许的最大值！");
                    }
                    _progressBar进度.Value += progressBarValueIncrease;
                    _progressBar进度._Refresh();
                }
            }));
        }


        /// <summary>
        /// 状态条和进度条功用完毕，全部隐藏。
        /// </summary>
        private void OverStatusAndProgressBar() {
            Dispatcher.Invoke(new FormControlInvoker(() => {
                _textBlock实时状态.Text = "";
                _textBlock实时状态.Visibility = Visibility.Collapsed;
                _progressBar进度.Visibility = Visibility.Collapsed;
                _textBlock实时状态._Refresh();
                _progressBar进度._Refresh();
            }));
        }

        private Wh6Info ParseWh6Info(string info) {
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

        private List<Wh6Trade> ImportWh6Trades(string tradeStr) {
            try {
                List<Wh6Trade> trades = new List<Wh6Trade>();
                string[] values = tradeStr.Split('\n');

                //
                for (int i = 0; i < values.Length; i++) {
                    try {
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
                    } catch (Exception) {
                    }
                }
                return trades;
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }

        public void AddLog(string message) {
            Dispatcher.Invoke(new FormControlInvoker(() => {
                _listBox运行日志.Items.Add(string.Format("{0}：{1}", DateTime.Now, message));
                _listBox运行日志._Refresh();
            }));
        }


        private string saveDirPath = System.Windows.Forms.Application.StartupPath + "\\statement";

        private void _button监控中心数据文件_Click(object sender, RoutedEventArgs e) {
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

        private void _button修改用户资料_Click(object sender, RoutedEventArgs e) {
            ModifyUserInfo mui = new ModifyUserInfo(_Session.LoginedUserId);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(mui) {
                    Owner = winformWindow.Handle
                };
            }
            mui.ShowDialog();
        }

        private void _checkBox记住密码_Unchecked(object sender, RoutedEventArgs e) {
            _checkBox自动登录.IsChecked = false;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString());
        }

        private void _checkBox记住密码_Checked(object sender, RoutedEventArgs e) {
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Checked(object sender, RoutedEventArgs e) {
            _checkBox记住密码.IsChecked = true;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Unchecked(object sender, RoutedEventArgs e) {
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), false.ToString());
        }

        private void _btn允许下载_Click(object sender, RoutedEventArgs e) {
        }
        private const string ALLOW_LOAD = "禁止更新";
        private const string NOT_ALLOW_LOAD = "允许更新";
        private const string ADD_COOPERATE = "修改账户";
        private const string DISUSE_ACCOUNT = "禁用账户";
        private const string RESUME_ACCOUNT = "使用账户";

        private void _btn删除数据_Click(object sender, RoutedEventArgs e) {
            try {
                if (_datePicker删除开始时间.SelectedDate.HasValue) {
                    var date = _datePicker删除开始时间.SelectedDate.Value.Date;
                    StatementContext context = new StatementContext(typeof(Account));
                    if (MessageBox.Show(string.Concat("确认要删除账户【",
                        context.Accounts.FirstOrDefault(m => m.Id == _Session.SelectedAccountId).AccountNumber,
                        "】",
                        _datePicker删除开始时间.SelectedDate.Value.Date.ToString("yyyy年MM月dd日"), "以及之后的数据吗？"), "删除数据确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                        DeleteData(date);
                        //Thread mythread2 = new Thread(() => DeleteData(date));
                        //mythread2.IsBackground = true;
                        ////設置為後臺線程,程式關閉后進程也關閉,如果不設置true，則程式關閉,此線程還在內存,不會關閉           
                        //mythread2.Start();
                    }

                } else {
                    MessageBox.Show("请先选择删除数据的起始时间！");
                    _datePicker删除开始时间.Focus();
                }
            } catch (Exception ex) {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DeleteData(DateTime date) {

            try {
                mainWindow.ProgressInfoVisible(3);
                using (StatementContext statement = new StatementContext()) {
                    mainWindow.SetProgressText("(1/3)正在删除数据...");
                    statement.Delete(date, _Session.SelectedAccountId, typeof(ClosedTradeDetail), typeof(CommoditySummarization), typeof(FundStatus), typeof(PositionDetail), typeof(Position), typeof(Remittance), typeof(Stock), typeof(TradeDetail), typeof(Trade));
                    mainWindow.AddProgressValue(1);

                    mainWindow.SetProgressText("(2/3)正在校准权益...");
                    var stocks = statement.Stocks.Where(m => m.AccountId == _Session.SelectedAccountId).OrderBy(m => m.Date);
                    var fundStatus = statement.FundStatus.Where(m => m.AccountId == _Session.SelectedAccountId).OrderBy(m => m.Date);
                    if (stocks.Count() > 0 && fundStatus.Count() > 0) {
                        var lastStock = stocks.ToList().LastOrDefault();
                        var lastFundStatus = fundStatus.ToList().LastOrDefault();
                        var diff = lastFundStatus.CustomerRights - lastStock.Close;
                        if (diff != 0) {
                            foreach (var stc in stocks) {
                                stc.Open += diff;
                                stc.High += diff;
                                stc.Low += diff;
                                stc.Close += diff;
                            }
                            mainWindow.AddProgressValue(1);
                            statement.UpdateStocks(stocks);
                        }
                    }

                    mainWindow.SetProgressText("(3/3)正在保存数据...");
                    statement.SaveChanged();
                    mainWindow.AddProgressValue(1);

                    MessageBox.Show(string.Concat("账号【", statement.Accounts.FirstOrDefault(m => m.Id == _Session.SelectedAccountId).AccountNumber, "】 ", date.ToString("yyyy年MM月dd日"), " 以及之后的数据已全部删除。"));
                }
                mainWindow.InitializeUserControlsThreadStart();
            } catch (Exception ex) {
                OverStatusAndProgressBar();
                MessageBoxSync(ex.Message + "\n" + ex.StackTrace);
            } finally {
                mainWindow.ProgressInfoHidden();
            }
        }


        public void MessageBoxSync(string message) {
            Dispatcher.Invoke(new FormControlInvoker(() => {
                MessageBox.Show(message);
            }));
        }

        /// <summary> 
        /// 将本地文件上传到指定的服务器(HttpWebRequest方法) 
        /// </summary> 
        /// <param name="address">文件上传到的服务器</param> 
        /// <param name="fileNamePath">要上传的本地文件（全路径）</param> 
        /// <param name="saveName">文件上传后的名称</param> 
        /// <param name="progressBar">上传进度条</param> 
        /// <returns>成功返回1，失败返回0</returns> 
        private int Upload_Request(string address, string fileNamePath, string saveName, ProgressBar progressBar) {
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
            try {
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
                while (size > 0) {
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
                    if (second > 0.001) {
                        lblSpeed = " 平均速度：" + (offset / 1024 / second).ToString("0.00") + "KB/秒";
                    } else {
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
                if (sReturnString == "Success") {
                    returnValue = 1;
                } else if (sReturnString == "Error") {
                    returnValue = 0;
                }

            } catch {
                returnValue = 0;
            } finally {
                fs.Close();
                r.Close();
            }

            return returnValue;
        }


        private void _btnFromEmail_Click(object sender, RoutedEventArgs e) {
            _btnFromEmail.IsEnabled = false;
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
            fd.Multiselect = false;
            fd.Filter = "备份文件(*.bak)|*.bak";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                File.Copy(fd.FileName, _Session.DatabaseFilePath, true);
                mainWindow.InitializeUserControlsThreadStart();
                MessageBox.Show("数据已恢复！");
            }
            _btnFromEmail.IsEnabled = true;
        }

        private void _btn交易品种_Click(object sender, RoutedEventArgs e) {
            CommodityDialog cd = new CommodityDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(cd) {
                    Owner = winformWindow.Handle
                };
            }
            cd.ShowDialog();
        }

        private void _btn查询手续费_Click(object sender, RoutedEventArgs e) {
            CommissionDialog dialog = new CommissionDialog();
            dialog.ShowDialog();
        }

        private void _btnOpenDbDir_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + _Session.DatabaseFilePath);
        }


        public void InitializeAccountList() {
            using (StatementContext context = new StatementContext(typeof(Stock), typeof(Account), typeof(Commodity), typeof(FundStatus))) {
                List<AccountListModel> accountListMain = new List<AccountListModel>();
                List<AccountListModel> accountListSettingJY = new List<AccountListModel>();
                List<AccountListModel> accountListSettingPZ = new List<AccountListModel>();
                //context.Accounts;
                foreach (var acc in context.Accounts.OrderByDescending(m => m.Type)) {
                    var firstStock = context.Stocks.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().FirstOrDefault();
                    var lastStock = context.Stocks.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().LastOrDefault();

                    // 非隐藏账户
                    if (acc.Type % 10 != 0) {
                        accountListMain.Add(new AccountListModel() {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            TypeForeground = (acc.Type == 1 || acc.Type == 10) ? "BLACK" : "RED"
                        });
                    }

                    // 配资账户
                    if (acc.Type == 2 || acc.Type == 20) {
                        accountListSettingPZ.Add(new AccountListModel() {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            Password = acc.Password._RSADecrypt(),
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            Balance = lastStock == null ? "" : lastStock.Close.ToString("n"),
                            Profit = lastStock == null ? "" : lastStock.Close < 100 ? (lastStock.Close - firstStock.Open).ToString("n") : string.Concat((lastStock.Close - firstStock.Open).ToString("n"), " (", ((lastStock.Close - firstStock.Open) / firstStock.Open).ToString("P2"), ")"),
                            ProfitForeground = lastStock == null ? "BLACK" : lastStock.Close - firstStock.Open > 0 ? "RED" : "GREEN",
                            //TypeForeground = "RED",
                            State1 = acc.Type % 10 == 0 ? "Visible" : "Collapsed",
                        });
                    } else // 期货公司交易账户
                      {
                        accountListSettingJY.Add(new AccountListModel() {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            IsAllowLoad = acc.IsAllowLoad,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            Balance = lastStock == null ? "" : lastStock.Close.ToString("n"),
                            Profit = lastStock == null ? "" : lastStock.Close < 100 ? (lastStock.Close - firstStock.Open).ToString("n") : string.Concat((lastStock.Close - firstStock.Open).ToString("n"), " (", ((lastStock.Close - firstStock.Open) / firstStock.Open).ToString("P2"), ")"),
                            ProfitForeground = lastStock == null ? "BLACK" : lastStock.Close - firstStock.Open > 0 ? "RED" : "GREEN",
                            //TypeForeground = "BLACK",
                            State1 = acc.Type % 10 == 0 ? "Visible" : "Collapsed",
                            State2 = acc.IsAllowLoad ? "Hidden" : "Visible"
                        });
                    }
                }
                Dispatcher.Invoke(new FormControlInvoker(() => {
                    mainWindow._combox账户列表_SelectionChanged_unlink();
                    mainWindow._combox账户列表.ItemsSource = accountListMain;
                    mainWindow._combox账户列表.SelectedItem = accountListMain.FirstOrDefault(m => m.Id == _Session.SelectedAccountId);
                    mainWindow._combox账户列表_SelectionChanged_link();
                    _listBox交易账户列表.ItemsSource = accountListSettingJY.OrderByDescending(m => m.LatestDataDate).ToList();
                }));
            }
        }

        private void _button添加账户_Click(object sender, RoutedEventArgs e) {
            AddAccountWindow aaf = new AddAccountWindow();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(aaf) {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value) {
                using (StatementContext statement = new StatementContext(typeof(Account))) {
                    InitializeAccountList();
                }
            }
        }

        private void _button添加配资账户_Click(object sender, RoutedEventArgs e) {
            AddCooperateAccountDialog aaf = new AddCooperateAccountDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(aaf) {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value) {
                InitializeAccountList();
            }
        }

        private void _btn是否下载_Click(object sender, RoutedEventArgs e) {
        }

        private void _listBox交易账户列表_ContextMenuOpening(object sender, ContextMenuEventArgs e) {
            if (selectedAccountItem == null || (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)) {
                _menuItem列表隐藏.IsEnabled = false;
                _menuItem列表显示.IsEnabled = false;
                _menuItem允许下载.IsEnabled = false;
                _menuItem禁止下载.IsEnabled = false;
                return;
            } else {
                _menuItem列表隐藏.IsEnabled = true;
                _menuItem列表显示.IsEnabled = true;
                _menuItem允许下载.IsEnabled = true;
                _menuItem禁止下载.IsEnabled = true;
            }

            // 列表中不显示
            if (selectedAccountItem.Type % 10 == 0) {
                _menuItem列表隐藏.Visibility = Visibility.Collapsed;
                _menuItem列表显示.Visibility = Visibility.Visible;
            } else {
                _menuItem列表隐藏.Visibility = Visibility.Visible;
                _menuItem列表显示.Visibility = Visibility.Collapsed;
            }

            // 跟随下载
            if (selectedAccountItem.IsAllowLoad) {
                _menuItem允许下载.Visibility = Visibility.Collapsed;
                _menuItem禁止下载.Visibility = Visibility.Visible;
            } else {
                _menuItem允许下载.Visibility = Visibility.Visible;
                _menuItem禁止下载.Visibility = Visibility.Collapsed;
            }
        }

        private void _menuItem列表显示_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext(typeof(Account))) {
                var account = statement.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type / 10;
                statement.EditAccount(account);
                statement.SaveChanged();
                InitializeAccountList();
            }
        }

        private void _menuItem列表隐藏_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext(typeof(Account))) {
                var account = statement.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type * 10;
                statement.EditAccount(account);
                statement.SaveChanged();
                InitializeAccountList();
            }
        }

        private void _menuItem修改账户_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            EditCooperateAccountDialog acdd = new EditCooperateAccountDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(acdd) {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
            InitializeAccountList();
        }

        private void _menuItem允许下载_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext(typeof(Account))) {
                Account acc = context.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.EditAccount(acc);
                context.SaveChanged();
                if (!acc.IsAllowLoad) {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        private void _menuItem禁止下载_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext(typeof(Account))) {
                Account acc = context.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.EditAccount(acc);
                context.SaveChanged();
                if (!acc.IsAllowLoad) {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        AccountListModel selectedAccountItem = null;

        private void _listBox交易账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = _listBox交易账户列表.SelectedItem;
            if (item == null)
                selectedAccountItem = null;
            else
                selectedAccountItem = item as AccountListModel;
        }

        private void _menuItemP查看数据_Click(object sender, RoutedEventArgs e) {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(acdd) {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
        }

        private void _menuItemP自动填充数据_Click(object sender, RoutedEventArgs e) {

        }

        private void _menuItem添加账户_Click(object sender, RoutedEventArgs e) {
            AddAccountWindow aaf = new AddAccountWindow();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null) {
                new WindowInteropHelper(aaf) {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value) {
                using (StatementContext statement = new StatementContext(typeof(Account))) {
                    InitializeAccountList();
                }
            }
        }

        private void _btnTest_Click(object sender, RoutedEventArgs e) {

            //using (StatementContext statement = new StatementContext()) {
            //    var accId = Guid.Parse("ecaa37d7-cfb5-4f44-bbc9-05e7dafa2e7b");
            //    ParseTxtStatement(statement,accId);

            //    // 复权处理
            //    var stocks = statement.Stocks.Where(s => s.AccountId == accId).OrderBy(s => s.Date);
            //    var lastStock = stocks.LastOrDefault();
            //    var lastFundStatus = statement.FundStatus.Where(fs => fs.AccountId == accId).OrderBy(fs => fs.Date).LastOrDefault();
            //    if (lastStock != null && lastFundStatus != null) {
            //        decimal tmp = lastStock.Close - lastFundStatus.TodayBalance;
            //        if (tmp != 0) {
            //            foreach (var stock in stocks) {
            //                stock.Open -= tmp;
            //                stock.High -= tmp;
            //                stock.Low -= tmp;
            //                stock.Close -= tmp;
            //            }
            //            statement.UpdateStocks(stocks);
            //        }
            //    }
            //    statement.SaveChanged();
            //}
        }
        public void ParseTxtStatement(StatementContext statement, Guid accountId) {
            var files = Directory.GetFiles("e:\\80900");
            var settlementType = SettlementType.trade;

            var date = new DateTime();
            decimal close = 0.0M;


            foreach (var file in files) {
                Console.WriteLine(file);
                var reader = new StreamReader(file, Encoding.GetEncoding("gb2312"));
                //var ff = File.OpenText(file);
                var text = reader.ReadToEnd();
                //Console.WriteLine(content);

                /**
                 * 
                 * 
                           制表时间 Creation Date：20200908
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

                 * 
                 */

                List<TradeDetail> tradeDetails = new List<TradeDetail>();
                List<Trade> trades = new List<Trade>();
                List<CommoditySummarization> commoditySummarizations = new List<CommoditySummarization>();
                List<Position> positions = new List<Position>();
                List<Remittance> remittances = new List<Remittance>();
                List<PositionDetail> positionDetails = new List<PositionDetail>();


                var ds = Regex.Match(text, "(?<=制表时间 Creation Date：).*").Value.Trim();
                date = new DateTime(Int32.Parse(ds.Substring(0, 4)), Int32.Parse(ds.Substring(4, 2)), Int32.Parse(ds.Substring(6, 2)));


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
                var AdditionalMargin = Regex.Match(content, "(?<=应追加资金 Margin Call：).*").Value.Trim();

                var fundStatus = new FundStatus();
                fundStatus.Id = Guid.NewGuid();
                fundStatus.AccountId = accountId;
                fundStatus.SettlementType = settlementType;

                fundStatus.Date = date;
                fundStatus.AdditionalMargin = Decimal.Parse(AdditionalMargin);
                fundStatus.ClosedProfit = Decimal.Parse(ClosedProfit);
                fundStatus.Commission = Decimal.Parse(Commission);
                fundStatus.CustomerRights = Decimal.Parse(CustomerRights);
                fundStatus.FloatingProfit = Decimal.Parse(FloatingProfit);
                fundStatus.FreeMargin = Decimal.Parse(FreeMargin);
                fundStatus.Margin = Decimal.Parse(Margin);
                fundStatus.MatterDeposit = Decimal.Parse(MatterDeposit);
                fundStatus.Remittance = Decimal.Parse(Remittance);
                fundStatus.TodayBalance = Decimal.Parse(TodayBalance);
                fundStatus.VentureFactor = Double.Parse(VentureFactor.Replace('%',' ').Trim())/100;
                fundStatus.YesterdayBalance = Decimal.Parse(YesterdayBalance);


                /* 日期* 入金* 出金* 方式* 摘要
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
                            var date1 = values[1].Trim();
                            var type = values[2].Trim();
                            var deposite = values[3].Trim();
                            var withdrawal = values[4].Trim();
                            var note = values[5].Trim();

                            Remittance rem = new Remittance();
                            rem.Id = Guid.NewGuid();
                            rem.AccountId = accountId;

                            rem.Date = new DateTime(Int32.Parse(date1.Substring(0, 4)), Int32.Parse(date1.Substring(4, 2)), Int32.Parse(date1.Substring(6, 2)));
                            rem.Deposit = Decimal.Parse(deposite);
                            rem.Summary = note;
                            rem.Type = type;
                            rem.WithDrawal = Decimal.Parse(withdrawal);
                            remittances.Add(rem);
                        }
                    }
                }



                /* 持仓汇总
                 * 
                * 持仓日期* 合约* 买持仓* 卖持仓* 买均价* 卖均价* 昨结算价* 今结算价* 持仓盈亏* 交易保证金* 投机套保
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

                            var item = values[2].Trim();
                            var buy = values[3].Trim();
                            var buyPrice = values[4].Trim();
                            var sale = values[5].Trim();
                            var salePrice = values[6].Trim();
                            var yes = values[7].Trim();
                            var today = values[8].Trim();
                            var profit = values[9].Trim();
                            var margin = values[10].Trim();
                            var type = "投机";


                            Position pos = new Position();
                            pos.Id = Guid.NewGuid();
                            pos.Date = date;
                            pos.SettlementType = settlementType;
                            pos.AccountId = accountId;

                            pos.BuyAveragePrice = Decimal.Parse(buyPrice);
                            pos.BuySize = Int32.Parse(buy);
                            pos.Item = item;
                            pos.Margin = Decimal.Parse(margin);
                            pos.Profit = Decimal.Parse(profit);
                            pos.SaleAveragePrice = Decimal.Parse(salePrice);
                            pos.SaleSize = Int32.Parse(sale);
                            pos.SH = type;
                            pos.TodaySettlementPrice = Decimal.Parse(today);
                            pos.YesterdaySettlementPrice = Decimal.Parse(yes);

                            positions.Add(pos);

                        }
                    }
                }



                /* 
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


                decimal totalAmount = 0.0M;
                start = text.IndexOf("成交记录");
                end = text.IndexOf("能源中心");
                if (start > 0) {
                    content = text.Substring(start, end - start);
                    var lines = content.Split(new char[] { '\r', '\n' });
                    foreach (var ll in lines) {
                        if (ll.Contains("投机")) {
                            var values = ll.Split('|');

                            var date1 = values[1].Trim();
                            var item = values[4].Trim();
                            var no = values[14].Trim();
                            var tradeType = values[5].Trim();
                            var type = values[6].Trim();
                            var price = values[7].Trim();
                            var amount = values[8].Trim();
                            var turnover = values[9].Trim();
                            var oc = values[10].Trim();
                            var comm = values[11].Trim();
                            var profit = values[12].Trim();
                            var name = values[3].Trim();

                            /* 成交明细
                             * 
                            * 实际成交时间* 合约* 成交序号* 买卖* 投机套保* 成交价* 手数* 成交额* 开平* 手续费* 平仓盈亏
                            */
                            TradeDetail td = new TradeDetail();
                            td.Id = Guid.NewGuid();
                            td.AccountId = accountId;

                            td.ActualTime = new DateTime(Int32.Parse(date1.Substring(0, 4)), Int32.Parse(date1.Substring(4, 2)), Int32.Parse(date1.Substring(6, 2)));
                            td.Amount = Int32.Parse(amount);
                            td.BS = tradeType;
                            td.ClosedProfit = Decimal.Parse(profit);
                            td.Commission = Decimal.Parse(comm);
                            td.Item = item;
                            td.OC = oc;
                            td.Price = Decimal.Parse(price);
                            td.SH = type;
                            td.Size = Int32.Parse(amount);
                            td.Ticket = no;
                            totalAmount += td.Amount;

                            tradeDetails.Add(td);

                            /* 成交汇总
                             * 
                            * 日期* 合约* 买卖* 投机套保* 成交价* 手数* 成交额* 开平* 手续费* 平仓盈亏
                            */
                            var aa = trades.Find(m => m.Item.Equals(td.Item) && m.Price == td.Price && m.BS.Equals(td.BS));
                            if (aa!=null) {
                                aa.Amount += Decimal.Parse( turnover);
                                aa.ClosedProfit += td.ClosedProfit;
                                aa.Commission += td.Commission;
                                aa.Size += td.Size;
                            } else {
                                Trade trade = new Trade();
                                trade.Id = Guid.NewGuid();
                                trade.Date = td.ActualTime;
                                trade.AccountId = accountId;

                                trade.Item = td.Item;
                                trade.OC = td.OC;
                                trade.Price = td.Price;
                                trade.SH = td.SH;
                                trade.Size = td.Size;
                                trade.Amount = Decimal.Parse(turnover);
                                trade.BS = td.BS;
                                trade.ClosedProfit = td.ClosedProfit;
                                trade.Commission = td.Commission;
                                trades.Add(trade);
                            }

                            /* 平仓明细
                             * 
                            * 实际成交日期* 合约* 成交序号* 买卖* 成交价* 开仓价* 手数* 昨结算价* 平仓盈亏* 原成交序号
                            */
                            if (!td.OC.Equals("开")) {
                                ClosedTradeDetail ctd = new ClosedTradeDetail();
                                ctd.Id = Guid.NewGuid();
                                ctd.ActualDate = td.ActualTime;
                                ctd.AccountId = accountId;

                                ctd.BS = td.BS;
                                ctd.ClosedProfit = td.ClosedProfit;
                                ctd.Item = td.Item;
                                ctd.PriceForClose = td.Price;
                                ctd.PriceForOpen = 0;
                                ctd.Size = td.Size;
                                ctd.TicketForClose = td.Ticket;
                                ctd.TicketForOpen = "";
                                ctd.YesterdaySettlementPrice = 0;
                            }

                            /* 
                            * 品种汇总
                            * 
                            * 日期* 品种* 手数* 成交额* 手续费* 平仓盈亏
                            */
                            var cc = commoditySummarizations.Find(m => m.Commodity.Equals(name));
                            if (cc!=null) {
                                cc.Amount +=Decimal.Parse(turnover);
                                cc.ClosedProfit += td.ClosedProfit;
                                cc.Commission += td.Commission;
                                cc.Size += td.Size;
                            } else {
                                CommoditySummarization cs = new CommoditySummarization();
                                cs.Id = Guid.NewGuid();
                                cs.Date = td.ActualTime;
                                cs.AccountId = accountId;

                                cs.Amount = Decimal.Parse(turnover);
                                cs.ClosedProfit = td.ClosedProfit;
                                cs.Commission = td.Commission;
                                cs.Commodity = name;
                                cs.Size = td.Size;
                                commoditySummarizations.Add(cs);
                            }

                        }
                    }
                }






                /* 
                * 持仓明细
                * 
                * 持仓日期 成交日期 合约 成交序号 买持仓 买入价 卖持仓 卖出价 昨结算价 今结算价 持仓盈亏 投机套保 交易编码
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
                        if (ll.Contains("投机")&&!ll.Contains("Speculation")) {
                            var values = ll.Split('|');

                            var openDate = values[4].Trim();
                            var item = values[3].Trim();
                            var no = "";
                            var bs = values[6].Trim();


                            var buy = "0";
                            var buyPrice = "0";
                            var sale = "0";
                            var salePrice = "0";
                            if (bs.Equals("买")) {
                                buy = values[7].Trim();
                                buyPrice = values[8].Trim();
                            } else {
                                sale = values[7].Trim();
                                salePrice = values[8].Trim();
                            }

                            var yes = values[9].Trim();
                            var today = values[10].Trim();
                            var profit = values[11].Trim();
                            var type = "投机";
                            var bianma = "";


                            PositionDetail pd = new PositionDetail();
                            pd.Id = Guid.NewGuid();
                            pd.SettlementType = settlementType;
                            pd.AccountId = accountId;

                            pd.BuyPrice = Decimal.Parse(buyPrice);
                            pd.BuySize = Int32.Parse(buy);
                            pd.DateForActual = new DateTime(Int32.Parse(openDate.Substring(0, 4)), Int32.Parse(openDate.Substring(4, 2)), Int32.Parse(openDate.Substring(6, 2)));
                            pd.DateForPosition = date;
                            pd.Item = item;
                            pd.Profit = Decimal.Parse(profit);
                            pd.SalePrice = Decimal.Parse(salePrice);
                            pd.SaleSize = Int32.Parse(sale);
                            pd.SH = type;
                            pd.Ticket = no;
                            pd.TodaySettlementPrice = Decimal.Parse(today);
                            pd.TradeCode = bianma;
                            pd.YesterdaySettlementPrice = Decimal.Parse(yes);

                            positionDetails.Add(pd);
                        }
                    }
                }

                var lastStock = statement.Stocks.Where(s => s.AccountId == accountId&&s.Date<date).OrderByDescending(s => s.Date).FirstOrDefault();
                Stock stock = new Stock();
                stock.Id = Guid.NewGuid();
                stock.Date = date.Date;
                stock.Open = lastStock.Close;
                stock.Close = lastStock.Close + (fundStatus.TodayBalance - fundStatus.YesterdayBalance - fundStatus.Remittance);
                close = stock.Close;
                if (stock.Close > stock.Open) {
                    stock.High = stock.Close;
                    stock.Low = stock.Open;
                } else {
                    stock.High = stock.Open;
                    stock.Low = stock.Close;
                }
                stock.Volume = totalAmount;
                stock.AccountId = accountId;

                statement.AddStock(stock);
                statement.AddFundStatus(fundStatus);
                statement.AddTradeDetails(tradeDetails);
                statement.AddTrades(trades);
                statement.AddCommoditySummarizations(commoditySummarizations);
                statement.AddPositions(positions);
                statement.AddRemittances(remittances);
                statement.AddPositionDetails(positionDetails);
            }

            var stocks = statement.Stocks.Where(s => s.AccountId == accountId && s.Date < date).OrderBy(s => s.Date);
            var diff = stocks.FirstOrDefault().Open - close;
            foreach(var st in stocks) {
                st.Open -= diff;
                st.High -= diff;
                st.Low -= diff;
                st.Close -= diff;
            }

            statement.SaveChanged();


            MessageBox.Show("处理完毕！");
        }
    }
}
