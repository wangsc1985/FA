using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Types;
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

namespace FuturesAssistantWPF.Windows
{
    /// <summary>
    /// SetStartDateWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetStockStartDateDialog : DialogBase
    {
        public DateTime Value { get; set; }

        public SetStockStartDateDialog()
        {
            InitializeComponent();
            var _chartDataStartDate = _Helper.GetParameter(ParameterName.StockChart图表开始日期.ToString());
            if (_chartDataStartDate != null)
            {
                _dateTimePicker.SelectedDate = DateTime.Parse(_chartDataStartDate);
            }
        }

        private void _button确定_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Value = _dateTimePicker.SelectedDate.Value.Date;
            Close();
        }

        private void _button取消_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
