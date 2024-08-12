using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Types;
using FuturesAssistant.Windows;
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

namespace FuturesAssistant.Controls
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
            var fds = new StatementContext().FundStatus.OrderBy(fs => fs.Date).ToList().FirstOrDefault();
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
                    var fds = new StatementContext().FundStatus.OrderBy(fs => fs.Date).ToList().FirstOrDefault();
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
        private string longStr = "多单";
        private string shortStr = "空单";

        private string TradeTypeString(Trade trade)
        {
            var isLong = true;
            if (trade.OC.Equals("开"))
            {
                isLong = trade.BS.Trim().Equals("买");
            }
            else
            {
                isLong = trade.BS.Trim().Equals("卖");
            }
            return isLong ? longStr : shortStr;
        }
        private string TradeTypeString(TradeDetail trade)
        {
            var isLong = true;
            if (trade.OC.Equals("开"))
            {
                isLong = trade.BS.Trim().Equals("买");
            }
            else
            {
                isLong = trade.BS.Trim().Equals("卖");
            }
            return isLong ? longStr : shortStr;
        }
        private string TradeTypeString(ClosedTradeDetail trade)
        {
            return trade.BS.Trim().Equals("卖") ? longStr : shortStr;
        }

        private List<Trade> tradeList = new List<Trade>();
        private List<CommoditySummarization> commoditySummarizationList = new List<CommoditySummarization>();
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
                            .OrderBy(fs => fs.Date).ToList();

                        var remittances = statement.Remittance
                            .Where(rt => rt.AccountId == _Session.SelectedAccountId && rt.Date <= endDateNext && rt.Date >= startDate.Value)
                            .OrderBy(rt => rt.Date).ToList();

                        var trades = statement.Trade
                            .Where(t => t.AccountId == _Session.SelectedAccountId && t.Date <= endDate.Value && t.Date >= startDate.Value)
                            .OrderBy(t => t.Date).ToList();

                        var codeList = new List<string>();
                        if (!isFromListSelected)
                        {
                            foreach (var td in trades)
                            {
                                if (codeList.FirstOrDefault(a => a.Equals($"{td.Item}\t{TradeTypeString(td)}\t{ToCommodityName(td)}")) == null)
                                {
                                    codeList.Add($"{td.Item}\t{TradeTypeString(td)}\t{ToCommodityName(td)}");
                                }
                            }
                        }
                        var positions = statement.Position
                            .Where(p => p.AccountId == _Session.SelectedAccountId && p.Date <= endDateNext && p.Date >= startDate.Value)
                            .OrderBy(p => p.Date).ToList();

                        var commoditys = statement.CommoditySummarization
                            .Where(c => c.AccountId == _Session.SelectedAccountId && c.Date <= endDateNext && c.Date >= startDate.Value)
                            .OrderBy(c => c.Date).ToList();

                        var tradeDetails = statement.TradeDetail
                            .Where(td => td.AccountId == _Session.SelectedAccountId && td.ActualTime <= endDateNext && td.ActualTime >= startDate.Value)
                            .OrderBy(td => td.ActualTime).ToList();

                        var closedTradeDetails = statement.ClosedTradeDetail
                            .Where(ctd => ctd.AccountId == _Session.SelectedAccountId && ctd.ActualDate <= endDateNext && ctd.ActualDate >= startDate.Value)
                            .OrderBy(ctd => ctd.ActualDate).ToList();
                        var positionDetails = statement.PositionDetail
                            .Where(pd => pd.AccountId == _Session.SelectedAccountId && pd.DateForPosition <= endDateNext && pd.DateForPosition >= startDate.Value)
                            .OrderBy(pd => pd.DateForPosition).ToList();

                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {

                            if (_listBox合约.SelectedItems.Count == 1)
                            {
                                var cc = _listBox合约.SelectedItem.ToString();
                                var ccs = cc.Split('\t');
                                var item = ccs[0];
                                var tradeType = ccs[1];
                                var isLongPosition = tradeType.Equals(longStr);

                                trades = statement.Trade.ToList()
                                .Where(t => t.AccountId == _Session.SelectedAccountId && t.Date <= endDate.Value && t.Date >= startDate.Value && t.Item.Equals(item) &&TradeTypeString(t).Equals(tradeType))
                                .OrderBy(t => t.Date).ToList();

                                tradeDetails = statement.TradeDetail.ToList()
                                .Where(td => td.AccountId == _Session.SelectedAccountId && td.ActualTime <= endDateNext && td.ActualTime >= startDate.Value && td.Item.Equals(item) && TradeTypeString(td).Equals(tradeType))
                                .OrderBy(td => td.ActualTime).ToList();

                                closedTradeDetails = statement.ClosedTradeDetail.ToList()
                                .Where(ctd => ctd.AccountId == _Session.SelectedAccountId && ctd.ActualDate <= endDateNext && ctd.ActualDate >= startDate.Value && ctd.Item.Equals(item) && TradeTypeString(ctd).Equals(tradeType))
                                .OrderBy(ctd => ctd.ActualDate).ToList();

                                positions = statement.Position.ToList()
                                .Where(p => p.AccountId == _Session.SelectedAccountId && p.Date <= endDateNext && p.Date >= startDate.Value && p.Item.Equals(item))
                                .OrderBy(p => p.Date).ToList();

                                positionDetails = statement.PositionDetail.ToList()
                                .Where(pd => pd.AccountId == _Session.SelectedAccountId && pd.DateForPosition <= endDateNext && pd.DateForPosition >= startDate.Value && pd.Item.Equals(item))
                                .OrderBy(pd => pd.DateForPosition).ToList();
                            }
                        }));

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

                        tradeDetails = tradeDetails.OrderBy(m => m.ActualTime).ToList();

                        decimal totalCommission = 0, totalProfit = 0, totalAmount = 0;
                        int totalSize = 0 ;
                        tradeList = new List<Trade>();
                        var isOpen = "";

                        foreach (var td in tradeDetails)
                        {
                            totalCommission += td.Commission;
                            totalProfit += td.ClosedProfit;
                            totalAmount += td.Amount;
                            totalSize += td.Size;

                            var trade = tradeList.FirstOrDefault(m =>m.OC.Equals(isOpen)&&m.Date.DayOfYear==td.ActualTime.DayOfYear&& m.Item.Equals(td.Item) && m.OC == td.OC && m.BS == td.BS && m.Price == td.Price);
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
                            isOpen = td.OC;
                        }
                        var tt = new Trade();
                        tt.Date = DateTime.Now;
                        tt.ClosedProfit = totalProfit;
                        tt.Commission = totalCommission;
                        tt.Size = totalSize;
                        tradeList.Add(tt);

                        commoditySummarizationList = new List<CommoditySummarization>();
                        foreach (var comm in commoditys)
                        {
                            var tmp = commoditySummarizationList.FirstOrDefault(m => m.Commodity ==ToCommodityName(comm.Commodity));
                            if(tmp == null)
                            {
                                var commodity = new CommoditySummarization();
                                commodity.Commission = comm.Commission;
                                commodity.Commodity = ToCommodityName(comm.Commodity);
                                commodity.AccountId = comm.AccountId;
                                commodity.ClosedProfit = comm.ClosedProfit;
                                commodity.Date = comm.Date;
                                commodity.Size = comm.Size;
                                commoditySummarizationList.Add(commodity);
                            }
                            else
                            {
                                tmp.Commission += comm.Commission;
                                tmp.ClosedProfit += comm.ClosedProfit;
                                tmp.Date = comm.Date;
                                tmp.Size += comm.Size;
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
                            _loading.Visibility = Visibility.Hidden;
                            _dataGrid资金状况.ItemsSource = fundStatus;
                            _dataGrid出入金明细.ItemsSource = remittances;
                            _dataGrid成交汇总.ItemsSource = tradeList.OrderBy(m=>m.Date).ToList();
                            _dataGrid持仓汇总.ItemsSource = positions;
                            _dataGrid品种汇总.ItemsSource = commoditySummarizationList;
                            _dataGrid成交明细.ItemsSource = tradeDetails;
                            _dataGrid平仓明细.ItemsSource = closedTradeDetails;
                            _dataGrid持仓明细.ItemsSource = positionDetails;


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
            _textBlock实时状态.Visibility = Visibility.Collapsed;
        }

        private string ToCommodityName(Trade trade)
        {
            var code = trade.Item.Replace("0", "").Replace("1", "").Replace("2", "").Replace("3", "").Replace("4", "")
                .Replace("5", "").Replace("6", "").Replace("7", "").Replace("8", "").Replace("9", "").ToLower();

            return ToCommodityName(code);
        }
        private string ToCommodityName(string commodityCode)
        {
            using (var context = new StatementContext())
            {
                var comm = context.Commodity.FirstOrDefault(com => com.Code.ToLower().Equals(commodityCode.ToLower()));
                if (comm != null)
                {
                    return comm.Name;
                }
                return "";
            }
        }

        private void SetStatus(string text)
        {
            _textBlock实时状态.Text = text;
            _textBlock实时状态.Visibility = Visibility.Visible;
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

        private void _buttonExport_Click(object sender, RoutedEventArgs e)
        {
            var strList =  new List<string>();

            foreach(var td in tradeList)
            {
                strList.Add($"{td.Id}:{td.Item}:{td.Date}:{td.OC}:{td.BS}:{td.Price}:{td.Size}:{td.Commission}:{td.ClosedProfit}:{td.SH}");
            }
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
