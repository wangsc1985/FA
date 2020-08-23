using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Helpers;
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

namespace FuturesAssistantWPF.Windows
{
    /// <summary>
    /// EditCooperateAccountDialog.xaml 的交互逻辑
    /// </summary>
    public partial class EditCooperateAccountDialog : DialogBase
    {
        private Account account;
        public EditCooperateAccountDialog(Guid accountId)
        {
            InitializeComponent();

            using (StatementContext statement = new StatementContext(typeof(Account)))
            {
                account = statement.Accounts.FirstOrDefault(m => m.Id == accountId);
                _textBox客户姓名.Text = account.CustomerName;
                _textBox配资公司.Text = account.FuturesCompanyName;
                _textBox配资账号.Text = account.AccountNumber;
                _textBox交易密码.Text = account.Password._RSADecrypt();
            }
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {
            using (StatementContext statement = new StatementContext(typeof(Account)))
            {
                account.CustomerName = _textBox客户姓名.Text;
                account.FuturesCompanyName = _textBox配资公司.Text;
                account.AccountNumber = _textBox配资账号.Text;
                account.Password = _textBox交易密码.Text.Trim()._RSAEcrypt();
                statement.EditAccount(account);
                statement.SaveChanged();
            }
            Close();
            MessageBox.Show("修改账户成功！");
        }
    }
}
