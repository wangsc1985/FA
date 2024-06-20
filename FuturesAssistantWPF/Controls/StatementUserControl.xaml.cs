﻿using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Types;
using FuturesAssistantWPF.Windows;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace FuturesAssistantWPF.Controls
{
    /// <summary>
    /// StatementUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class StatementUserControl : UserControl
    {
        private delegate void FormControlInvoker();
        private string directoryPath = System.Windows.Forms.Application.StartupPath + "\\statement";

        public StatementUserControl()
        {
            InitializeComponent();
        }

        // 事件
        private void _buttonQuery_Click(object sender, RoutedEventArgs e)
        {
            Thread query = new Thread(() => Query(false));
            query.Start();
            AllQueryButtonEnabled();
        }

        private void _buttonRecovery_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "数据文件(*.bak)|*.bak";
            string dirPath = string.Format(@"{0}\backup", System.Windows.Forms.Application.StartupPath);
            if (Directory.Exists(dirPath))
            {
                ofd.InitialDirectory = dirPath;
            }
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Recovery(ofd.FileName);
            }
        }

        private void _buttonDayQuery_Click(object sender, RoutedEventArgs e)
        {
            //
            AllQueryButtonEnabled();
            _buttonDayQuery.IsEnabled = false;
            _buttonDayQuery._Refresh();
            //
            _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread query = new Thread(() => Query(false));
            query.Start();
        }

        private void _buttonWeekQuery_Click(object sender, RoutedEventArgs e)
        {
            //
            AllQueryButtonEnabled();
            _buttonWeekQuery.IsEnabled = false;
            _buttonWeekQuery._Refresh();
            //
            _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate.AddDays(DayOfWeek.Monday - _Session.LatestFundStatusDate.DayOfWeek);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread query = new Thread(() => Query(false));
            query.Start();
        }

        private void _buttonMonthQuery_Click(object sender, RoutedEventArgs e)
        {
            //
            AllQueryButtonEnabled();
            _buttonMonthQuery.IsEnabled = false;
            _buttonMonthQuery._Refresh();
            //
            _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, _Session.LatestFundStatusDate.Month, 1);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread query = new Thread(() => Query(false));
            query.Start();
        }

        private void _buttonYearQuery_Click(object sender, RoutedEventArgs e)
        {
            //
            AllQueryButtonEnabled();
            _buttonYearQuery.IsEnabled = false;
            _buttonYearQuery._Refresh();
            //
            _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, 1, 1);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread query = new Thread(() => Query(false));
            query.Start();
        }

        private void _buttonAllQuery_Click(object sender, RoutedEventArgs e)
        {
            //
            AllQueryButtonEnabled();
            _buttonAllQuery.IsEnabled = false;
            _buttonAllQuery._Refresh();
            //
            var fds = new StatementContext(typeof(FundStatus)).FundStatus.OrderBy(fs => fs.Date).FirstOrDefault();
            if (fds != null)
                _dateTimePicker开始.SelectedDate = fds.Date;
            else
                _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread query = new Thread(() => Query(false));
            query.Start();
        }

        private void _buttonImportExcel_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _buttonExportExcel_Click(object sender, RoutedEventArgs e)
        {
        }

        public void Initialize(bool sync = true)
        {
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    this._Refresh();
                    //this._buttonDayQuery_Click(null, null, sync);
                    if (sync)
                    {
                        new Thread(new ThreadStart(() =>
                        {
                            //
                            Dispatcher.Invoke(new FormControlInvoker(() =>
                            {
                                _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
                                _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
                            }));
                            //
                            Query();
                        })).Start(); ;
                    }
                    else
                    {
                        //
                        _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
                        _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
                        //
                        Query();
                    }
                    var fds = new StatementContext(typeof(FundStatus)).FundStatus.OrderBy(fs => fs.Date).FirstOrDefault();
                    if (fds != null)
                    {
                        _dateTimePicker开始.DisplayDateStart = _dateTimePicker结束.DisplayDateStart = fds.Date;
                    }
                    _dateTimePicker结束.DisplayDateEnd = DateTime.Today;
                }));
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void Query(bool isFromListSelected = false)
        {
            try
            {
                //
                DateTime? startDate = null;
                DateTime? endDate = null;
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _loading.Visibility = System.Windows.Visibility.Visible;
                    this._Refresh();
                    //
                    if (_dateTimePicker开始.SelectedDate.Value == DateTime.MinValue)
                        _dateTimePicker开始.SelectedDate = DateTime.Today;
                    if (_dateTimePicker结束.SelectedDate.Value == DateTime.MinValue)
                        _dateTimePicker结束.SelectedDate = DateTime.Today;
                    //
                    startDate = _dateTimePicker开始.SelectedDate;
                    endDate = _dateTimePicker结束.SelectedDate;
                }));

                if (startDate.HasValue && endDate.HasValue)
                {
                    using (StatementContext statement = new StatementContext())
                    {

                        DateTime endDateNext = endDate.Value.Date.AddDays(1);
                        var fundStatus = statement.FundStatus
                            .Where(fs => fs.AccountId == _Session.SelectedAccountId && fs.Date <= endDateNext && fs.Date >= startDate.Value)
                            .OrderBy(fs => fs.Date);

                        var remittances = statement.Remittances
                            .Where(rt => rt.AccountId == _Session.SelectedAccountId && rt.Date <= endDateNext && rt.Date >= startDate.Value)
                            .OrderBy(rt => rt.Date);

                        var trades = statement.Trades
                            .Where(t => t.AccountId == _Session.SelectedAccountId && t.Date <= endDate.Value && t.Date >= startDate.Value)
                            .OrderBy(t => t.Date);

                        var codeList = new List<string>();
                        if (!isFromListSelected)
                        {
                            foreach (var td in trades)
                            {
                                if (codeList.FirstOrDefault(a => a.Equals(td.Item)) == null)
                                {
                                    codeList.Add(td.Item);
                                }
                            }
                        }
                        var positions = statement.Positions
                            .Where(p => p.AccountId == _Session.SelectedAccountId && p.Date <= endDateNext && p.Date >= startDate.Value)
                            .OrderBy(p => p.Date);

                        var commoditys = statement.CommoditySummarizations
                            .Where(c => c.AccountId == _Session.SelectedAccountId && c.Date <= endDateNext && c.Date >= startDate.Value)
                            .OrderBy(c => c.Date);

                        var tradeDetails = statement.TradeDetails
                            .Where(td => td.AccountId == _Session.SelectedAccountId && td.ActualTime <= endDateNext && td.ActualTime >= startDate.Value)
                            .OrderBy(td => td.ActualTime);

                        var closedTradeDetails = statement.ClosedTradeDetails
                            .Where(ctd => ctd.AccountId == _Session.SelectedAccountId && ctd.ActualDate <= endDateNext && ctd.ActualDate >= startDate.Value)
                            .OrderBy(ctd => ctd.ActualDate);

                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {

                            if (_listBox合约.SelectedItems.Count == 1)
                            {
                                var cc = _listBox合约.SelectedItem.ToString();
                                trades = statement.Trades
                            .Where(t => t.AccountId == _Session.SelectedAccountId && t.Date <= endDate.Value && t.Date >= startDate.Value && t.Item.Equals(cc))
                            .OrderBy(t => t.Date);

                                tradeDetails = statement.TradeDetails
                                    .Where(td => td.AccountId == _Session.SelectedAccountId && td.ActualTime <= endDateNext && td.ActualTime >= startDate.Value && td.Item.Equals(cc))
                                    .OrderBy(td => td.ActualTime);

                                closedTradeDetails = statement.ClosedTradeDetails
                            .Where(ctd => ctd.AccountId == _Session.SelectedAccountId && ctd.ActualDate <= endDateNext && ctd.ActualDate >= startDate.Value && ctd.Item.Equals(cc))
                            .OrderBy(ctd => ctd.ActualDate);

                                positions = statement.Positions
                                .Where(p => p.AccountId == _Session.SelectedAccountId && p.Date <= endDateNext && p.Date >= startDate.Value && p.Item.Equals(cc))
                                .OrderBy(p => p.Date);
                            }

                        }));
                        var positionDetails = statement.PositionDetails
                            .Where(pd => pd.AccountId == _Session.SelectedAccountId && pd.DateForPosition <= endDateNext && pd.DateForPosition >= startDate.Value)
                            .OrderBy(pd => pd.DateForPosition);

                        foreach (var td in tradeDetails)
                        {
                            if (td.ActualTime.Hour > 15)
                            {
                                if (td.ActualTime.DayOfWeek == DayOfWeek.Monday)
                                {
                                    td.ActualTime = td.ActualTime.AddDays(-3);
                                }
                                else
                                {
                                    td.ActualTime = td.ActualTime.AddDays(-1);
                                }
                            }
                        }

                        tradeDetails = tradeDetails.OrderBy(m => m.ActualTime);

                        var tradeList = new List<Trade>();
                        foreach (var td in tradeDetails)
                        {
                            var trade = tradeList.FirstOrDefault(m => m.Item.Equals(td.Item) && m.OC == td.OC && m.BS == td.BS && m.Price == td.Price);
                            if (trade == null)
                            {
                                trade = new Trade();
                                trade.AccountId = td.AccountId;
                                trade.Amount = td.Amount;
                                trade.BS = td.BS;
                                trade.Commission = td.Commission;
                                trade.ClosedProfit = td.ClosedProfit;
                                trade.Date = td.ActualTime;
                                trade.Item = td.Item;
                                trade.OC = td.OC;
                                trade.Price = td.Price;
                                trade.Size = td.Size;
                                tradeList.Add(trade);
                            }
                            else
                            {
                                trade.Amount += td.Amount;
                                trade.Commission += td.Commission;
                                trade.ClosedProfit += td.ClosedProfit;
                                trade.Date = td.ActualTime;
                                trade.Size += td.Size;
                            }
                        }

                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            if (!isFromListSelected)
                            {
                                _listBox合约.Items.Clear();
                                foreach (var code in codeList)
                                {
                                    _listBox合约.Items.Add(code);
                                }
                            }
                            _loading.Visibility = System.Windows.Visibility.Hidden;
                            _dataGrid资金状况.ItemsSource = fundStatus.ToList();
                            _dataGrid出入金明细.ItemsSource = remittances.ToList();
                            _dataGrid成交汇总.ItemsSource = tradeList;
                            _dataGrid持仓汇总.ItemsSource = positions.ToList();
                            _dataGrid品种汇总.ItemsSource = commoditys.ToList();
                            _dataGrid成交明细.ItemsSource = tradeDetails.ToList();
                            _dataGrid平仓明细.ItemsSource = closedTradeDetails.ToList();
                            _dataGrid持仓明细.ItemsSource = positionDetails.ToList();


                        }));

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void HideStatus()
        {
            _textBlock实时状态.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void SetStatus(string text)
        {
            _textBlock实时状态.Text = text;
            _textBlock实时状态.Visibility = System.Windows.Visibility.Visible;
            _textBlock实时状态._Refresh();
        }

        private void AllQueryButtonEnabled()
        {
            _buttonDayQuery.IsEnabled = true;
            _buttonWeekQuery.IsEnabled = true;
            _buttonMonthQuery.IsEnabled = true;
            _buttonYearQuery.IsEnabled = true;
            _buttonAllQuery.IsEnabled = true;
        }

        private string saveDirPath = System.Windows.Forms.Application.StartupPath + "\\statement";

        private void _listBox合约_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Thread query = new Thread(() => Query(true));
            query.Start();
        }


