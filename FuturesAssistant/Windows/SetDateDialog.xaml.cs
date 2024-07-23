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
    /// DeleteStartDateDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SetDateDialog : DialogBase
    {
        public DateTime Value { get; set; }
        /// <summary>
        /// 默认起始日期为数据库中第一天结算单日期，默认结束日期为数据库中最后一天结算单日期
        /// </summary>
        public SetDateDialog()
        {
            InitializeComponent();
            using (StatementContext statement = new StatementContext())
            {
                var fss = statement.FundStatus.Where(fs => fs.AccountId == _Session.SelectedAccountId).OrderBy(fs => fs.Date);
                if (fss != null)
                {
                    _dateTimePicker.DisplayDateStart = fss.FirstOrDefault().Date;
                    _dateTimePicker.DisplayDateEnd = fss.LastOrDefault().Date;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="title">对话框标题</param>
        /// <param name="displayDateStart">可null参数，设置日期控件显示的最早日期，默认起始日期为数据库中第一天结算单日期</param>
        /// <param name="displayDateEnd">可null参数，设置日期控件显示的最晚日期，默认结束日期为数据库中最后一天结算单日期</param>
        public SetDateDialog(string title, DateTime displayDateStart, DateTime displayDateEnd)
        {
            InitializeComponent();
            Title = title;
            _dateTimePicker.DisplayDateStart = displayDateStart;
            _dateTimePicker.DisplayDateEnd = displayDateEnd;
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
