using FuturesAssistant.Models;
using FuturesAssistant.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuturesAssistant.Controls
{
    /// <summary>
    /// ExchangeCommissionUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class ExchangeCommissionUserControl : UserControl
    {
        private delegate void FormControlInvoker();
        private string blTmp;
        private bool sh = true, zz = true, dl = true;

        public ExchangeCommissionUserControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取上海商品交易所数据
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string GetDataSH(DateTime dateTime)
        {
            try
            {
                //if (dateTime.Date==DateTime.Today && dateTime.Hour < 17)
                //    dateTime = dateTime.AddDays(-1);
                string year = "" + dateTime.Year;
                string month = dateTime.Month < 10 ? "0" + dateTime.Month : "" + dateTime.Month;
                string day = dateTime.Day < 10 ? "0" + dateTime.Day : "" + dateTime.Day;
                HttpWebRequest request = WebRequest.Create("http://www.shfe.com.cn/data/instrument/Settlement" + year + month + day + ".dat") as HttpWebRequest;
                request.Method = "get";
                request.CookieContainer = new CookieContainer();
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = 50000;

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("utf-8"));
                string result = sr.ReadToEnd();
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取郑州商品交易所数据
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        private string GetDataZZ(DateTime dateTime)
        {
            try
            {
                //if (dateTime.Date==DateTime.Today && dateTime.Hour < 17)
                //    dateTime = dateTime.AddDays(-1);
                string year = "" + dateTime.Year;
                string month = dateTime.Month < 10 ? "0" + dateTime.Month : "" + dateTime.Month;
                string day = dateTime.Day < 10 ? "0" + dateTime.Day : "" + dateTime.Day;
                HttpWebRequest request = WebRequest.Create("http://www.czce.com.cn/cms/cmsface/czce/exchangefront/calendarnewquery.jsp") as HttpWebRequest;
                string postData = string.Format("pubDate={0}&dataType=CLEARPARAMS", year + "-" + month + "-" + day);
                request.Method = "post";
                request.CookieContainer = new CookieContainer();
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postData.Length;
                request.Timeout = 50000;
                StreamWriter sw = new StreamWriter(request.GetRequestStream());
                sw.Write(postData);
                sw.Flush();

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream s = response.GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.GetEncoding("gb2312"));
                string result = sr.ReadToEnd();
                sr.Dispose();
                sr.Close();
                s.Dispose();
                s.Close();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 下载交易所手续费列表
        /// </summary>
        /// <param name="sync">是否以线程同步方式运行。</param>
        public void Initialize(bool sync = true)
        {
            blTmp = Helpers._Helper.GetParameter(ParameterName.CommissionRation.ToString(), "2");
            _textBox倍率.Text = blTmp;
            QueryData();
        }

        class Settement
        {
            /// <summary>
            /// 合约代码
            /// </summary>
            public string InstrumentId { get; set; }
            /// <summary>
            /// 品种名称
            /// </summary>
            public string Commodity { get; set; }
            /// <summary>
            /// 结算价
            /// </summary>
            public decimal SettlementPrice { get; set; }
            /// <summary>
            /// 开仓手续费率
            /// </summary>
            public decimal? TradeFeeRation { get; set; }
            /// <summary>
            /// 平今手续费率
            /// </summary>
            public decimal? CloseTodayTradeFeeRation
            {
                get
                {
                    return TradeFeeRation * DiscountRate;
                }
            }
            /// <summary>
            /// 开平今手续费率
            /// </summary>
            public decimal? TotalTodayTradeFeeRation
            {
                get
                {
                    return (TradeFeeRation + CloseTodayTradeFeeRation);
                }
            }
            /// <summary>
            /// 交易手续费额(元/手)
            /// </summary>
            public decimal? TradeFeeUnit { get; set; }
            /// <summary>
            /// 交割手续费
            /// </summary>
            public decimal CommoditydeLiveryFeeUnit { get; set; }
            /// <summary>
            /// 投机买保证金率
            /// </summary>
            public decimal LongMarginRatio { get; set; }
            /// <summary>
            /// 投机卖保证金率
            /// </summary>
            public decimal ShortMarginRatio { get; set; }
            /// <summary>
            /// 套保买保证金率
            /// </summary>
            public decimal SpecLongMarginRatio { get; set; }
            /// <summary>
            /// 套保卖保证金率
            /// </summary>
            public decimal SpecShortMarginRatio { get; set; }
            /// <summary>
            /// 保证金
            /// </summary>
            public decimal Margin
            {
                get
                {
                    return Worth * LongMarginRatio;
                }
            }
            /// <summary>
            /// 平今折扣率
            /// </summary>
            public decimal DiscountRate { get; set; }
            /// <summary>
            /// 背景标记
            /// </summary>
            public bool BGMark { get; set; }
            /// <summary>
            /// 手续费标记 false为费率，true为费额
            /// </summary>
            public bool FeeMark { get; set; }
            public string RationColor { get; set; }
            public string UnitColor { get; set; }
            /// <summary>
            /// 合约价值
            /// </summary>
            public decimal Worth { get; set; }
        }

        /// <summary>
        /// 填充上海商品交易所数据
        /// </summary>
        /// <param name="dateTime"></param>
        private void FillDataSH(DateTime dateTime)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _loading.Visibility = System.Windows.Visibility.Visible;
                sh = false;
            }));
            decimal bl = 0;
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    bl = Decimal.Parse(_textBox倍率.Text.Trim());
                }));
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _loading.Visibility = System.Windows.Visibility.Hidden;
                }));
                MessageBox.Show("倍率必须为有效的小数或整数。");
                return;
            }

            try
            {
                List<Settement> settements = new List<Settement>();
                var json = JObject.Parse(GetDataSH(dateTime));
                var array = json.GetValue("Settlement");
                string name = "";
                bool inn = false;
                for (int i = 0; i < array.Count(); i++)
                {
                    Settement settement = new Settement();
                    JObject data = array[i] as JObject;
                    // 合约代码
                    var instrumentId = data.GetValue("INSTRUMENTID");
                    settement.InstrumentId = instrumentId.ToString().Trim();
                    // 结算价
                    var settlementPrice = data.GetValue("SETTLEMENTPRICE");
                    if (String.IsNullOrEmpty(settlementPrice.ToString()))
                    {
                        throw new Exception("(404) 未找到");
                    }
                    settement.SettlementPrice = Decimal.Parse(settlementPrice.ToString());

                    // 交易手续费率
                    var tradeFeeRation = data.GetValue("TRADEFEERATION");
                    var aaa = Decimal.Parse(tradeFeeRation.ToString());
                    if (aaa != 0)
                    {
                        settement.TradeFeeRation = aaa * bl;
                        settement.FeeMark = false;
                        settement.RationColor = "Red";
                        settement.UnitColor = "Black";
                    }
                    // 交易手续费额(元/手)
                    var tradeFeeUnit = data.GetValue("TRADEFEEUNIT");
                    var bbb = Decimal.Parse(tradeFeeUnit.ToString());
                    if (bbb != 0)
                    {
                        settement.TradeFeeUnit = bbb * bl;
                        settement.FeeMark = true;
                        settement.RationColor = "Black";
                        settement.UnitColor = "Red";
                    }

                    // 自动配置
                    AutoSettement(settement);
                    // 交割手续费
                    var commoditydeLiveryFeeUnit = data.GetValue("COMMODITYDELIVERYFEEUNIT");
                    settement.CommoditydeLiveryFeeUnit = Decimal.Parse(commoditydeLiveryFeeUnit.ToString());
                    // 投机买保证金率
                    var longMarginRatio = data.GetValue("LONGMARGINRATIO");
                    settement.LongMarginRatio = Decimal.Parse(longMarginRatio.ToString());
                    // 投机卖保证金率
                    var shortMarginRatio = data.GetValue("SHORTMARGINRATIO");
                    settement.ShortMarginRatio = Decimal.Parse(shortMarginRatio.ToString());
                    // 套保买保证金率
                    var spec_longMarginRatio = data.GetValue("SPEC_LONGMARGINRATIO");
                    settement.SpecLongMarginRatio = Decimal.Parse(spec_longMarginRatio.ToString());
                    // 套保卖保证金率
                    var spec_shortMarginRatio = data.GetValue("SPEC_SHORTMARGINRATIO");
                    settement.SpecShortMarginRatio = Decimal.Parse(spec_shortMarginRatio.ToString());
                    // 平今折扣率
                    var discountRate = data.GetValue("DISCOUNTRATE");
                    settement.DiscountRate = Decimal.Parse(discountRate.ToString());

                    //var commodityDeliveryFeeRation = data.GetValue("COMMODITYDELIVERYFEERATION");
                    //var tradingDay = data.GetValue("TRADINGDAY");
                    //var updateDate = data.GetValue("UPDATE_DATE");
                    //var id = data.GetValue("id");

                    settements.Add(settement);
                }
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    var order = settements.GroupBy(m => m.Commodity).OrderBy(m => m.Min(n => n.TotalTodayTradeFeeRation));
                    List<Settement> source = new List<Settement>();
                    foreach (var or in order)
                    {
                        var comm = or.Last().Commodity;
                        if (!name.Equals(comm))
                        {
                            inn = !inn;
                            name = comm;
                        }
                        var setts = settements.Where(m => m.Commodity.Equals(comm)).OrderBy(m => m.InstrumentId);
                        foreach (var st in setts)
                        {
                            st.BGMark = inn;
                        }
                        source.AddRange(setts);
                    }
                    _dataGridSH.ItemsSource = source;
                }));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(404) 未找到"))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _dataGridSH.ItemsSource = null;
                    }));
                    MessageBox.Show("交易所不存在[" + dateTime.ToShortDateString() + "]的数据。");
                }
                else
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                }
            }
            finally
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    sh = true;
                    if (sh && zz && dl)
                        _loading.Visibility = System.Windows.Visibility.Hidden;
                }));
            }
        }

        /// <summary>
        /// 填充郑州商品交易所数据
        /// </summary>
        /// <param name="dateTime"></param>
        private void FillDataZZ(DateTime dateTime)
        {
            Dispatcher.Invoke(new FormControlInvoker(() =>
            {
                _loading.Visibility = System.Windows.Visibility.Visible;
                zz = false;
            }));
            decimal bl = 0;
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    bl = Decimal.Parse(_textBox倍率.Text.Trim());
                }));
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _loading.Visibility = System.Windows.Visibility.Hidden;
                }));
                MessageBox.Show("倍率必须为有效的小数或整数。");
                return;
            }

            try
            {
                List<Settement> settements = new List<Settement>();
                var result = GetDataZZ(dateTime);
                string name = "";
                bool inn = false;

                if (result.Contains("无交易记录！"))
                {
                    MessageBox.Show("交易所不存在[" + dateTime.ToShortDateString() + "]的数据。");
                    return;
                }
                result = result.Substring(result.IndexOf("平今手续费减半"));
                result = result.Substring(0, result.IndexOf("</table>"));

                while (true)
                {
                    Settement settement = new Settement();
                    result = result.Substring(result.IndexOf("<td>") + 4);
                    string item = result.Substring(0, result.IndexOf("</td>"));
                    settement.InstrumentId = item;

                    result = result.Substring(result.IndexOf("\">") + 2);
                    string price = result.Substring(0, result.IndexOf("</td>"));
                    settement.SettlementPrice = Decimal.Parse(price);


                    result = result.Substring(result.IndexOf("\">") + 2);
                    result = result.Substring(result.IndexOf("\">") + 2);

                    result = result.Substring(result.IndexOf("\">") + 2);
                    string buyMargin = result.Substring(0, result.IndexOf("</td>"));
                    settement.LongMarginRatio = Decimal.Parse(buyMargin) / 100;

                    result = result.Substring(result.IndexOf("\">") + 2);
                    string saleMargin = result.Substring(0, result.IndexOf("</td>"));
                    settement.ShortMarginRatio = Decimal.Parse(saleMargin) / 100;

                    result = result.Substring(result.IndexOf("\">") + 2);
                    string fee = result.Substring(0, result.IndexOf("</td>"));
                    settement.TradeFeeUnit = Decimal.Parse(fee) * bl;
                    settement.FeeMark = true;
                    settement.RationColor = "Black";
                    settement.UnitColor = "Red";
                    AutoSettement(settement);
                    if (!name.Equals(settement.Commodity))
                    {
                        inn = !inn;
                        name = settement.Commodity;
                    }
                    settement.BGMark = inn;

                    result = result.Substring(result.IndexOf("\">") + 2);

                    result = result.Substring(result.IndexOf("\">") + 2);
                    string pingjin = result.Substring(0, result.IndexOf("</td>"));
                    if (pingjin.Equals("Y"))
                    {
                        settement.DiscountRate = 0.5m;
                    }
                    else
                    {
                        settement.DiscountRate = 1m;
                    }
                    settements.Add(settement);

                    if (result.IndexOf("<tr>") < 0)
                        break;
                    else
                        result = result.Substring(result.IndexOf("<tr>") + 2);
                }


                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    var order = settements.GroupBy(m => m.Commodity).OrderBy(m => m.Min(n => n.TotalTodayTradeFeeRation));
                    List<Settement> source = new List<Settement>();
                    foreach (var or in order)
                    {
                        var comm = or.Last().Commodity;
                        if (!name.Equals(comm))
                        {
                            inn = !inn;
                            name = comm;
                        }
                        var setts = settements.Where(m => m.Commodity.Equals(comm)).OrderBy(m => m.InstrumentId);
                        foreach (var st in setts)
                        {
                            st.BGMark = inn;
                        }
                        source.AddRange(setts);
                    }
                    _dataGridZZ.ItemsSource = source;
                }));
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("(404) 未找到"))
                {
                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _dataGridZZ.ItemsSource = null;
                    }));
                    MessageBox.Show("交易所不存在[" + dateTime.ToShortDateString() + "]的数据。");
                }
                else
                {
                    MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace);
                }
            }
            finally
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    zz = true;
                    if (sh && zz && dl)
                        _loading.Visibility = System.Windows.Visibility.Hidden;
                }));
            }
        }

        private void AutoSettement(Settement settement)
        {
            // 得到品种
            string it = settement.InstrumentId.Substring(0, 2);
            switch (it)
            {
                case "cu":
                    settement.Commodity = "铜";
                    settement.Worth = settement.SettlementPrice * 5;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "al":
                    settement.Commodity = "铝";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "zn":
                    settement.Commodity = "锌";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "pb":
                    settement.Commodity = "铅";
                    settement.Worth = settement.SettlementPrice * 5;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "ru":
                    settement.Commodity = "橡胶";
                    settement.Worth = settement.SettlementPrice * 10;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "fu":
                    settement.Commodity = "燃油";
                    settement.Worth = settement.SettlementPrice * 50;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "rb":
                    settement.Commodity = "螺纹";
                    settement.Worth = settement.SettlementPrice * 10;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "wr":
                    settement.Commodity = "线材";
                    settement.Worth = settement.SettlementPrice * 10;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "au":
                    settement.Commodity = "黄金";
                    settement.Worth = settement.SettlementPrice * 1000;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "ag":
                    settement.Commodity = "白银";
                    settement.Worth = settement.SettlementPrice * 15;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "bu":
                    settement.Commodity = "沥青";
                    settement.Worth = settement.SettlementPrice * 10;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "hc":
                    settement.Commodity = "热卷";
                    settement.Worth = settement.SettlementPrice * 10;
                    //settement.TradeFeeUnit = settement.Worth * settement.TradeFeeRation / 10000;
                    break;
                case "ni":
                    settement.Commodity = "镍";
                    settement.Worth = settement.SettlementPrice;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "sn":
                    settement.Commodity = "锡";
                    settement.Worth = settement.SettlementPrice;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;



                case "CF":
                    settement.Commodity = "棉花";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "FG":
                    settement.Commodity = "玻璃";
                    settement.Worth = settement.SettlementPrice * 20;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "JR":
                    settement.Commodity = "粳稻";
                    settement.Worth = settement.SettlementPrice * 20;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "LR":
                    settement.Commodity = "晚籼";
                    settement.Worth = settement.SettlementPrice * 20;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "MA":
                    settement.Commodity = "甲醇";
                    settement.Worth = settement.SettlementPrice * 10;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "OI":
                    settement.Commodity = "菜油";
                    settement.Worth = settement.SettlementPrice * 10;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "PM":
                    settement.Commodity = "普麦";
                    settement.Worth = settement.SettlementPrice * 50;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "RI":
                    settement.Commodity = "早籼";
                    settement.Worth = settement.SettlementPrice * 20;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "RM":
                    settement.Commodity = "菜粕";
                    settement.Worth = settement.SettlementPrice * 10;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "RS":
                    settement.Commodity = "菜籽";
                    settement.Worth = settement.SettlementPrice * 10;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "SF":
                    settement.Commodity = "硅铁";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "SM":
                    settement.Commodity = "锰硅";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "SR":
                    settement.Commodity = "白糖";
                    settement.Worth = settement.SettlementPrice * 10;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "TA":
                    settement.Commodity = "PTA";
                    settement.Worth = settement.SettlementPrice * 5;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "TC":
                    settement.Commodity = "动力煤";
                    settement.Worth = settement.SettlementPrice * 200;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "WH":
                    settement.Commodity = "强麦";
                    settement.Worth = settement.SettlementPrice * 20;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
                case "ZC":
                    settement.Commodity = "动力煤";
                    settement.Worth = settement.SettlementPrice * 100;
                    settement.TradeFeeRation = settement.TradeFeeUnit / settement.Worth;
                    break;
            }


        }

        private void _btn手续费查询_Click(object sender, RoutedEventArgs e)
        {
            if (blTmp != _textBox倍率.Text)
            {
                blTmp = _textBox倍率.Text.Trim();
                Helpers._Helper.SetParameter(ParameterName.CommissionRation.ToString(), blTmp);
            }
            QueryData();
        }

        private void QueryData()
        {
            using (StatementContext statement = new StatementContext())
            {
                var fs = statement.Stocks.LastOrDefault();
                if (fs != null)
                {
                    DateTime queryDate = fs.Date;
                    _datePicker手续费日期.Text = queryDate.ToShortDateString();
                    //if (_tab数据.SelectedItem == this._tabItem上海期货交易所)
                    new Thread(new ThreadStart(() =>
                    {
                        FillDataSH(queryDate);
                    })).Start();
                    //else if (_tab数据.SelectedItem == this._tabItem郑州期货交易所)
                    new Thread(new ThreadStart(() =>
                    {
                        FillDataZZ(queryDate);
                    })).Start();
                }
            }
        }

        private void _textBox倍率_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            decimal bl = 0;
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    bl = Decimal.Parse(_textBox倍率.Text.Trim());
                }));
            }
            catch (Exception)
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _loading.Visibility = System.Windows.Visibility.Hidden;
                }));
                MessageBox.Show("倍率必须为有效的小数或整数。");
                return;
            }
            if (e.Delta == 120)
            {
                bl += 0.1m;
            }
            else if (e.Delta == -120)
            {
                bl -= 0.1m;
            }
            _textBox倍率.Text = bl.ToString();
        }
    }
}
