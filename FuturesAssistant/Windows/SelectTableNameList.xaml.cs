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
    /// SeleteTableNameList.xaml 的交互逻辑
    /// </summary>
    public partial class SelectTableNameList : DialogBase
    {
        public List<Type> SelectedTypes = new List<Type>();

        public SelectTableNameList()
        {
            InitializeComponent();
        }

        private void _button提交_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _checkBox全选_Checked(object sender, RoutedEventArgs e)
        {
            if (_checkBox全选.IsChecked.Value)
            {
                _checkBox参数表.IsChecked = true;
                _checkBox持仓表.IsChecked = true;
                _checkBox持仓明细表.IsChecked = true;
                _checkBox出入金明细表.IsChecked = true;
                _checkBox交易表.IsChecked = true;
                _checkBox交易明细表.IsChecked = true;
                _checkBox蜡烛图表.IsChecked = true;
                _checkBox品种交易统计表.IsChecked = true;
                _checkBox平仓明细表.IsChecked = true;
                _checkBox用户表.IsChecked = true;
                _checkBox账户表.IsChecked = true;
                _checkBox资金状况表.IsChecked = true;
            }
            else
            {
                _checkBox参数表.IsChecked = false;
                _checkBox持仓表.IsChecked = false;
                _checkBox持仓明细表.IsChecked = false;
                _checkBox出入金明细表.IsChecked = false;
                _checkBox交易表.IsChecked = false;
                _checkBox交易明细表.IsChecked = false;
                _checkBox蜡烛图表.IsChecked = false;
                _checkBox品种交易统计表.IsChecked = false;
                _checkBox平仓明细表.IsChecked = false;
                _checkBox用户表.IsChecked = false;
                _checkBox账户表.IsChecked = false;
                _checkBox资金状况表.IsChecked = false;
            }
        }
    }
}
