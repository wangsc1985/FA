using FuturesAssistant.Models;
using FuturesAssistant.Helpers;
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
    /// AddUserDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddUserDialog : DialogBase
    {
        public AddUserDialog()
        {
            InitializeComponent();
        }

        private void _button添加_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StatementContext statement = new StatementContext(typeof(User)))
                {
                    //
                    if (string.IsNullOrEmpty(_textBox用户名.Text) || _textBox用户名.Text.Length > 20)
                    {
                        MessageBox.Show("用户名必须1-20个字符！");
                        return;
                    }
                    if (statement.Users.FirstOrDefault(user => user.UserName.Equals(_textBox用户名.Text)) != null)
                    {
                        MessageBox.Show("用户名已被占用！");
                        return;
                    }
                    if (string.IsNullOrEmpty(_textBox用户密码.Password) || _textBox用户密码.Password.Length < 6 || _textBox用户密码.Password.Length > 20)
                    {
                        MessageBox.Show("密码必须6-20个字符！");
                        return;
                    }
                    if (!_textBox用户密码.Password.Equals(_textBox用户密码2.Password))
                    {
                        MessageBox.Show("两次密码不一致！");
                        return;
                    }

                    //
                    statement.AddUser(new User { Id = Guid.NewGuid(), UserName = _textBox用户名.Text, UserPassword = _textBox用户密码.Password._RSAEcrypt(), Email = _textBox邮箱.Text });
                    statement.SaveChanged();
                    DialogResult = true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void _button取消_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //DialogResult = false;
        }
    }
}
