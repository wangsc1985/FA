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
    /// AddCooperateDataDialog.xaml 的交互逻辑
    /// </summary>
    public partial class AddCooperateDataDialog : DialogBase
    {
        private string accountId;
        private DateTime startDate;

        public AddCooperateDataDialog(string accountId)
        {
            InitializeComponent();

            this.accountId = accountId;
            _datePicker日期.SelectedDate = DateTime.Today;
            //this._datePicker日期.Text = DateTime.Today.ToShortDateString();

            using (StatementContext statement = new StatementContext())
            {
                var account = statement.Account.ToList().FirstOrDefault(m => m.Id == accountId);
                var stocks = statement.Stock.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                var firstStock = stocks.FirstOrDefault();
                var lastStock = stocks.LastOrDefault();
                if (lastStock.Date.Date >= DateTime.Today)
                {
                    _datePicker日期.Visibility = System.Windows.Visibility.Hidden;
                    _button确认.Visibility = System.Windows.Visibility.Hidden;
                    return;
                }
                if (firstStock != null)
                {
                    _datePicker日期.DisplayDateStart = startDate = firstStock.Date;
                }
                _datePicker日期.DisplayDateEnd = DateTime.Today;
                foreach (var stock in stocks)
                {
                    _datePicker日期.BlackoutDates.Add(new CalendarDateRange(stock.Date.Date));
                }
                if (lastStock != null && lastStock.Date.Date < DateTime.Today)
                {
                    _datePicker日期.SelectedDate = lastStock.Date.Date.AddDays(1);
                    if (lastStock.Date.DayOfWeek == DayOfWeek.Friday)
                    {
                        _datePicker日期.SelectedDate = lastStock.Date.Date.AddDays(3);
                    }
                }
            }
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {

            decimal profit = 0, commission = 0, remittance = 0, volume = 0;
            int bing = 0;
            try
            {
                profit = Decimal.Parse(_textBox盈利.Text.Trim());
                bing = 1;
                commission = Decimal.Parse(_textBox手续费.Text.Trim());
                bing = 2;
                remittance = Decimal.Parse(_textBox出入金.Text.Trim());
                bing = 3;
                volume = Decimal.Parse(_textBox成交额.Text.Trim());
            }
            catch (FormatException)
            {
                switch (bing)
                {
                    case 0: MessageBox.Show("盈利必须为数字！");
                        break;
                    case 1: MessageBox.Show("手续费必须为数字！");
                        break;
                    case 2: MessageBox.Show("出入金必须为数字！");
                        break;
                    case 3: MessageBox.Show("成交额必须为数字！");
                        break;
                }
            }
            catch (OverflowException)
            {
                switch (bing)
                {
                    case 0: MessageBox.Show("盈利超出数字范围！");
                        break;
                    case 1: MessageBox.Show("手续费超出数字范围！");
                        break;
                    case 2: MessageBox.Show("出入金超出数字范围！");
                        break;
                    case 3: MessageBox.Show("成交额超出数字范围！");
                        break;
                }
            }

            using (StatementContext statement = new StatementContext())
            {
                var cooperate = statement.Account.ToList().FirstOrDefault(m => m.Id == accountId);
                if (cooperate == null)
                    throw new Exception(string.Concat("不存在ID为：", accountId, "的配资账户！"));

                var sto = statement.Stock.ToList().FirstOrDefault(m => m.AccountId == accountId && m.Date == _datePicker日期.SelectedDate.Value);
                if (sto != null)
                {
                    MessageBox.Show(string.Concat("此账户【", _datePicker日期.SelectedDate.Value, "】数据已经存在！"));
                    return;
                }

                var allStocks = statement.Stock.Where(m => m.AccountId == accountId).OrderBy(m => m.Date);
                var beforeStocks = allStocks.Where(m => m.Date < _datePicker日期.SelectedDate.Value);
                var afterStocks = allStocks.Where(m => m.Date > _datePicker日期.SelectedDate.Value);
                var lastStock = beforeStocks.LastOrDefault();

                Stock stock = new Stock();
                if (lastStock != null)
                {
                    stock.Open = lastStock.Close;
                }
                else
                {
                    stock.Open = remittance;
                }
                stock.Close = stock.Open + profit - commission;
                stock.High = stock.Open < stock.Close ? stock.Close : stock.Open;
                stock.Low = stock.Open > stock.Close ? stock.Close : stock.Open;
                stock.Date = _datePicker日期.SelectedDate.Value;
                stock.Volume = volume;
                stock.AccountId = accountId;

                //
                statement.Stock.Add(stock);

                //
                if (afterStocks.Count() != 0 && stock.Close != stock.Open)
                {
                    foreach (var st in afterStocks)
                    {
                        st.Open += stock.Close - stock.Open;
                        st.Close += stock.Close - stock.Open;
                        st.High += stock.Close - stock.Open;
                        st.Low += stock.Close - stock.Open;
                    }
                }

                if (remittance != 0)
                {
                    var stocks = statement.Stock.Where(m => m.AccountId == accountId);
                    foreach (var st in stocks)
                    {
                        st.Open += remittance;
                        st.Close += remittance;
                        st.High += remittance;
                        st.Low += remittance;
                    }
                }
                statement.SaveChanges();
            }
            DialogResult = true;
            MessageBox.Show("添加成功！");
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _textBox盈利_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Decimal.Parse(_textBox盈利.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("盈利必须为数字！");
                _textBox盈利.Text = "0";
            }
            catch (OverflowException)
            {
                MessageBox.Show("盈利超出数字范围！");
                _textBox盈利.Text = "0";
            }
        }

        private void _textBox手续费_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Decimal.Parse(_textBox手续费.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("手续费必须为数字！");
                _textBox手续费.Text = "0";
            }
            catch (OverflowException)
            {
                MessageBox.Show("手续费超出数字范围！");
                _textBox手续费.Text = "0";
            }
        }

        private void _textBox出入金_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Decimal.Parse(_textBox出入金.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("出入金必须为数字！");
                _textBox出入金.Text = "0";
            }
            catch (OverflowException)
            {
                MessageBox.Show("出入金超出数字范围！");
                _textBox出入金.Text = "0";
            }
        }

        private void _textBox出入金_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBox出入金.SelectAll();
        }

        private void _textBox手续费_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBox手续费.SelectAll();
        }

        private void _textBox盈利_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBox盈利.SelectAll();
        }

        private void _textBox成交额_GotFocus(object sender, RoutedEventArgs e)
        {
            _textBox成交额.SelectAll();
        }

        private void _textBox成交额_LostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                Decimal.Parse(_textBox成交额.Text.Trim());
            }
            catch (FormatException)
            {
                MessageBox.Show("成交额必须为数字！");
                _textBox成交额.Text = "0";
            }
            catch (OverflowException)
            {
                MessageBox.Show("成交额超出数字范围！");
                _textBox成交额.Text = "0";
            }
        }

        private void _datePicker日期_LostFocus(object sender, RoutedEventArgs e)
        {
            //if (DateTime.Parse(this._datePicker日期.Text) < startDate)
            //{
            //    MessageBox.Show(string.Concat("日期超出有效范围！<", startDate.ToShortDateString(), " - ", DateTime.Today.ToShortDateString(), ">"));
            //    this._datePicker日期.DisplayDateStart = startDate;
            //}
        }

        private void _datePicker日期_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            using (StatementContext statement = new StatementContext())
            {
                var account = statement.Account.ToList().FirstOrDefault(m => m.Id == accountId);
                var stock = statement.Stock.ToList().FirstOrDefault(m => m.AccountId == accountId && m.Date == _datePicker日期.SelectedDate.Value);
                if (stock != null)
                {
                    _button确认.Content = "替换";
                }
                else
                {
                    _button确认.Content = "添加";
                }
            }
        }
    }
}
