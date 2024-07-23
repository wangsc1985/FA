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
using FuturesAssistant.Helpers;

namespace FuturesAssistant.Windows
{
    /// <summary>
    /// ReplaceUserPassword.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyUserInfo : DialogBase
    {
        private User user;
        public ModifyUserInfo(string userId)
        {
            InitializeComponent();
            using (StatementContext statement = new StatementContext())
            {
                user = statement.User.ToList().FirstOrDefault(model => model.Id.Equals(userId));
                _textBox用户名.Text = user.UserName;
                _textBox用户密码.Password = user.UserPassword._RSADecrypt();
                _textBox用户密码2.Password = user.UserPassword._RSADecrypt();
                _textBox邮箱.Text = user.Email;
                Title = string.Format("<{0}>的信息", user.UserName);
            }
        }

        private void _button修改_Click(object sender, RoutedEventArgs e)
        {
            bool changed = false;
            using (StatementContext statement = new StatementContext())
            {
                //
                if (string.IsNullOrEmpty(_textBox用户名.Text) || _textBox用户名.Text.Length > 20)
                {
                    MessageBox.Show("用户名必须1-20个字符！");
                    return;
                }
                if (statement.User.ToList().FirstOrDefault(model => model.Id != user.Id && model.UserName.Equals(_textBox用户名.Text)) != null)
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

                var oldUser = statement.User.ToList().FirstOrDefault(model => model.Id == user.Id);
                if (!_textBox用户名.Text.Trim().Equals(user.UserName))
                {
                    changed = true;
                    oldUser.UserName = _textBox用户名.Text.Trim();
                }
                if (!_textBox用户密码.Password.Trim().Equals(user.UserPassword._RSADecrypt()))
                {
                    changed = true;
                    oldUser.UserPassword = _textBox用户密码.Password._RSAEcrypt();
                }
                if (!_textBox邮箱.Text.Trim().Equals(user.Email))
                {
                    changed = true;
                    oldUser.Email = _textBox邮箱.Text.Trim();
                }

                if (changed)
                {
                    statement.SaveChanges();
                    MessageBox.Show("信息修改成功！");
                }
                Close();
            }
        }

        private void _button取消_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
