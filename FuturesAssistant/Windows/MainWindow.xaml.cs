using FuturesAssistant.Controls;
using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace FuturesAssistant.Windows
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : FuturesAssistant.Windows.WindowBase
    {
        private delegate void FormControlInvoker();
        private object locker = new object();
        LoginWindow loginWindowHandler;
        //private StatementParseWay parseWay = StatementParseWay.excel;

        public MainWindow(LoginWindow loginWindowHandler)
        {
            try
            {
                this.loginWindowHandler = loginWindowHandler;
                InitializeComponent();
                Initialize();

                int width = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
                int height = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

                if (width < 1300)
                {
                    Width = width;
                }
                if (height < 700 - 40)
                {
                    Height = height - 40;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                System.Environment.Exit(0);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SettingButton.Click += delegate
            {
                SettingDialog settingDialog = new SettingDialog(this);
                settingDialog.ShowDialog();
            };
        }

        private void Initialize()
        {
            try
            {
                //
                loginWindowHandler.setProcessText("正在加载配置界面....");
                _settingUserControl.Initialize(this);
                //loginWindowHandler.setProcessText("正在加载账户管理界面....");
                //this._accountManagerUserControl.Initialize(this);
                loginWindowHandler.setProcessText("正在加载手续费数据....");
                //this._exchangeCommissionUserControl.Initialize(false);
                //this.InitializeUserControlsThreadStart(); 

                loginWindowHandler.setProcessText("正在加载图表....");
                _stockChartUserControl.Initialize();
                _statisticsUserControl.Initialize(false);
                loginWindowHandler.setProcessText("正在加载结算单....");
                _statementUserControl.Initialize(false);
                CheckMargin();
                loginWindowHandler.hideProcessMessage();

                //
                Visibility = System.Windows.Visibility.Visible;
                WindowState = WindowState.Normal;
                Activate();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CheckMargin()
        {
            //
            using (StatementContext statement = new StatementContext(typeof(Account), typeof(FundStatus)))
            {
                var accounts = statement.Accounts;
                foreach (var account in accounts)
                {
                    var latestFundStatus = statement.FundStatus.OrderBy(fs => fs.Date).LastOrDefault(fs => fs.AccountId == account.Id);

                    if (latestFundStatus != null && latestFundStatus.AdditionalMargin != 0)
                    {
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            string message = string.Format("日      期： {1}\n\n期货公司： {3}\n\n账      号： {0}\n\n需追加保证金：  {2}。", account.AccountNumber, latestFundStatus.Date.ToString("yyyy年MM月dd日"), latestFundStatus.AdditionalMargin.ToString("C"), account.FuturesCompanyName);
                            MessageBox.Show(message, DateTime.Today.ToString("yyyy年MM月dd日 ddd"));
                            _settingUserControl.AddLog(message);
                        }));
                    }
                }
            }
        }

        /// <summary>
        /// 线程初始化用户控件
        /// </summary>
        public void InitializeUserControlsThreadStart()
        {
            new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    LoadingControlVisible();

                    loginWindowHandler.setProcessText("正在加载图表....");
                    _stockChartUserControl.Initialize();
                    _statisticsUserControl.Initialize();
                    loginWindowHandler.setProcessText("正在加载结算单....");
                    _statementUserControl.Initialize();
                    CheckMargin();
                    loginWindowHandler.hideProcessMessage();

                    LoadingControlHide();
                }));
            })).Start();
        }

        private void _tabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_tabControlMain.SelectedItem == _tabItem资金曲线 && _Session.NeedRefreshStockControl)
                {
                    //
                    LoadingControlVisible();
                    _stockChartUserControl.Initialize();
                    LoadingControlHide();

                    //
                    _Session.NeedRefreshStockControl = false;
                }
                else if (_tabControlMain.SelectedItem == _tabItem统计图表 && _Session.NeedRefreshStatisticsControl)
                {
                    //
                    LoadingControlVisible();
                    _statisticsUserControl.Initialize();
                    LoadingControlHide();

                    //
                    _Session.NeedRefreshStatisticsControl = false;
                }
                else if (_tabControlMain.SelectedItem == _tabItem客户结单 && _Session.NeedRefreshStatementControl)
                {
                    //
                    LoadingControlVisible();
                    _statementUserControl.Initialize();
                    LoadingControlHide();

                    //
                    _Session.NeedRefreshStatementControl = false;
                }
                if (_Session.NeedRefreshMarginControl)
                {
                    //
                    //CheckMargin();

                    //
                    _Session.NeedRefreshMarginControl = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void LoadingControlHide()
        {
            //this.Dispatcher.Invoke(new FormControlInvoker(() =>
            //{
            _mainWindowLoading.Visibility = System.Windows.Visibility.Hidden;
            _mainWindowLoading._Refresh();
            //}));
        }

        private void LoadingControlVisible()
        {
            //this.Dispatcher.Invoke(new FormControlInvoker(() =>
            //{
            _mainWindowLoading.Visibility = System.Windows.Visibility.Visible;
            _mainWindowLoading._Refresh();
            //}));
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void _combox账户列表_SelectionChanged_link()
        {
            _combox账户列表.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(_combox账户列表_SelectionChanged);
        }
        public void _combox账户列表_SelectionChanged_unlink()
        {
            _combox账户列表.SelectionChanged -= new System.Windows.Controls.SelectionChangedEventHandler(_combox账户列表_SelectionChanged);
        }

        private void _combox账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_combox账户列表.SelectedItem != null)
            {
                var tm = _combox账户列表.SelectedItem as AccountListModel;

                //
                using (StatementContext statement = new StatementContext(typeof(Account)))
                {
                    // 更新_Session.CurrentAccountId。 
                    //      注“合并账户显示”项的Id为Guid.Empty。
                    Account acc = statement.Accounts.FirstOrDefault(m => m.Id == tm.Id);
                    var selectedAccount = statement.Accounts.FirstOrDefault(model => model.Id.Equals(acc.Id));
                    if (selectedAccount != null)
                    {
                        _Session.SelectedAccountId = selectedAccount.Id;
                    }
                    else
                    {
                        _Session.SelectedAccountId = Guid.Empty;
                    }

                    //
                    if (_Session.SelectedAccountId == Guid.Parse(_Helper.GetParameter(ParameterName.DefaultAccountId.ToString(), _Session.SelectedAccountId.ToString())))
                    {
                        _button默认账户.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    else
                    {
                        _button默认账户.Visibility = System.Windows.Visibility.Visible;
                    }
                }

                // 重新初始化数据控件
                InitializeUserControlsThreadStart();
            }
        }

        private void _button默认账户_Click(object sender, RoutedEventArgs e)
        {
            _Helper.SetParameter(ParameterName.DefaultAccountId.ToString(), _Session.SelectedAccountId.ToString());
            _button默认账户.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void _button更新数据_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //
                _button更新数据.Visibility = Visibility.Collapsed;
                _button更新数据._Refresh();
                //this.parseWay = StatementParseWay.html;
                //var tm = this._combox账户列表.SelectedItem as AccountListModel;
                //using (StatementContext statement = new StatementContext(typeof(Account)))
                //{
                //    // 更新_Session.CurrentAccountId。 
                //    //      注“合并账户显示”项的Id为Guid.Empty。
                //    Account acc = statement.Accounts.FirstOrDefault(m => m.Id == tm.Id);
                //    if (acc.Type == 2)
                //    {
                //        //AddCooperateDataDialog acdd = new AddCooperateDataDialog(tm.Id);
                //        ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(tm.Id, this);
                //        acdd.Owner = this;
                //        acdd.ShowDialog();
                //    }
                //    else
                //    {
                LoadAllStatements();
                //    }

                //}
                _button更新数据.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                File.AppendAllText(_Session.DatabaseDirPath + "log.txt", ex.Message + "\n" + ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
        }

        private void _button更新数据_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //
                _button更新数据.Visibility = Visibility.Collapsed;
                _button更新数据._Refresh();
                //this.parseWay = StatementParseWay.html;
                var tm = _combox账户列表.SelectedItem as AccountListModel;
                using (StatementContext statement = new StatementContext(typeof(Account)))
                {
                    // 更新_Session.CurrentAccountId。 
                    //      注“合并账户显示”项的Id为Guid.Empty。
                    Account acc = statement.Accounts.FirstOrDefault(m => m.Id == tm.Id);
                    if (acc.Type == 2)
                    {
                        AddCooperateDataDialog acdd = new AddCooperateDataDialog(tm.Id);
                        //ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(tm.Id, this);
                        acdd.Owner = this;
                        acdd.ShowDialog();
                    }
                    else
                    {
                        LoadAllStatements();
                    }

                }
                _button更新数据.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        public void ReflushStocks()
        {
            _stockChartUserControl.Initialize();
        }

        private void LoadAllStatements()
        {
            using (StatementContext statement = new StatementContext(typeof(FundStatus), typeof(Account)))
            {
                bool loaded = false; // 判断是否有账户从监控中心更新了数据
                var accountList = statement.Accounts.Where(m => m.UserId == _Session.LoginedUserId && (m.Type == 1 || m.Type == 10));
                foreach (var account in accountList)
                {
                    try
                    {
                        var lastest = statement.FundStatus.Where(fs => fs.AccountId == account.Id).OrderByDescending(fs => fs.Date).FirstOrDefault();
                        if (!account.IsAllowLoad)
                        {
                            //if (lastest != null && (DateTime.Now.Date - lastest.Date.Date).TotalDays <= 60)
                                if (lastest != null )
                                    continue;
                        }
                        if (lastest != null && lastest.Date.Date >= DateTime.Now.Date)
                        {
                            continue;
                        }
                        loaded = true;
                        LoginCfmmcDialog lcf = new LoginCfmmcDialog(account);
                        lcf.Owner = this;
                        lcf.ShowDialog();
                        if (lcf.DialogResult.Value)
                        {
                            Thread th = new Thread(new ThreadStart(() => LoadStatementFromCFMMC(lcf.Cookie, account.Id)));
                            th.IsBackground = true;
                            th.Start();
                        }
                        //else
                        //{
                        //    break;
                        //}
                    }
                    catch (IOException)
                    {
                        MessageBox.Show("因网络IO问题，程序将中断运行。");
                        break;
                    }
                    catch (WebException)
                    {
                        MessageBox.Show("因网络问题，程序将中断运行。");
                        break;
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                //
                if (!loaded)
                {
                    MessageBox.Show("当前用户的所有允许更新账户数据已是最新，无需更新！");
                }
            }
        }

        public void ProgressInfoVisible(int progressBarMaximum)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _textBlock提醒信息.Text = "";
                _progressBar进度1.Maximum = progressBarMaximum;
                _textBlock提醒信息.Visibility = System.Windows.Visibility.Visible;
                _progressBar进度1.Visibility = System.Windows.Visibility.Visible;
                _progressBar进度1.Value = 0;
            }));
        }

        public void ProgressInfoHidden()
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _textBlock提醒信息.Text = "";
                _textBlock提醒信息.Visibility = System.Windows.Visibility.Hidden;
                _progressBar进度1.Visibility = System.Windows.Visibility.Hidden;
                _progressBar进度1.Value = 0;
            }));
        }

        public void SetProgressText(string message)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _textBlock提醒信息.Text = message;
            }));
        }
        public void AddProgressValue(int addValue)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _progressBar进度1.Value += addValue;
            }));
        }

        private void LoadStatementFromCFMMC(CookieContainer cookie, Guid accountId)
        {
            StatementParseWay parseWay = StatementParseWay.excel;
            lock (locker)
            {
                //
                StatementContext statement = new StatementContext();
                Account account = statement.Accounts.FirstOrDefault(model => model.Id == accountId);

                //try
                //{
                DateTime startDate = DateTime.Now.AddDays(-186).Date;
                DateTime endDate = DateTime.Now.Date;

                decimal close = 0;
                decimal? yesterdayBalance, todayBalance, remittance, amount;
                int dataCount = 0;
                FundStatus lastFundStatus;
                Stock lastStock;
                //
                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadCommitted;
                transactionOptions.Timeout = new TimeSpan(0, 30, 0);
                //using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
                //{



                lastStock = statement.Stocks.Where(s => s.AccountId == account.Id).OrderByDescending(s => s.Date).FirstOrDefault();
                if (lastStock == null)
                {
                    ProgressInfoVisible((endDate - startDate).Days + 1);
                    //
                    bool firstLine = true;
                    //
                    while (startDate <= endDate)
                    {
                        SetProgressText(string.Concat("【", account.AccountNumber, "】  ", startDate.ToString("yyyy年MM月dd日")));

                        if (parseWay == StatementParseWay.html)
                            _Helper.GetStatementByRequestHtml(cookie, startDate, SettlementType.date, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, account.Id);
                        else
                            _Helper.GetStatementByLoadExcel(cookie, startDate, SettlementType.date, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, account.Id);

                        if (yesterdayBalance.HasValue && todayBalance.HasValue && remittance.HasValue && amount.HasValue)
                        {
                            Stock stock = new Stock();
                            stock.Id = Guid.NewGuid();
                            if (firstLine)
                            {
                                stock.Date = startDate.Date;
                                stock.Open = yesterdayBalance.Value + remittance.Value;
                                stock.Close = close = todayBalance.Value;
                                if (stock.Close > stock.Open)
                                {
                                    stock.High = stock.Close;
                                    stock.Low = stock.Open;
                                }
                                else
                                {
                                    stock.High = stock.Open;
                                    stock.Low = stock.Close;
                                }
                                stock.Volume = amount.Value;
                                stock.AccountId = account.Id;
                                firstLine = false;
                            }
                            else
                            {
                                stock.Date = startDate.Date;
                                stock.Open = close;
                                stock.Close = close += todayBalance.Value - yesterdayBalance.Value - remittance.Value;
                                if (stock.Close > stock.Open)
                                {
                                    stock.High = stock.Close;
                                    stock.Low = stock.Open;
                                }
                                else
                                {
                                    stock.High = stock.Open;
                                    stock.Low = stock.Close;
                                }
                                stock.Volume = amount.Value;
                                stock.AccountId = account.Id;
                            }
                            statement.AddStock(stock);
                            dataCount++;
                        }
                        //
                        startDate = startDate.AddDays(1);
                        AddProgressValue(1);
                    }
                }
                else
                {
                    startDate = lastStock.Date.AddDays(1).Date;
                    //if ((endDate - startDate).Days > 365)
                    //{
                    //    endDate = startDate.AddDays(365);
                    //}


                    ProgressInfoVisible((endDate - startDate).Days + 1);
                    //
                    close = lastStock.Close;
                    while (startDate <= endDate)
                    {
                        SetProgressText(string.Concat("【", account.AccountNumber, "】  ", startDate.ToString("yyyy年MM月dd日")));

                        if (parseWay == StatementParseWay.html)
                            _Helper.GetStatementByRequestHtml(cookie, startDate, SettlementType.date, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, account.Id);
                        else
                            _Helper.GetStatementByLoadExcel(cookie, startDate, SettlementType.date, statement, out yesterdayBalance, out todayBalance, out remittance, out amount, account.Id);

                        if (yesterdayBalance.HasValue && todayBalance.HasValue && remittance.HasValue && amount.HasValue)
                        {
                            Stock stock = new Stock();
                            stock.Id = Guid.NewGuid();
                            stock.Date = startDate.Date;
                            stock.Open = close;
                            stock.Close = close += todayBalance.Value - yesterdayBalance.Value - remittance.Value;
                            if (stock.Close > stock.Open)
                            {
                                stock.High = stock.Close;
                                stock.Low = stock.Open;
                            }
                            else
                            {
                                stock.High = stock.Open;
                                stock.Low = stock.Close;
                            }
                            stock.Volume = amount.Value;
                            stock.AccountId = account.Id;
                            statement.AddStock(stock);
                            dataCount++;
                        }
                        //
                        startDate = startDate.AddDays(1);
                        AddProgressValue(1);
                    }
                }


                // 复权处理
                var stocks = statement.Stocks.Where(s => s.AccountId == account.Id).OrderBy(s => s.Date);
                lastStock = stocks.LastOrDefault();
                lastFundStatus = statement.FundStatus.Where(fs => fs.AccountId == account.Id).OrderBy(fs => fs.Date).LastOrDefault();
                if (lastStock != null && lastFundStatus != null)
                {
                    decimal tmp = lastStock.Close - lastFundStatus.TodayBalance;
                    if (tmp != 0)
                    {
                        SetProgressText(string.Format("<{0}>图表复权...", account.AccountNumber));
                        foreach (var stock in stocks)
                        {
                            stock.Open -= tmp;
                            stock.High -= tmp;
                            stock.Low -= tmp;
                            stock.Close -= tmp;
                        }
                        statement.UpdateStocks(stocks);
                    }
                }

                //
                SetProgressText(string.Format("<{0}>保存数据...", account.AccountNumber));
                statement.SaveChanged();


                if (dataCount > 0)
                {
                    InitializeUserControlsThreadStart();
                }
                _settingUserControl.AddLog(string.Format("账户<{0}>下载数据完毕，新增 {1} 天数据。", account.AccountNumber, dataCount));
                ProgressInfoHidden();
                //}
                //catch (Exception)
                //{
                //    throw;
                //}
                //finally
                //{
                //    this.ProgressInfoHidden();
                //}


                // 备份数据至邮箱。
                //MailYunBackup(true);
            }
        }

        private void MailYunBackup(bool isBackground)
        {
            using (StatementContext statement = new StatementContext(typeof(User)))
            {
                var user = statement.Users.FirstOrDefault(model => model.Id.Equals(_Session.LoginedUserId));
                //
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

        private void _combox账户列表_DragEnter(object sender, DragEventArgs e)
        {
            //if (e.Data.GetDataPresent(DataFormats.FileDrop))
            //    e.Effects = DragDropEffects.Link;
            //else
            //    e.Effects = DragDropEffects.None;
        }

        private void _combox账户列表_Drop(object sender, DragEventArgs e)
        {
            var tm = _combox账户列表.SelectedItem as AccountListModel;
            if (tm.Type == 2 || tm.Type == 20)
            {
                ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(tm.Id, this);

                var files = ((System.Array)e.Data.GetData(DataFormats.FileDrop));
                acdd.ImportFiles(files);

            }
        }

    }
}
