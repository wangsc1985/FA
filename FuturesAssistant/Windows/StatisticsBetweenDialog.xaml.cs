using System;
using System.Collections.Generic;
using System.Linq;
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
    /// StatisticsBetweenWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StatisticsBetweenDialog : DialogBase
    {
        public StatisticsBetweenDialog(DateTime beginDate, DateTime stopDate, decimal open, decimal high, decimal low, decimal close, decimal volume, string count)
        {
            InitializeComponent();

            _textBox起始日期.Text = beginDate.ToShortDateString();
            _textBox结束日期.Text = stopDate.ToShortDateString();
            _textBox期初价.Text = open.ToString("C");
            _textBox最高价.Text = high.ToString("C");
            _textBox最低价.Text = low.ToString("C");
            _textBox期末价.Text = close.ToString("C");
            _textBox成交额.Text = volume.ToString("C");
            _textBox盈利.Text = (close - open).ToString("C");
            _textBox区间涨幅.Text = ((close - open) * 100 / open).ToString("0.00") + "%";
            _textBox区间振幅.Text = ((high - low) * 100 / open).ToString("0.00") + "%";
            _textBox统计周期.Text = count;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
