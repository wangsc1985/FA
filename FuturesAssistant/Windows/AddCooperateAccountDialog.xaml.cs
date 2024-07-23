using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// AddCooperateAccountDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddCooperateAccountDialog : DialogBase
    {
        public AddCooperateAccountDialog()
        {
            InitializeComponent();

            using (StatementContext statement = new StatementContext())
            {
                _textBox客户姓名.Text = statement.User.ToList().FirstOrDefault(m => m.Id == _Session.LoginedUserId).UserName;
            }
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {
            //
            using (StatementContext statement = new StatementContext())
            {
                if (statement.Account.Where(a => a.AccountNumber.Equals(_textBox配资账号.Text.Trim()) && a.FuturesCompanyName.Equals(_textBox配资公司.Text.Trim())).Count() != 0)
                {
                    MessageBox.Show("此配资账户已经存在于当前数据库中！");
                    Close();
                }
                else
                {
                    try
                    {
                        Account account = new Account();

                        //
                        account.Id = Guid.NewGuid().ToString();
                        account.Type = 2;
                        account.FuturesCompanyName = _textBox配资公司.Text.Trim();
                        account.AccountNumber = _textBox配资账号.Text.Trim();
                        account.CustomerName = _textBox客户姓名.Text.Trim();
                        account.Password = _textBox交易密码.Text.Trim()._RSAEcrypt(); ;
                        account.UserId = _Session.LoginedUserId;
                        statement.Account.Add(account);
                        statement.SaveChanges();

                        //
                        DialogResult = true;
                        MessageBox.Show("添加配资账户成功！");

                        //
                        Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(DateTime.Now.ToLongTimeString() + ex.Message);
                    }
                }
            }
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
