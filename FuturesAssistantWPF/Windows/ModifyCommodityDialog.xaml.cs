using FuturesAssistantWPF.Models;
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
    /// ModifyCommodity.xaml 的交互逻辑
    /// </summary>
    public partial class ModifyCommodity : DialogBase
    {
        public ModifyCommodity(Commodity commodity)
        {
            InitializeComponent();
            _tb品种代码.Text = commodity.Code;
            _tb品种名称.Text = commodity.Name;
            _tb品种名称.Focus();
            _tb品种名称.SelectAll();
        }

        private void _btnOK_Click(object sender, RoutedEventArgs e)
        {
            using (StatementContext statement = new StatementContext())
            {
                var comm = statement.Commoditys.FirstOrDefault(m => m.Code.ToLower().Equals(_tb品种代码.Text.Trim().ToLower()));
                if (comm == null)
                {
                    MessageBox.Show("数据库中不存在此品种");
                    return;
                }
                comm.Name = _tb品种名称.Text;
                statement.EditCommodity(comm);
                statement.SaveChanged();
            }
            Close();
        }

        private void _btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _tb品种名称_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _btnOK_Click(sender, e);
            }
        }
    }
}
