using FuturesAssistant.FAException;
using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuturesAssistant.Windows
{
    /// <summary>
    /// AddAccountWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoginCfmmcDialog : DialogBase
    {
        private CookieContainer cookie = new CookieContainer();
        public CookieContainer Cookie { get { return cookie; } }

        private delegate void FormControlInvoker();
        private Account requestAccount;

        public LoginCfmmcDialog(Account requestAccount)
        {
            InitializeComponent();

            //
            this.requestAccount = requestAccount;
            _textBox期货公司.Text = requestAccount.FuturesCompanyName;
            _textBox资金账号.Text = requestAccount.AccountNumber;
            _textBox资金密码.Password = requestAccount.Password._RSADecrypt();
            _pictureBox验证码.Click += _pictureBox验证码_Click;
            _textBox验证码.Focus();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                try
                {
                    _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
                    //var cracker = new Cracker();
                    //var result = cracker.Read(new Bitmap(this._pictureBox验证码.Image));
                    //this._textBox验证码.Text = result;
                }
                catch (Exception ex)
                {
                    Close();
                    MessageBox.Show(string.Format("连接保证金监控中心失败，请保证网络通畅！", ex.Message), string.Format("<{0}>打开登录窗口失败", requestAccount.AccountNumber));

                }
            }));
        }

        void _pictureBox验证码_Click(object sender, EventArgs e)
        {
            _gridLoading.Visibility = System.Windows.Visibility.Visible;
            _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
            _gridLoading.Visibility = System.Windows.Visibility.Hidden;
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _gridLoading.Visibility = System.Windows.Visibility.Visible;
                _gridLoading._Refresh();
                _Helper.LoginWithVerify(Cookie, _textBox资金账号.Text.Trim(), _textBox资金密码.Password.Trim(), _textBox验证码.Text.Trim());
                if (_textBox资金密码.IsEnabled == true)
                {
                    using (StatementContext statement = new StatementContext(typeof(Account)))
                    {
                        Account account = statement.Accounts.FirstOrDefault(a => a.AccountNumber.Equals(_textBox资金账号.Text.Trim()));
                        if (account != null)
                        {
                            account.Password = _textBox资金密码.Password.Trim()._RSAEcrypt();
                            statement.EditAccount(account);
                            statement.SaveChanged();
                        }
                        else
                        {
                            _gridLoading.Visibility = System.Windows.Visibility.Hidden;
                            MessageBox.Show(string.Format("数据库错误：本地数据库中不存在资金账户“{0}”，请联系管理员！", _textBox资金账号.Text.Trim()));
                        }
                    }
                }
                DialogResult = true;
                _gridLoading.Visibility = System.Windows.Visibility.Hidden;
                Close();
            }
            catch (IdentifyingCodeMismatchException ecwex)
            {
                MessageBox.Show(DateTime.Now.ToLongTimeString() + ecwex.Message);
            }
            catch (UsernameOrPasswordWrongException uopwex)
            {
                MessageBox.Show(DateTime.Now.ToLongTimeString() + uopwex.Message);
                _textBox资金密码.Password = "";
                _textBox资金密码.IsEnabled = true;
                _textBox资金密码.Focus();
            }
            catch (InputNullException inex)
            {
                MessageBox.Show(DateTime.Now.ToLongTimeString() + inex.Message);
            }
            catch (TryTooMoreException ttmex)
            {
                MessageBox.Show(DateTime.Now.ToLongTimeString() + ttmex.Message);
                Close();
                return;
            }
            catch (IllegalCustomerNameException iex)
            {
                MessageBox.Show(DateTime.Now.ToLongTimeString() + iex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("账号<{1}>登录保证金监控中心时失败，错误信息：\n{0}", ex.Message, requestAccount.AccountNumber));
            }
            //
            _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
            _gridLoading.Visibility = System.Windows.Visibility.Hidden;
            _textBox验证码.Text = "";
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _textBox验证码_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_textBox验证码.Text.Trim().Count() == 6)
            {
                _button确认_Click(sender, e);
            }
        }
    }
}
