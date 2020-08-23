using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuturesAssistantWPF.Windows
{
    public class Movie
    {
        public DateTime Date { set; get; }
        public string Title { get; set; }
    }
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : System.Windows.Window
    {
        private delegate void FormControlInvoker();
        public TestWindow()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //StatementStream oldStream = new StatementStream();
            //StatementStream newStream = new StatementStream("d:\\aaaa\\bbbbb\\asdf.db");
            //newStream.Append(oldStream.GetAccounts());
            //newStream.Append(oldStream.GetClosedTradeDetails());
            //newStream.Append(oldStream.GetCommoditys());
            //newStream.Append(oldStream.GetCommoditySummarizations());
            //newStream.Append(oldStream.GetFundStatus());
            //newStream.Append(oldStream.GetParameters());
            //newStream.Append(oldStream.GetPositionDetails());
            //newStream.Append(oldStream.GetPositions());
            //newStream.Append(oldStream.GetRemittances());
            //newStream.Append(oldStream.GetStocks());
            //newStream.Append(oldStream.GetTradeDetails());
            //newStream.Append(oldStream.GetTrades());
            //newStream.Append(oldStream.GetUsers());
            //int a = 9;
        }
    }
}