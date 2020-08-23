using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Types;
using FuturesAssistantWPF.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
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

namespace FuturesAssistantWPF.Controls
{
    public class AccountListModel
    {
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
    public partial class SettingUserControl : UserControl
    {
        private delegate void FormControlInvoker();
        private BinaryFormatter formatter = new BinaryFormatter();
        private object locker = new object();
        private MainWindow mainWindow;

        public SettingUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
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
                                AddLog(string.Format("不允许从<{0}>向<{1}>导入数据，导入数据必须与当前选定账户匹配！", wh6Info.AccountNumber, currentAccount.AccountNumber));
                                OverStatusAndProgressBar();
                                return;
                            }
                        }
                        else
                        {
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

                            AddLog(string.Format("从文华6月结单导入数据完毕！", currentAccount.AccountNumber));
                            OverStatusAndProgressBar();
                        }
                    }
                    else
                    {
                        AddLog(string.Format("本地没有<{0}>的数据，请先从保证金监控中心下载数据，然后再手动导入数据！", currentAccount.AccountNumber));
                        OverStatusAndProgressBar();
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
                OverStatusAndProgressBar();
            }
        }

        /// <summary>
        /// 初始化状态条和进度条，并显示。
        /// </summary>
        /// <param name="progressMaxValue">进度条最大值</param>
        private void InitialStatusAndProgressBar(int progressMaxValue)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
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
        private void UpdateStatusAndProgressBar(string text = "", int progressBarValueIncrease = 0)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                if (!string.IsNullOrEmpty(text))
                {
                    _textBlock实时状态.Text = text;
                    _textBlock实时状态._Refresh();
                }
                if (progressBarValueIncrease != 0)
                {
                    if (_progressBar进度.Value + progressBarValueIncrease > _progressBar进度.Maximum)
                    {
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
        private void OverStatusAndProgressBar()
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _textBlock实时状态.Text = "";
                _textBlock实时状态.Visibility = Visibility.Collapsed;
                _progressBar进度.Visibility = Visibility.Collapsed;
                _textBlock实时状态._Refresh();
                _progressBar进度._Refresh();
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                return null;
            }
        }

        public void AddLog(string message)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _listBox运行日志.Items.Add(string.Format("{0}：{1}", DateTime.Now, message));
                _listBox运行日志._Refresh();
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
            _checkBox自动登录.IsChecked = false;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString());
        }

        private void _checkBox记住密码_Checked(object sender, RoutedEventArgs e)
        {
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Checked(object sender, RoutedEventArgs e)
        {
            _checkBox记住密码.IsChecked = true;
            _Helper.SetParameter(ParameterName.RememberUserPassword.ToString(), true.ToString());
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), true.ToString());
        }

        private void _checkBox自动登录_Unchecked(object sender, RoutedEventArgs e)
        {
            _Helper.SetParameter(ParameterName.AutoLogin.ToString(), false.ToString());
        }

        private void _btn允许下载_Click(object sender, RoutedEventArgs e)
        {
        }
        private const string ALLOW_LOAD = "禁止更新";
        private const string NOT_ALLOW_LOAD = "允许更新";
        private const string ADD_COOPERATE = "修改账户";
        private const string DISUSE_ACCOUNT = "禁用账户";
        private const string RESUME_ACCOUNT = "使用账户";

        private void _btn删除数据_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_datePicker删除开始时间.SelectedDate.HasValue)
                {
                    var date = _datePicker删除开始时间.SelectedDate.Value.Date;
                    StatementContext context = new StatementContext(typeof(Account));
                    if (MessageBox.Show(string.Concat("确认要删除账户【",
                        context.Accounts.FirstOrDefault(m => m.Id == _Session.SelectedAccountId).AccountNumber,
                        "】",
                        _datePicker删除开始时间.SelectedDate.Value.Date.ToString("yyyy年MM月dd日"), "以及之后的数据吗？"), "删除数据确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        DeleteData(date);
                        //Thread mythread2 = new Thread(() => DeleteData(date));
                        //mythread2.IsBackground = true;
                        ////設置為後臺線程,程式關閉后進程也關閉,如果不設置true，則程式關閉,此線程還在內存,不會關閉           
                        //mythread2.Start();
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
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DeleteData(DateTime date)
        {

            try
            {
                mainWindow.ProgressInfoVisible(3);
                using (StatementContext statement = new StatementContext())
                {
                    mainWindow.SetProgressText("(1/3)正在删除数据...");
                    statement.Delete(date, _Session.SelectedAccountId, typeof(ClosedTradeDetail), typeof(CommoditySummarization), typeof(FundStatus), typeof(PositionDetail), typeof(Position), typeof(Remittance), typeof(Stock), typeof(TradeDetail), typeof(Trade));
                    mainWindow.AddProgressValue(1);

                    mainWindow.SetProgressText("(2/3)正在校准权益...");
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
            }
            catch (Exception ex)
            {
                OverStatusAndProgressBar();
                MessageBoxSync(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                mainWindow.ProgressInfoHidden();
            }
        }


        public void MessageBoxSync(string message)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
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


        private void _btnFromEmail_Click(object sender, RoutedEventArgs e)
        {
            _btnFromEmail.IsEnabled = false;
            System.Windows.Forms.OpenFileDialog fd = new System.Windows.Forms.OpenFileDialog();
            fd.Multiselect = false;
            fd.Filter = "备份文件(*.bak)|*.bak";
            fd.FilterIndex = 1;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                File.Copy(fd.FileName, _Session.DatabaseFilePath, true);
                mainWindow.InitializeUserControlsThreadStart();
                MessageBox.Show("数据已恢复！");
            }
            _btnFromEmail.IsEnabled = true;
        }

        private void _btnToEmail_Click(object sender, RoutedEventArgs e)
        {
            Thread th = new Thread(new ThreadStart(() => MailYunBackup(false)));
            th.IsBackground = true;
            th.Start();
        }

        private void MailYunBackup(bool isBackground)
        {
            using (StatementContext statement = new StatementContext(typeof(User)))
            {
                string text = null;

                if (!isBackground)
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        text = _btnToEmail.Content.ToString();
                    }));
                var user = statement.Users.FirstOrDefault(model => model.Id.Equals(_Session.LoginedUserId));
                //
                MessageBoxResult result = MessageBoxResult.Cancel;
                if (!isBackground)
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        result = MessageBox.Show(string.Concat("将数据文件备份至【", user.Email, "】，确认后继续操作！"), "备份确认", MessageBoxButton.OKCancel);
                    }));
                else
                    result = MessageBoxResult.OK;
                if (result == MessageBoxResult.OK)
                {
                    if (!isBackground)
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _btnToEmail.Content = "正在上传...";
                        _btnToEmail.IsEnabled = false;
                    }));
                    AddLog("正在向邮箱上传数据...");
                    //确定smtp服务器地址。实例化一个Smtp客户端
                    SmtpClient client = new SmtpClient("smtp.163.com");

                    //构造一个发件人地址对象
                    MailAddress from = new MailAddress("outlook163@163.com", "期货助手", Encoding.UTF8);
                    //构造一个收件人地址对象
                    MailAddress to = new MailAddress(user.Email, user.UserName, Encoding.UTF8);

                    //构造一个Email的Message对
                    MailMessage message = new MailMessage(from, to);


                    //添加邮件主题和内容
                    message.Subject = string.Concat("期货助手 - 备份文件(", _Session.LatestFundStatusDate.ToShortDateString(), ")");
                    message.SubjectEncoding = Encoding.UTF8;
                    //message.Body = string.Concat("使用方法：将附件中的文件覆盖原数据文件，即可恢复数据！");
                    message.BodyEncoding = Encoding.UTF8;
                    string bakFile = _Session.DatabaseFilePath.Replace(".db", "") + ".bak";
                    File.Copy(_Session.DatabaseFilePath, bakFile, true);
                    message.Attachments.Add(new Attachment(bakFile));


                    //设置邮件的信息                
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.Timeout = 1000 * 60 * 15;
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
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _btnToEmail.Content = text;
                        _btnToEmail.IsEnabled = true;
                    }));
                    message.Dispose();
                    client.Dispose();
                    File.Delete(bakFile);

                    if (!isBackground)
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        MessageBox.Show(string.Concat("数据文件已备份至【", user.Email, "】。"));
                    }));
                }
            }
        }

        private void _btn交易品种_Click(object sender, RoutedEventArgs e)
        {
            CommodityDialog cd = new CommodityDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(cd)
                {
                    Owner = winformWindow.Handle
                };
            }
            cd.ShowDialog();
        }

        private void _btn查询手续费_Click(object sender, RoutedEventArgs e)
        {
            CommissionDialog dialog = new CommissionDialog();
            dialog.ShowDialog();
        }

        private void _btnOpenDbDir_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", "/select," + _Session.DatabaseFilePath);
        }


        public void InitializeAccountList()
        {
            using (StatementContext context = new StatementContext(typeof(Stock), typeof(Account), typeof(Commodity), typeof(FundStatus)))
            {
                List<AccountListModel> accountListMain = new List<AccountListModel>();
                List<AccountListModel> accountListSettingJY = new List<AccountListModel>();
                List<AccountListModel> accountListSettingPZ = new List<AccountListModel>();
                //context.Accounts;
                foreach (var acc in context.Accounts.OrderByDescending(m => m.Type))
                {
                    var firstStock = context.Stocks.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().FirstOrDefault();
                    var lastStock = context.Stocks.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().LastOrDefault();

                    // 非隐藏账户
                    if (acc.Type % 10 != 0)
                    {
                        accountListMain.Add(new AccountListModel()
                        {
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
                    if (acc.Type == 2 || acc.Type == 20)
                    {
                        accountListSettingPZ.Add(new AccountListModel()
                        {
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
                    }
                    else // 期货公司交易账户
                    {
                        accountListSettingJY.Add(new AccountListModel()
                        {
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
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    mainWindow._combox账户列表_SelectionChanged_unlink();
                    mainWindow._combox账户列表.ItemsSource = accountListMain;
                    mainWindow._combox账户列表.SelectedItem = accountListMain.FirstOrDefault(m => m.Id == _Session.SelectedAccountId);
                    mainWindow._combox账户列表_SelectionChanged_link();
                    _listBox交易账户列表.ItemsSource = accountListSettingJY.OrderByDescending(m => m.LatestDataDate).ToList();
                }));
            }
        }

        private void _button添加账户_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow aaf = new AddAccountWindow();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(aaf)
                {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
            {
                using (StatementContext statement = new StatementContext(typeof(Account)))
                {
                    InitializeAccountList();
                }
            }
        }

        private void _button添加配资账户_Click(object sender, RoutedEventArgs e)
        {
            AddCooperateAccountDialog aaf = new AddCooperateAccountDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(aaf)
                {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
            {
                InitializeAccountList();
            }
        }

        private void _btn是否下载_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _listBox交易账户列表_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (selectedAccountItem == null || (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10))
            {
                _menuItem列表隐藏.IsEnabled = false;
                _menuItem列表显示.IsEnabled = false;
                _menuItem允许下载.IsEnabled = false;
                _menuItem禁止下载.IsEnabled = false;
                return;
            }
            else
            {
                _menuItem列表隐藏.IsEnabled = true;
                _menuItem列表显示.IsEnabled = true;
                _menuItem允许下载.IsEnabled = true;
                _menuItem禁止下载.IsEnabled = true;
            }

            // 列表中不显示
            if (selectedAccountItem.Type % 10 == 0)
            {
                _menuItem列表隐藏.Visibility = Visibility.Collapsed;
                _menuItem列表显示.Visibility = Visibility.Visible;
            }
            else
            {
                _menuItem列表隐藏.Visibility = Visibility.Visible;
                _menuItem列表显示.Visibility = Visibility.Collapsed;
            }

            // 跟随下载
            if (selectedAccountItem.IsAllowLoad)
            {
                _menuItem允许下载.Visibility = Visibility.Collapsed;
                _menuItem禁止下载.Visibility = Visibility.Visible;
            }
            else
            {
                _menuItem允许下载.Visibility = Visibility.Visible;
                _menuItem禁止下载.Visibility = Visibility.Collapsed;
            }
        }

        private void _menuItem列表显示_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext(typeof(Account)))
            {
                var account = statement.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type / 10;
                statement.EditAccount(account);
                statement.SaveChanged();
                InitializeAccountList();
            }
        }

        private void _menuItem列表隐藏_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext(typeof(Account)))
            {
                var account = statement.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type * 10;
                statement.EditAccount(account);
                statement.SaveChanged();
                InitializeAccountList();
            }
        }

        private void _menuItem修改账户_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            EditCooperateAccountDialog acdd = new EditCooperateAccountDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(acdd)
                {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
            InitializeAccountList();
        }

        private void _menuItem允许下载_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext(typeof(Account)))
            {
                Account acc = context.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.EditAccount(acc);
                context.SaveChanged();
                if (!acc.IsAllowLoad)
                {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        private void _menuItem禁止下载_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext(typeof(Account)))
            {
                Account acc = context.Accounts.FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.EditAccount(acc);
                context.SaveChanged();
                if (!acc.IsAllowLoad)
                {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        AccountListModel selectedAccountItem = null;

        private void _listBox交易账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = _listBox交易账户列表.SelectedItem;
            if (item == null)
                selectedAccountItem = null;
            else
                selectedAccountItem = item as AccountListModel;
        }

        private void _menuItemP查看数据_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(acdd)
                {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
        }

        private void _menuItemP自动填充数据_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _menuItem添加账户_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow aaf = new AddAccountWindow();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(aaf)
                {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
            {
                using (StatementContext statement = new StatementContext(typeof(Account)))
                {
                    InitializeAccountList();
                }
            }
        }
    }
}
