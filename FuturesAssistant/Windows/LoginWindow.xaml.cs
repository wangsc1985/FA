using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Types;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FuturesAssistant.Windows
{
    /// <summary>
    /// LoginWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginWindow : Window
    {
        private delegate void FormControlInvoker();

        public LoginWindow()
        {
            try
            {
                InitializeComponent();
                Visibility = System.Windows.Visibility.Visible;
                WindowState = WindowState.Normal;
                Activate();

                if (_Session.IsT)
                {
                    try
                    {
                        DateTime now = _Helper.GetNetDateTime();
                        var have = now - _Session.testStartDate;
                        var rev = _Session.testStartDate.AddDays(1) - now;
                        if (rev.TotalSeconds < 0)
                        {
                            MessageBox.Show("试用期已结束，请购买正式版。\n\n联系QQ：645708679（微信同号）");
                            Close();
                        }
                        else
                        {
                            MessageBox.Show(string.Concat("试用剩余", rev.Days, "天", rev.Hours, "小时", rev.Minutes, "分", rev.Seconds, "秒"));
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("试用版必须在联网状态下启动，请保持网络畅通。");
                        Close();
                    }
                }
                //
                Initialize();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                Close();
            }
        }

        private void Initialize()
        {
            //
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    ListAccountNames();
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _textProcessMessage.Visibility = System.Windows.Visibility.Hidden;
                        Storyboard story = Resources["OnLoadedStoryboard"] as Storyboard;
                        story.Begin();
                    }));
                }
                catch (Exception)
                {
                    throw;
                }
            })).Start();
        }
        /// <summary>
        /// 向资金账号列表中添加数据
        /// </summary>
        private void ListAccountNames()
        {
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    //this._loadingControl.Visibility = System.Windows.Visibility.Visible;
                    _comboBox用户列表.Items.Clear();
                    this._Refresh();
                }));
                //
                using (StatementContext statement = new StatementContext(typeof(User)))
                {
                    var users = statement.Users;
                    foreach (var user in users)
                    {
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            _comboBox用户列表.Items.Add(user.UserName);
                        }));
                    }
                }
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _comboBox用户列表.SelectedIndex = 0;
                }));

                // 自动数据密码
                InputUserPassword();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _textBoxPassword.Focus();
        }

        public void _button登陆_Click(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    Storyboard story = Resources["OnLoginBtnClick"] as Storyboard;
                    story.Begin();
                    //this._groupBoxLogin.Visibility = System.Windows.Visibility.Hidden;
                    //this._groupBoxLogin._Refresh();
                    //this.groupBox.Margin = new Thickness(250, 0, 0, 0);
                    //this.groupBox._Refresh();
                }));
            })).Start();
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(300);
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    setProcessText("正在登陆...");
                    Login();
                }));
            })).Start();
        }

        private void Login()
        {
            //this._loadingControl.Visibility = System.Windows.Visibility.Visible;
            //this._loadingControl._Refresh();
            try
            {
                if (string.IsNullOrEmpty(_comboBox用户列表.SelectedValue.ToString().Trim()))
                {
                    MessageBox.Show("请先选定用户！");
                    return;
                }
                if (string.IsNullOrEmpty(_textBoxPassword.Password.Trim()))
                {
                    MessageBox.Show("密码不能为空！");
                    return;
                }
                using (StatementContext statement = new StatementContext(typeof(User), typeof(Account)))
                {
                    string userName = _comboBox用户列表.SelectedValue.ToString().Trim();
                    var users = statement.Users.Where(o => o.UserName.Trim().Equals(userName));

                    if (users.Count() == 0)
                    {
                        MessageBox.Show(string.Format("数据库错误：本地数据库中不存在用户“{0}”，请联系管理员！", _comboBox用户列表.Text.Trim()));
                    }
                    else if (users.Count() != 1)
                    {
                        MessageBox.Show(string.Format("数据库错误：用户“{0}”在数据库中出现重复数据，请联系管理员！", _comboBox用户列表.Text.Trim()));
                    }
                    else
                    {
                        var user = users.FirstOrDefault();
                        var passwordServer = user.UserPassword._RSADecrypt();
                        var passwordClient = _textBoxPassword.Password.Trim();
                        if (!passwordServer.Equals(passwordClient))
                        {
                            MessageBox.Show("您输入的密码不正确，请重新输入！");
                            _textBoxPassword.Password = "";
                            _textBoxPassword.Focus();
                            _textProcessMessage.Visibility = System.Windows.Visibility.Hidden;
                            Storyboard story = Resources["OnLoadedStoryboard"] as Storyboard;
                            story.Begin();
                        }
                        else
                        {
                            // 设置会话中的UserId
                            _Session.LoginedUserId = user.Id;

                            //
                            if (statement.Accounts.FirstOrDefault(acc => acc.UserId == user.Id) == null)
                            {
                                if (MessageBox.Show(this, "当前用户没有资金账户，是否添加一个资金账户？", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                                {
                                    AddAccountWindow aaf = new AddAccountWindow();
                                    aaf.Owner = this;
                                    aaf.ShowDialog();
                                    //
                                    if (aaf.DialogResult.Value)
                                    {
                                        _Session.SelectedAccountId = Guid.Parse(_Helper.GetParameter(ParameterName.DefaultAccountId.ToString(), new StatementContext(typeof(Account)).Accounts.FirstOrDefault().Id.ToString()));
                                    }
                                    else
                                    {
                                        Show();
                                        return;
                                    }
                                }
                                else
                                {
                                    Show();
                                    return;
                                }
                            }
                            else
                            {
                                _Session.LoginedUserId = user.Id;
                                _Session.SelectedAccountId = Guid.Parse(_Helper.GetParameter(ParameterName.DefaultAccountId.ToString(), statement.Accounts.FirstOrDefault().Id.ToString()));
                            }

                            MainWindow mw = new MainWindow(this);
                            Hide();
                            mw.ShowDialog();
                            Close();
                            System.Environment.Exit(0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                //this._loadingControl.Visibility = System.Windows.Visibility.Collapsed;
                _textProcessMessage.Visibility = System.Windows.Visibility.Hidden;
                Storyboard story = Resources["OnLoadedStoryboard"] as Storyboard;
                story.Begin();
            }
        }

        public void hideProcessMessage()
        {
            _textProcessMessage.Visibility = System.Windows.Visibility.Hidden;
        }
        public void setProcessText(string text)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _textProcessMessage.Visibility = System.Windows.Visibility.Visible;
                _textProcessMessage.Text = text;
                _textProcessMessage._Refresh();
            }));
        }

        private void _linkLabel添加用户_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _linkLabel添加用户.Foreground = new SolidColorBrush(Colors.Gray);
            _linkLabel添加用户._Refresh();
            AddUserDialog aaf = new AddUserDialog();
            aaf.Owner = this;
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
                ListAccountNames();
            //
            _linkLabel添加用户.Foreground = new SolidColorBrush(Colors.Blue);


            //this._linkLabel添加资金账号.Foreground = new SolidColorBrush(Colors.Gray);
            //this._linkLabel添加资金账号.Refresh();
            //AddAccountWindow aaf = new AddAccountWindow();
            //aaf.Owner = this;
            //aaf.ShowDialog();
            ////
            //if (aaf.OneAccountAdded)
            //    ListAccountNames();
            ////
            //this._linkLabel添加资金账号.Foreground = new SolidColorBrush(Colors.Blue);
        }

        private void _linkLabel忘记密码_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                using (StatementContext statement = new StatementContext(typeof(User)))
                {
                    var user = statement.Users.FirstOrDefault(model => model.UserName.Equals(_comboBox用户列表.Text.Trim()));
                    //
                    if (MessageBox.Show(string.Format("是否向用户<{0}>的密保邮箱发送新密码？", user.UserName), "发送新密码确认", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //确定smtp服务器地址。实例化一个Smtp客户端
                        SmtpClient client = new SmtpClient("smtp.163.com");

                        //构造一个发件人地址对象
                        MailAddress from = new MailAddress("outlook163@163.com", "期货助手", Encoding.UTF8);
                        //构造一个收件人地址对象
                        MailAddress to = new MailAddress(user.Email, _comboBox用户列表.Text.Trim(), Encoding.UTF8);

                        //构造一个Email的Message对
                        MailMessage message = new MailMessage(from, to);

                        Random random = new Random();
                        string newPassword = string.Format("{0}{1}{2}{3}{4}{5}", random.Next(10), random.Next(10), random.Next(10), random.Next(10), random.Next(10), random.Next(10));



                        //添加邮件主题和内容
                        message.Subject = "期货助手 - 新密码";
                        message.SubjectEncoding = Encoding.UTF8;
                        message.Body = string.Format("您的新密码是：{0}", newPassword);
                        message.BodyEncoding = Encoding.UTF8;

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

                        //
                        user.UserPassword = newPassword._RSAEcrypt();
                        statement.EditUser(user);
                        statement.SaveChanged();

                        //提示发送成功
                        MessageBox.Show("新密码已经发送到您的邮箱中，请及时查收!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }

        }

        private void _exitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
            System.Environment.Exit(0);
        }

        private void _comboBox用户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InputUserPassword();
        }

        private void InputUserPassword()
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                //
                if (_comboBox用户列表.Items.Count != 0 && !string.IsNullOrEmpty(_comboBox用户列表.SelectedValue.ToString().Trim()))
                {
                    try
                    {
                        using (StatementContext statement = new StatementContext(typeof(User)))
                        {
                            var user = statement.Users.FirstOrDefault(acc => acc.UserName.Equals(_comboBox用户列表.SelectedValue.ToString().Trim()));
                            if (user != null)
                            {
                                //if (bool.Parse(_Helper.GetParameter(ParameterName.RememberUserPassword.ToString(), false.ToString(), user)))
                                //{
                                _textBoxPassword.Password = user.UserPassword._RSADecrypt();
                                if (bool.Parse(_Helper.GetParameter(ParameterName.AutoLogin.ToString(), false.ToString(), user)))
                                {
                                    Login();
                                }
                                //}
                            }
                            else
                            {
                                MessageBox.Show(string.Format("数据库中不存在此账户{0}信息，请联系管理员！", _comboBox用户列表.Text));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }
                }
            }));
        }


    }
}