#if false
        private void Backup()
        {
            string content = this._buttonBackup.Content.ToString();
            this._buttonBackup.Content = "正在备份...";
            this._buttonBackup.UpdateUIEvents();
            string dirPath = string.Format(@"{0}\backup", System.Windows.Forms.Application.StartupPath);
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            string bakPath = string.Format(@"{0}\Statement.{1}.bak", dirPath, DateTime.Now.ToString("yyyyMMdd.HHmmss"));
            var conn = new StatementContext().Database.Connection;
            var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = string.Format("BACKUP DATABASE Statement TO DISK ='{0}'", bakPath);
            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
                MessageBox.Show(string.Format("数据已备份至\n\"{0}\"", bakPath));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            finally
            {
                conn.Close();
                this._buttonBackup.Content = content;
            }
        }
        private void Recovery(string bakPath)
        {
            if (File.Exists(bakPath))
            {
            this._textBlock实时状态.Text = "正在恢复...";
            this._textBlock实时状态.UpdateUIEvents();
                using (var statement = new StatementContext())
                {
                    var conn = statement.Database.Connection;
                    var cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = string.Format("USE master RESTORE DATABASE Statement FROM DISK='{0}' WITH REPLACE", bakPath);
                    try
                    {
                        conn.Open();
                        cmd.ExecuteNonQuery();
                        FASession.HaveNewStatements = true;
                        MessageBox.Show("数据已恢复成功！");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("未发现备份文件！");
            }
        }
#endif

    }
}
