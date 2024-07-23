using FuturesAssistant.FAException;
using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using System;
using System.Collections.Generic;
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
    public partial class AddAccountWindow : DialogBase
    {
        private CookieContainer cookie = new CookieContainer();
        public CookieContainer Cookie { get { return cookie; } }
        public AddAccountWindow()
        {
            InitializeComponent();
            _pictureBox验证码.Click += _pictureBox验证码_Click;
            _textBox资金账号.Focus();
        }

        private void _pictureBox验证码_Click(object sender, EventArgs e)
        {
            _gridLoading.Visibility = System.Windows.Visibility.Visible;
            _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
            _gridLoading.Visibility = System.Windows.Visibility.Hidden;
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {

            _gridLoading.Visibility = System.Windows.Visibility.Visible;
            //
            using (StatementContext statement = new StatementContext())
            {
                if (statement.Account.Where(a => a.AccountNumber.Equals(_textBox资金账号.Text.Trim())).Count() != 0)
                {
                    _gridLoading.Visibility = System.Windows.Visibility.Hidden;
                    MessageBox.Show("此资金账户已经存在于当前数据库中！");
                    Close();
                }
                else
                {
                    try
                    {
                        string htmlStatement=_Helper.LoginWithVerify(Cookie, _textBox资金账号.Text.Trim(), _textBox资金密码.Password.Trim(), _textBox验证码.Text.Trim());
                         //= _Helper.RequestElementrayInformationPage(Cookie);
                        Account account = new Account();

                        //基本资料
                        int startIndex = htmlStatement.IndexOf("基本资料");
                        int endIndex = htmlStatement.IndexOf("资金状况");
                        string strAccount = htmlStatement.Substring(startIndex, endIndex - startIndex);
                        endIndex = strAccount.IndexOf("</table>", StringComparison.CurrentCultureIgnoreCase);
                        strAccount = strAccount.Substring(0, endIndex);

                        //客户名称
                        startIndex = strAccount.IndexOf("客户名称");
                        strAccount = strAccount.Substring(startIndex);
                        startIndex = strAccount.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strAccount = strAccount.Substring(startIndex);
                        startIndex = strAccount.IndexOf(">") + 1;
                        strAccount = strAccount.Substring(startIndex);
                        endIndex = strAccount.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);
                        //
                        account.CustomerName = strAccount.Substring(0, endIndex).Replace("&nbsp;", "").Trim();

                        //期货公司名称
                        startIndex = strAccount.IndexOf("期货公司名称");
                        strAccount = strAccount.Substring(startIndex);
                        startIndex = strAccount.IndexOf(@"<TD", StringComparison.CurrentCultureIgnoreCase);
                        strAccount = strAccount.Substring(startIndex);
                        startIndex = strAccount.IndexOf(">") + 1;
                        strAccount = strAccount.Substring(startIndex);
                        endIndex = strAccount.IndexOf(@"</TD>", StringComparison.CurrentCultureIgnoreCase);

                        //
                        account.FuturesCompanyName = strAccount.Substring(0, endIndex).Replace("&nbsp;", "").Trim();
                        account.IsAllowLoad = true;
                        account.AccountNumber = _textBox资金账号.Text.Trim();
                        account.Password = _textBox资金密码.Password.Trim()._RSAEcrypt();
                        account.UserId = _Session.LoginedUserId;

                        account.Type = 1;
                        statement.Account.Add(account);
                        statement.SaveChanges();

                        //
                        _gridLoading.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show("添加资金账户成功！");
                        DialogResult = true;

                        //
                        Close();
                    }
                    catch (IdentifyingCodeMismatchException ecwex)
                    {
                        MessageBox.Show(DateTime.Now.ToLongTimeString() + ecwex.Message);
                    }
                    catch (UsernameOrPasswordWrongException uopwex)
                    {
                        MessageBox.Show(DateTime.Now.ToLongTimeString() + uopwex.Message);
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
                        MessageBox.Show(DateTime.Now.ToLongTimeString() + ex.Message);
                    }
                    //
                    _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
                    _gridLoading.Visibility = System.Windows.Visibility.Hidden;
                    _textBox验证码.Text = "";
                }
            }
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {

            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //
                _pictureBox验证码.Image = _Helper.GetValidateCode(Cookie);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
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
