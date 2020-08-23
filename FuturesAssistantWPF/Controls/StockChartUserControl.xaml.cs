using ChineseAlmanac;
using FuturesAssistantWPF.Helpers;
using FuturesAssistantWPF.Models;
using FuturesAssistantWPF.Types;
using FuturesAssistantWPF.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
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
    public enum ChartCycle
    {
        日线, 周线, 月线, 年线
    }
    /// <summary>
    /// StockChartUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class StockChartUserControl : System.Windows.Controls.UserControl
    {
        private delegate void FormControlInvoker();

        // 定义变量
        #region
        private bool _showCursorXY = false;
        private bool _chartInfoBoardMove = false;
        private int _mouseX;
        private int _mouseY;
        private List<Stock> _currentCycleStocks;
        private List<Stock> _stocks;
        private List<Stock> _dayStocks;
        private List<Stock> _weekStocks;
        private List<Stock> _monthStocks;
        private List<Stock> _yearStocks;
        private ChartCycle _cycle;
        private int _average1Value;
        private int _average2Value;
        private int _average3Value;
        private System.Drawing.Color _average1Color;
        private System.Drawing.Color _average2Color;
        private System.Drawing.Color _average3Color;
        private bool _average1Visibility;
        private bool _average2Visibility;
        private bool _average3Visibility;
        private int _stockCount;


        //
        private bool _statisticsBetween = false;
        private bool _showStatisticsCursorXY = false;
        Stock _beginStock, _stopStock;


        // _stockChart 变量定义
        ChartArea chartAreaInfo;
        ChartArea chartAreaPrice;
        ChartArea chartAreaVolume;
        Series seriesPrice;
        Series seriesVolume;
        Series seriesAverage1;
        Series seriesAverage2;
        Series seriesAverage3;
        Title titleInfo;
        Title titleAverage;


        // _panel信息板 变量定义
        private System.Windows.Forms.Panel _panel信息板;
        private System.Windows.Forms.Label _label日期;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label _label涨幅;
        private System.Windows.Forms.Label _label利润;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label _label最低;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label _label量;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label _label最高;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label _label收盘;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label _label开盘;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label _label当前值;
        private System.Windows.Forms.Label label3;

        // _contextMenuStrip图表菜单 变量定义
        private SaveFileDialog _saveFileDialog保存到图片文件;
        private ContextMenuStrip _contextMenuStrip图表菜单;

        private ToolStripMenuItem _ToolStripMenuItem账户合并显示;
        private ToolStripSeparator toolStripSeparator0;
        private ToolStripMenuItem _ToolStripMenuItem日线;
        private ToolStripMenuItem _ToolStripMenuItem周线;
        private ToolStripMenuItem _ToolStripMenuItem月线;
        private ToolStripMenuItem _ToolStripMenuItem年线;

        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem _ToolStripMenuItem蜡烛线;
        private ToolStripMenuItem _ToolStripMenuItem收盘线;
        //private ToolStripMenuItem _ToolStripMenuItem图表起始时间;
        private ToolStripMenuItem _ToolStripMenuItem保存到;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem _toolStripMenuItem均线;
        private ToolStripMenuItem _toolStripMenuItem均线参数;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem _toolStripMenuItem区间统计;
        #endregion

        public StockChartUserControl()
        {
            InitializeComponent();
            InitializeCustomComponent();
            LoadSolarTerm();
        }

        private void InitializeCustomComponent()
        {
            // 控件变量赋值
            #region

            //
            _weekStocks = new List<Stock>();
            _monthStocks = new List<Stock>();
            _yearStocks = new List<Stock>();
            //
            _contextMenuStrip图表菜单 = new System.Windows.Forms.ContextMenuStrip();
            _ToolStripMenuItem账户合并显示 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator0 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripMenuItem日线 = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItem周线 = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItem月线 = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItem年线 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripMenuItem蜡烛线 = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItem收盘线 = new System.Windows.Forms.ToolStripMenuItem();
            _toolStripMenuItem均线 = new System.Windows.Forms.ToolStripMenuItem();
            _toolStripMenuItem均线参数 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            //this._ToolStripMenuItem图表起始时间 = new System.Windows.Forms.ToolStripMenuItem();
            _ToolStripMenuItem保存到 = new System.Windows.Forms.ToolStripMenuItem();
            _panel信息板 = new System.Windows.Forms.Panel();
            label17 = new System.Windows.Forms.Label();
            label13 = new System.Windows.Forms.Label();
            _label涨幅 = new System.Windows.Forms.Label();
            _label利润 = new System.Windows.Forms.Label();
            _label最低 = new System.Windows.Forms.Label();
            label15 = new System.Windows.Forms.Label();
            _label量 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            _label最高 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            _label收盘 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            _label开盘 = new System.Windows.Forms.Label();
            label5 = new System.Windows.Forms.Label();
            _label当前值 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            _label日期 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            _saveFileDialog保存到图片文件 = new System.Windows.Forms.SaveFileDialog();
            _toolStripMenuItem区间统计 = new System.Windows.Forms.ToolStripMenuItem();


            chartAreaInfo = new ChartArea();
            chartAreaPrice = new ChartArea();
            chartAreaVolume = new ChartArea();
            seriesPrice = new Series();
            seriesVolume = new Series();
            seriesAverage1 = new Series();
            seriesAverage2 = new Series();
            seriesAverage3 = new Series();
            titleInfo = new Title();
            titleAverage = new Title();
            #endregion

            // 设置控件参数
            #region


            // 
            // _contextMenuStrip图表菜单
            // 
            _contextMenuStrip图表菜单.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            _ToolStripMenuItem日线,
            _ToolStripMenuItem周线,
            _ToolStripMenuItem月线,
            _ToolStripMenuItem年线,
            toolStripSeparator1,
            _ToolStripMenuItem蜡烛线,
            _ToolStripMenuItem收盘线,
            toolStripSeparator2,
            _toolStripMenuItem均线,
            _toolStripMenuItem均线参数,
            toolStripSeparator3,
            _ToolStripMenuItem账户合并显示,
            _toolStripMenuItem区间统计,
            //this._ToolStripMenuItem图表起始时间,
            //this._ToolStripMenuItem保存到
            });
            _contextMenuStrip图表菜单.Name = "_contextMenuStrip图表菜单";
            _contextMenuStrip图表菜单.Size = new System.Drawing.Size(166, 336);
            // 
            // _ToolStripMenuItem日线
            // 
            _ToolStripMenuItem账户合并显示.Name = "_ToolStripMenuItem账户合并显示";
            _ToolStripMenuItem账户合并显示.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem账户合并显示.Text = "账户合并显示";
            _ToolStripMenuItem账户合并显示.Click += new System.EventHandler(_toolStripLabel账户合并显示_Click);

            // 
            // toolStripSeparator0
            // 
            toolStripSeparator0.Name = "toolStripSeparator0";
            toolStripSeparator0.Size = new System.Drawing.Size(162, 6);

            // 
            // _ToolStripMenuItem日线
            // 
            _ToolStripMenuItem日线.Name = "_ToolStripMenuItem日线";
            _ToolStripMenuItem日线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem日线.Text = "日线";
            _ToolStripMenuItem日线.Click += new System.EventHandler(_toolStripLabel日线_Click);
            // 
            // _ToolStripMenuItem周线
            // 
            _ToolStripMenuItem周线.Name = "_ToolStripMenuItem周线";
            _ToolStripMenuItem周线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem周线.Text = "周线";
            _ToolStripMenuItem周线.Click += new System.EventHandler(_toolStripLabel周线_Click);
            // 
            // _ToolStripMenuItem月线
            // 
            _ToolStripMenuItem月线.Name = "_ToolStripMenuItem月线";
            _ToolStripMenuItem月线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem月线.Text = "月线";
            _ToolStripMenuItem月线.Click += new System.EventHandler(_toolStripLabel月线_Click);
            // 
            // _ToolStripMenuItem年线
            // 
            _ToolStripMenuItem年线.Name = "_ToolStripMenuItem年线";
            _ToolStripMenuItem年线.ShowShortcutKeys = false;
            _ToolStripMenuItem年线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem年线.Text = "年线";
            _ToolStripMenuItem年线.Click += new System.EventHandler(_toolStripLabel年线_Click);
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new System.Drawing.Size(162, 6);
            // 
            // _ToolStripMenuItem蜡烛线
            // 
            _ToolStripMenuItem蜡烛线.Name = "_ToolStripMenuItem蜡烛线";
            _ToolStripMenuItem蜡烛线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem蜡烛线.Text = "蜡烛线 (K)";
            _ToolStripMenuItem蜡烛线.Click += new System.EventHandler(_ToolStripMenuItem蜡烛线_Click);
            // 
            // _ToolStripMenuItem收盘线
            // 
            _ToolStripMenuItem收盘线.Name = "_ToolStripMenuItem收盘线";
            _ToolStripMenuItem收盘线.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem收盘线.Text = "收盘线 (C)";
            _ToolStripMenuItem收盘线.Click += new System.EventHandler(_ToolStripMenuItem收盘线_Click);
            // 
            // _toolStripMenuItem均线
            // 
            _toolStripMenuItem均线.Name = "_toolStripMenuItem均线";
            _toolStripMenuItem均线.Size = new System.Drawing.Size(165, 22);
            _toolStripMenuItem均线.Text = "均线 (空格)";
            _toolStripMenuItem均线.Click += new System.EventHandler(_toolStripMenuItem均线_Click);
            // 
            // _toolStripMenuItem均线参数
            // 
            _toolStripMenuItem均线参数.Name = "_toolStripMenuItem均线参数";
            _toolStripMenuItem均线参数.Size = new System.Drawing.Size(165, 22);
            _toolStripMenuItem均线参数.Text = "均线参数";
            _toolStripMenuItem均线参数.Click += new System.EventHandler(_toolStripMenuItem均线参数_Click);

            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(162, 6);

            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new System.Drawing.Size(162, 6);
            // 
            // _ToolStripMenuItem图表起始时间
            // 
            //this._ToolStripMenuItem图表起始时间.Name = "_ToolStripMenuItem图表起始时间";
            //this._ToolStripMenuItem图表起始时间.Size = new System.Drawing.Size(165, 22);
            //this._ToolStripMenuItem图表起始时间.Text = "图表起始时间";
            //this._ToolStripMenuItem图表起始时间.Click += new System.EventHandler(this._ToolStripMenuItem图表起始时间_Click);
            // 
            // _ToolStripMenuItem保存到
            // 
            _ToolStripMenuItem保存到.Name = "_ToolStripMenuItem保存到";
            _ToolStripMenuItem保存到.Size = new System.Drawing.Size(165, 22);
            _ToolStripMenuItem保存到.Text = "保存到...";
            _ToolStripMenuItem保存到.Click += new System.EventHandler(_ToolStripMenuItem保存到_Click);
            // 
            // _panel信息板
            // 
            _panel信息板.BackColor = System.Drawing.Color.Black;
            _panel信息板.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            _panel信息板.Controls.Add(_label涨幅);
            _panel信息板.Controls.Add(_label利润);
            _panel信息板.Controls.Add(_label日期);
            _panel信息板.Controls.Add(_label当前值);
            _panel信息板.Controls.Add(_label开盘);
            _panel信息板.Controls.Add(_label收盘);
            _panel信息板.Controls.Add(_label最高);
            _panel信息板.Controls.Add(_label最低);
            _panel信息板.Controls.Add(_label量);
            _panel信息板.Controls.Add(label17);
            _panel信息板.Controls.Add(label13);
            _panel信息板.Controls.Add(label15);
            _panel信息板.Controls.Add(label11);
            _panel信息板.Controls.Add(label9);
            _panel信息板.Controls.Add(label7);
            _panel信息板.Controls.Add(label5);
            _panel信息板.Controls.Add(label3);
            _panel信息板.Controls.Add(label1);
            _panel信息板.Location = new System.Drawing.Point(3, 160);
            _panel信息板.Name = "_panel信息板";
            _panel信息板.Size = new System.Drawing.Size(100, 298);
            _panel信息板.TabIndex = 2;
            _panel信息板.Visible = false;
            _panel信息板.MouseDown += new System.Windows.Forms.MouseEventHandler(_panel信息板_MouseDown);
            _panel信息板.MouseMove += new System.Windows.Forms.MouseEventHandler(_panel信息板_MouseMove);
            _panel信息板.MouseUp += new System.Windows.Forms.MouseEventHandler(_panel信息板_MouseUp);
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.BackColor = System.Drawing.Color.Black;
            label17.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label17.ForeColor = System.Drawing.Color.White;
            label17.Location = new System.Drawing.Point(3, 261);
            label17.Name = "label17";
            label17.Size = new System.Drawing.Size(29, 12);
            label17.TabIndex = 0;
            label17.Text = "涨幅";
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.BackColor = System.Drawing.Color.Black;
            label13.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label13.ForeColor = System.Drawing.Color.White;
            label13.Location = new System.Drawing.Point(3, 197);
            label13.Name = "label13";
            label13.Size = new System.Drawing.Size(41, 12);
            label13.TabIndex = 0;
            label13.Text = "成交额";
            // 
            // _label涨幅
            // 
            _label涨幅.BackColor = System.Drawing.Color.Black;
            _label涨幅.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label涨幅.ForeColor = System.Drawing.Color.White;
            _label涨幅.Location = new System.Drawing.Point(3, 276);
            _label涨幅.Name = "_label涨幅";
            _label涨幅.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label涨幅.TabIndex = 0;
            _label涨幅.Text = " ";
            _label涨幅.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _label利润
            // 
            _label利润.BackColor = System.Drawing.Color.Black;
            _label利润.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label利润.ForeColor = System.Drawing.Color.White;
            _label利润.Location = new System.Drawing.Point(3, 242);
            _label利润.Name = "_label利润";
            _label利润.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label利润.TabIndex = 0;
            _label利润.Text = " ";
            _label利润.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // _label最低
            // 
            _label最低.BackColor = System.Drawing.Color.Black;
            _label最低.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label最低.ForeColor = System.Drawing.Color.White;
            _label最低.Location = new System.Drawing.Point(3, 180);
            _label最低.Name = "_label最低";
            _label最低.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label最低.TabIndex = 0;
            _label最低.Text = " ";
            _label最低.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = System.Drawing.Color.Black;
            label15.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label15.ForeColor = System.Drawing.Color.White;
            label15.Location = new System.Drawing.Point(3, 229);
            label15.Name = "label15";
            label15.Size = new System.Drawing.Size(29, 12);
            label15.TabIndex = 0;
            label15.Text = "利润";
            // 
            // _label量
            // 
            _label量.BackColor = System.Drawing.Color.Black;
            _label量.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label量.ForeColor = System.Drawing.Color.Yellow;
            _label量.Location = new System.Drawing.Point(3, 210);
            _label量.Name = "_label量";
            _label量.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label量.TabIndex = 0;
            _label量.Text = " ";
            _label量.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = System.Drawing.Color.Black;
            label11.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label11.ForeColor = System.Drawing.Color.White;
            label11.Location = new System.Drawing.Point(3, 165);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(41, 12);
            label11.TabIndex = 0;
            label11.Text = "最低价";
            // 
            // _label最高
            // 
            _label最高.BackColor = System.Drawing.Color.Black;
            _label最高.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label最高.ForeColor = System.Drawing.Color.White;
            _label最高.Location = new System.Drawing.Point(3, 148);
            _label最高.Name = "_label最高";
            _label最高.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label最高.TabIndex = 0;
            _label最高.Text = " ";
            _label最高.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = System.Drawing.Color.Black;
            label9.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label9.ForeColor = System.Drawing.Color.White;
            label9.Location = new System.Drawing.Point(3, 133);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(41, 12);
            label9.TabIndex = 0;
            label9.Text = "最高价";
            // 
            // _label收盘
            // 
            _label收盘.BackColor = System.Drawing.Color.Black;
            _label收盘.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label收盘.ForeColor = System.Drawing.Color.White;
            _label收盘.Location = new System.Drawing.Point(3, 116);
            _label收盘.Name = "_label收盘";
            _label收盘.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label收盘.TabIndex = 0;
            _label收盘.Text = " ";
            _label收盘.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = System.Drawing.Color.Black;
            label7.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label7.ForeColor = System.Drawing.Color.White;
            label7.Location = new System.Drawing.Point(3, 101);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(41, 12);
            label7.TabIndex = 0;
            label7.Text = "收盘价";
            // 
            // _label开盘
            // 
            _label开盘.BackColor = System.Drawing.Color.Black;
            _label开盘.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label开盘.ForeColor = System.Drawing.Color.White;
            _label开盘.Location = new System.Drawing.Point(3, 84);
            _label开盘.Name = "_label开盘";
            _label开盘.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label开盘.TabIndex = 0;
            _label开盘.Text = " ";
            _label开盘.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = System.Drawing.Color.Black;
            label5.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label5.ForeColor = System.Drawing.Color.White;
            label5.Location = new System.Drawing.Point(3, 69);
            label5.Name = "label5";
            label5.Size = new System.Drawing.Size(41, 12);
            label5.TabIndex = 0;
            label5.Text = "开盘价";
            // 
            // _label当前值
            // 
            _label当前值.BackColor = System.Drawing.Color.Black;
            _label当前值.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label当前值.ForeColor = System.Drawing.Color.White;
            _label当前值.Location = new System.Drawing.Point(3, 52);
            _label当前值.Name = "_label当前值";
            _label当前值.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label当前值.TabIndex = 0;
            _label当前值.Text = " ";
            _label当前值.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = System.Drawing.Color.Black;
            label3.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label3.ForeColor = System.Drawing.Color.White;
            label3.Location = new System.Drawing.Point(3, 37);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(41, 12);
            label3.TabIndex = 0;
            label3.Text = "当前值";
            // 
            // _label日期
            // 
            _label日期.BackColor = System.Drawing.Color.Black;
            _label日期.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            _label日期.ForeColor = System.Drawing.Color.White;
            _label日期.Location = new System.Drawing.Point(3, 20);
            _label日期.Name = "_label日期";
            _label日期.Size = new System.Drawing.Size(_panel信息板.Size.Width - 10, 17);
            _label日期.TabIndex = 0;
            _label日期.Text = " ";
            _label日期.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = System.Drawing.Color.Black;
            label1.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            label1.ForeColor = System.Drawing.Color.White;
            label1.Location = new System.Drawing.Point(3, 5);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(29, 12);
            label1.TabIndex = 0;
            label1.Text = "日期";
            // 
            // _toolStripMenuItem区间统计
            // 
            _toolStripMenuItem区间统计.Name = "_toolStripMenuItem区间统计";
            _toolStripMenuItem区间统计.Size = new System.Drawing.Size(165, 22);
            _toolStripMenuItem区间统计.Text = "区间统计 (Shift)";
            _toolStripMenuItem区间统计.Click += new System.EventHandler(_toolStripMenuItem区间统计_Click);
            //
            // stockChart
            //
            chartAreaInfo.Name = "Info";
            chartAreaInfo.Position.Auto = false;
            chartAreaInfo.Position.Height = 1F;
            chartAreaInfo.Position.Width = 95F;
            chartAreaInfo.Position.X = 1F;
            chartAreaInfo.Position.Y = 2F;
            chartAreaInfo.AxisX.IsMarginVisible = false;

            chartAreaPrice.AxisX.IsLabelAutoFit = false;
            chartAreaPrice.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartAreaPrice.AxisX.LabelStyle.IsEndLabelVisible = false;
            chartAreaPrice.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //chartArea2.AxisX.IsMarginVisible = false;
            chartAreaPrice.AxisX.MajorGrid.Interval = 0D;
            chartAreaPrice.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaPrice.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartAreaPrice.AxisY.InterlacedColor = System.Drawing.Color.Black;
            chartAreaPrice.AxisY.IsLabelAutoFit = false;
            chartAreaPrice.AxisY.IsStartedFromZero = false;
            chartAreaPrice.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartAreaPrice.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaPrice.AxisY.MajorGrid.Interval = 0D;
            chartAreaPrice.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaPrice.BackColor = BackgroundColor;
            chartAreaPrice.BackSecondaryColor = System.Drawing.Color.Black;
            chartAreaPrice.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartAreaPrice.CursorX.IsUserEnabled = true;
            chartAreaPrice.CursorX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartAreaPrice.CursorY.IsUserEnabled = true;
            chartAreaPrice.CursorY.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartAreaPrice.IsSameFontSizeForAllAxes = true;
            chartAreaPrice.Name = "Price";
            chartAreaPrice.Position.Auto = false;
            chartAreaPrice.Position.Height = 65F;
            chartAreaPrice.Position.Width = 95F;
            chartAreaPrice.Position.X = 2F;
            chartAreaPrice.Position.Y = 3F;
            chartAreaPrice.ShadowColor = System.Drawing.Color.Transparent;

            chartAreaVolume.AlignWithChartArea = "Price";
            chartAreaVolume.AxisX.IsLabelAutoFit = false;
            //chartArea3.AxisX.IsMarginVisible = false;
            chartAreaVolume.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartAreaVolume.AxisX.LabelStyle.IsEndLabelVisible = false;
            chartAreaVolume.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaVolume.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaVolume.AxisY.Enabled = System.Windows.Forms.DataVisualization.Charting.AxisEnabled.True;
            chartAreaVolume.AxisY.IsLabelAutoFit = false;
            chartAreaVolume.AxisY.IsStartedFromZero = false;
            chartAreaVolume.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartAreaVolume.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaVolume.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaVolume.BackColor = BackgroundColor;
            chartAreaVolume.BackSecondaryColor = System.Drawing.Color.White;
            chartAreaVolume.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartAreaVolume.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartAreaVolume.CursorX.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartAreaVolume.Name = "Volume";
            chartAreaVolume.Position.Auto = false;
            chartAreaVolume.Position.Height = 28F;
            chartAreaVolume.Position.Width = 95F;
            chartAreaVolume.Position.X = 2F;
            chartAreaVolume.Position.Y = 72F;
            chartAreaVolume.ShadowColor = System.Drawing.Color.Transparent;


            _stockChart.ChartAreas.Add(chartAreaInfo);
            _stockChart.ChartAreas.Add(chartAreaPrice);
            _stockChart.ChartAreas.Add(chartAreaVolume);
            _stockChart.ContextMenuStrip = _contextMenuStrip图表菜单;
            _stockChart.Name = "_stockChart";
            _stockChart.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Grayscale;

            seriesPrice.BackImageTransparentColor = System.Drawing.Color.White;
            seriesPrice.BackSecondaryColor = System.Drawing.Color.Cyan;
            seriesPrice.BorderColor = PriceBorderColor1;
            seriesPrice.ChartArea = "Price";
            seriesPrice.ChartType = SeriesChartType.Line;
            seriesPrice.Color = PriceColor1;
            seriesPrice.EmptyPointStyle.IsVisibleInLegend = false;
            seriesPrice.EmptyPointStyle.Color = System.Drawing.Color.White;
            seriesPrice.Name = "Price";
            seriesPrice.ShadowColor = System.Drawing.Color.Green;
            seriesPrice.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            seriesPrice.YValuesPerPoint = 4;

            seriesVolume.BorderColor = System.Drawing.Color.Black;
            seriesVolume.ChartArea = "Volume";
            seriesVolume.Color = VolumeColor1;
            seriesVolume.EmptyPointStyle.IsVisibleInLegend = false;
            seriesVolume.Name = "Volume";
            seriesVolume.ShadowColor = System.Drawing.Color.White;
            seriesVolume.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            seriesAverage1.ChartArea = "Price";
            seriesAverage1.ChartType = SeriesChartType.Line;
            seriesAverage1.Name = "Average1";
            seriesAverage2.ChartArea = "Price";
            seriesAverage2.ChartType = SeriesChartType.Line;
            seriesAverage2.Color = System.Drawing.Color.Yellow;
            seriesAverage2.Name = "Average2";
            seriesAverage3.ChartArea = "Price";
            seriesAverage3.ChartType = SeriesChartType.Line;
            seriesAverage3.Color = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            seriesAverage3.Name = "Average3";

            _stockChart.Series.Add(seriesPrice);
            _stockChart.Series.Add(seriesVolume);
            _stockChart.Series.Add(seriesAverage1);
            _stockChart.Series.Add(seriesAverage2);
            _stockChart.Series.Add(seriesAverage3);
            _stockChart.TabIndex = 1;
            titleInfo.Alignment = System.Drawing.ContentAlignment.MiddleCenter;
            titleInfo.DockedToChartArea = "Info";
            titleInfo.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            titleInfo.Name = "Info";
            titleInfo.Text = "";
            titleAverage.Alignment = System.Drawing.ContentAlignment.TopLeft;
            titleAverage.DockedToChartArea = "Price";
            titleAverage.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            titleAverage.ForeColor = System.Drawing.Color.White;
            titleAverage.Name = "Average";
            titleAverage.Text = "";
            _stockChart.Titles.Add(titleInfo);
            _stockChart.Titles.Add(titleAverage);
            _stockChart.AntiAliasing = AntiAliasingStyles.None;
#if false
            this._stockChart.KeyDown += new System.Windows.Forms.KeyEventHandler(this._stockChart_KeyDown);
#endif
            _stockChart.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(_chart_MouseDoubleClick);
            _stockChart.MouseDown += new System.Windows.Forms.MouseEventHandler(_stockChart_MouseDown);
            _stockChart.MouseMove += new System.Windows.Forms.MouseEventHandler(_chart_MouseMove);
            _stockChart.MouseUp += new System.Windows.Forms.MouseEventHandler(_stockChart_MouseUp);
            _stockChart.MouseWheel += _stockChart_MouseWheel;
            //
            _stockChart.Controls.Add(_panel信息板);
            #endregion
        }

        private void _toolStripLabel账户合并显示_Click(object sender, EventArgs e)
        {
            _ToolStripMenuItem账户合并显示.Checked = !_ToolStripMenuItem账户合并显示.Checked;
            _btn合.Content = _ToolStripMenuItem账户合并显示.Checked ? "分" : "合";
            _Helper.SetParameter(ParameterName.IsMergingAccount.ToString(), _ToolStripMenuItem账户合并显示.Checked.ToString());
            RefreshChart();
        }

        public void Initialize()
        {
            try
            {
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    _ToolStripMenuItem账户合并显示.Checked = bool.Parse(_Helper.GetParameter(ParameterName.IsMergingAccount.ToString(), false.ToString()));
                    if (_ToolStripMenuItem账户合并显示.Checked)
                    {
                        _btn合.Content = "分";
                    }

                    this._Refresh();

                    // 开始日期
                    _stockCount = Int32.Parse(_Helper.GetParameter(ParameterName.StockCount.ToString(), "400"));


                    // 均线
                    var averageVisibility = bool.Parse(_Helper.GetParameter(ParameterName.StockChart显示均线.ToString(), false.ToString()));
                    _toolStripMenuItem均线.Checked = averageVisibility;

                    _average1Visibility = Boolean.Parse(_Helper.GetParameter(ParameterName.Average1Visibility.ToString(), true.ToString()));
                    _average2Visibility = Boolean.Parse(_Helper.GetParameter(ParameterName.Average2Visibility.ToString(), true.ToString()));
                    _average3Visibility = Boolean.Parse(_Helper.GetParameter(ParameterName.Average3Visibility.ToString(), true.ToString()));
                    _average1Value = Convert.ToInt32(_Helper.GetParameter(ParameterName.Average1Value.ToString(), "10"));
                    _average2Value = Convert.ToInt32(_Helper.GetParameter(ParameterName.Average2Value.ToString(), "20"));
                    _average3Value = Convert.ToInt32(_Helper.GetParameter(ParameterName.Average3Value.ToString(), "30"));
                    _average1Color = System.Drawing.Color.FromArgb(Convert.ToInt32(_Helper.GetParameter(ParameterName.Average1Color.ToString(), System.Drawing.Color.White.ToArgb().ToString())));
                    _average2Color = System.Drawing.Color.FromArgb(Convert.ToInt32(_Helper.GetParameter(ParameterName.Average2Color.ToString(), System.Drawing.Color.Yellow.ToArgb().ToString())));
                    _average3Color = System.Drawing.Color.FromArgb(Convert.ToInt32(_Helper.GetParameter(ParameterName.Average3Color.ToString(), System.Drawing.Color.Green.ToArgb().ToString())));
                    _stockChart.Titles["Average"].Text = string.Format("均线（{0}，{1}，{2}）", _average1Value, _average2Value, _average3Value);
                    _stockChart.Titles["Average"].Visible = _toolStripMenuItem均线.Checked;

                    // 风格
                    var _chartType = _Helper.GetParameter(ParameterName.StockChart风格.ToString(), SeriesChartType.Line.ToString());
                    if (_chartType.Equals(SeriesChartType.Candlestick.ToString()))
                    {
                        _stockChart.Series["Price"].ChartType = SeriesChartType.Candlestick;
                        _ToolStripMenuItem蜡烛线.Enabled = _btnK.IsEnabled = false;
                        _ToolStripMenuItem收盘线.Enabled = _btnC.IsEnabled = true;
                    }
                    else if (_chartType.Equals(SeriesChartType.Line.ToString()))
                    {
                        _stockChart.Series["Price"].ChartType = SeriesChartType.Line;
                        _ToolStripMenuItem蜡烛线.Enabled = _btnK.IsEnabled = true;
                        _ToolStripMenuItem收盘线.Enabled = _btnC.IsEnabled = false;
                    }

                    // 周期
                    var _chartCycle = _Helper.GetParameter(ParameterName.StockChart周期.ToString(), ChartCycle.日线.ToString());
                    if (_chartCycle.Equals(ChartCycle.日线.ToString()))
                    {
                        _ToolStripMenuItem日线.Enabled = _btn日.IsEnabled = false;
                        _cycle = ChartCycle.日线;
                    }
                    else if (_chartCycle.Equals(ChartCycle.周线.ToString()))
                    {
                        _ToolStripMenuItem周线.Enabled = _btn周.IsEnabled = false;
                        _cycle = ChartCycle.周线;
                    }
                    else if (_chartCycle.Equals(ChartCycle.月线.ToString()))
                    {
                        _ToolStripMenuItem月线.Enabled = _btn月.IsEnabled = false;
                        _cycle = ChartCycle.月线;
                    }
                    else if (_chartCycle.Equals(ChartCycle.年线.ToString()))
                    {
                        _ToolStripMenuItem年线.Enabled = _btn年.IsEnabled = false;
                        _cycle = ChartCycle.年线;
                    }

                    //
                    RefreshChart();
                }));
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        class SmallStock
        {
            public decimal Profit { get; set; }
            public decimal Volume { get; set; }
        }

        public void RefreshChart()
        {
            using (StatementContext statement = new StatementContext(typeof(Stock), typeof(Account)))
            {
                if (_ToolStripMenuItem账户合并显示.Checked)
                {
                    _stocks = new List<Stock>();
                    IDictionary<DateTime, SmallStock> profits = new Dictionary<DateTime, SmallStock>();
                    decimal money = 0m;
                    var accountList = statement.Accounts.Where(m => m.UserId == _Session.LoginedUserId);
                    foreach (var account in accountList)
                    {
                        var stocks = statement.Stocks.Where(model => model.AccountId == account.Id).OrderByDescending(model => model.Date);

                        // 取得当前账户余额
                        var lastStock = stocks.FirstOrDefault();
                        if (lastStock != null)
                        {
                            money += lastStock.Close;
                        }

                        // 取得每天赢损
                        foreach (var stock in stocks)
                        {
                            var exit = profits.ContainsKey(stock.Date);
                            if (exit)
                            {
                                profits[stock.Date].Profit += stock.Close - stock.Open;
                                profits[stock.Date].Volume += stock.Volume;
                            }
                            else
                            {
                                profits.Add(stock.Date, new SmallStock { Profit = stock.Close - stock.Open, Volume = stock.Volume });
                            }
                        }
                    }

                    // 逆向生成股票图数据
                    foreach (var profit in profits.OrderByDescending(model => model.Key))
                    {
                        Stock stock = new Stock();
                        stock.Date = profit.Key;
                        stock.Close = money;
                        stock.Open = money - profit.Value.Profit;
                        if (profit.Value.Profit > 0)
                        {
                            stock.High = stock.Close;
                            stock.Low = stock.Open;
                        }
                        else
                        {
                            stock.High = stock.Open;
                            stock.Low = stock.Close;
                        }
                        stock.Volume = profit.Value.Volume;
                        _stocks.Add(stock);
                        money = stock.Open;
                    }

                    // 重置股票图数据顺序
                    //_stocks = _dayStocks.ToList();
                    //this._currentCycleStocks = this._stocks.ToList();
                }
                else
                {
                    _stocks = statement.Stocks.Where(o => o.AccountId == _Session.SelectedAccountId).OrderByDescending(o => o.Date).ToList();
                }

                //this._currentCycleStocks = this._stocks;

                //
                switch (_cycle)
                {
                    case ChartCycle.日线:
                        RefreshDayChart();
                        break;
                    case ChartCycle.周线:
                        RefreshWeekChart();
                        break;
                    case ChartCycle.月线:
                        RefreshMonthChart();
                        break;
                    case ChartCycle.年线:
                        RefreshYearChart();
                        break;
                    default: break;
                }

                // 抬头标题
                _stockChart.Titles[0].Text = RefreshTitle();

                // 均线
                RefreshAverageLine1();
                RefreshAverageLine2();
                RefreshAverageLine3();

                // 默认均线标题显示最后一天均线数据
                LatestAverageTitle();
            }
        }

        /// <summary>
        /// 抬头标题
        /// </summary>
        /// <returns></returns>
        private string RefreshTitle()
        {
            var lastCurrentCycleStock = _currentCycleStocks.LastOrDefault();
            if (lastCurrentCycleStock != null && lastCurrentCycleStock.Open != 0)
                return string.Format("<{0}> {1:n}（{2:n} {3:P2}）", _cycle.ToString(),
                    lastCurrentCycleStock.Close,
                    lastCurrentCycleStock.Close - lastCurrentCycleStock.Open,
                    (lastCurrentCycleStock.Close - lastCurrentCycleStock.Open) / lastCurrentCycleStock.Open);
            else
                return string.Format("<{0}> {1:n}（{2:n} {3:P2}）", _cycle.ToString(), 0, 0, 0);
        }

        private void RefreshAverageLine1()
        {
            _stockChart.Series["Average1"].Enabled = _average1Visibility && _toolStripMenuItem均线.Checked;
            if (_average1Visibility)
            {
                _stockChart.Series["Average1"].Points.Clear();
                if (_currentCycleStocks.Count > _average1Value)
                {
                    //

                    List<Stock> averageStocks = new List<Stock>();
                    foreach (var stock in _currentCycleStocks)
                    {
                        averageStocks.Add(stock);
                        if (averageStocks.Count == _average1Value)
                        {
                            decimal average = 0;
                            foreach (var a in averageStocks)
                            {
                                average += a.Close;
                            }
                            average = average / _average1Value;
                            averageStocks.RemoveAt(0);
                            _stockChart.Series["Average1"].Points.AddXY(stock.Date.ToShortDateString(), average);
                        }
                        else
                        {
                            var xIndex = _stockChart.Series["Average1"].Points.AddXY(stock.Date.ToShortDateString(), 0);
                            _stockChart.Series["Average1"].Points[xIndex].IsEmpty = true;
                        }
                    }
                    //
                    _stockChart.Series["Average1"].Color = _average1Color;
                }
            }
        }

        private void RefreshAverageLine2()
        {
            _stockChart.Series["Average2"].Enabled = _average2Visibility && _toolStripMenuItem均线.Checked;
            if (_average2Visibility)
            {
                _stockChart.Series["Average2"].Points.Clear();
                if (_currentCycleStocks.Count > _average2Value)
                {

                    List<Stock> averageStocks = new List<Stock>();
                    foreach (var stock in _currentCycleStocks)
                    {
                        averageStocks.Add(stock);
                        if (averageStocks.Count == _average2Value)
                        {
                            decimal average = 0;
                            foreach (var a in averageStocks)
                            {
                                average += a.Close;
                            }
                            average = average / _average2Value;
                            averageStocks.RemoveAt(0);
                            _stockChart.Series["Average2"].Points.AddXY(stock.Date.ToShortDateString(), average);
                        }
                        else
                        {
                            var xIndex = _stockChart.Series["Average2"].Points.AddXY(stock.Date.ToShortDateString(), 0);
                            _stockChart.Series["Average2"].Points[xIndex].IsEmpty = true;
                        }
                    }
                    //
                    _stockChart.Series["Average2"].Color = _average2Color;
                }
            }
        }

        private void RefreshAverageLine3()
        {
            _stockChart.Series["Average3"].Enabled = _average3Visibility && _toolStripMenuItem均线.Checked;
            if (_average3Visibility)
            {
                _stockChart.Series["Average3"].Points.Clear();
                if (_currentCycleStocks.Count > _average3Value)
                {

                    List<Stock> averageStocks = new List<Stock>();
                    foreach (var stock in _currentCycleStocks)
                    {
                        averageStocks.Add(stock);
                        if (averageStocks.Count == _average3Value)
                        {
                            decimal average = 0;
                            foreach (var a in averageStocks)
                            {
                                average += a.Close;
                            }
                            average = average / _average3Value;
                            averageStocks.RemoveAt(0);
                            _stockChart.Series["Average3"].Points.AddXY(stock.Date.ToShortDateString(), average);
                        }
                        else
                        {
                            var xIndex = _stockChart.Series["Average3"].Points.AddXY(stock.Date.ToShortDateString(), 0);
                            _stockChart.Series["Average3"].Points[xIndex].IsEmpty = true;
                        }
                    }
                    //
                    _stockChart.Series["Average3"].Color = _average3Color;
                }
            }
        }

        System.Drawing.Color BackgroundColor = System.Drawing.Color.Black;

        System.Drawing.Color PriceBorderColor1 = System.Drawing.Color.Red;
        System.Drawing.Color PriceBackSecondaryColor1 = System.Drawing.Color.Black;
        System.Drawing.Color PriceColor1 = System.Drawing.Color.Red;
        System.Drawing.Color VolumeColor1 = System.Drawing.Color.Red;

        System.Drawing.Color PriceBorderColor2 = System.Drawing.Color.Cyan;
        System.Drawing.Color PriceBackSecondaryColor2 = System.Drawing.Color.Cyan;
        System.Drawing.Color PriceColor2 = System.Drawing.Color.Cyan;
        System.Drawing.Color VolumeColor2 = System.Drawing.Color.Cyan;

        private void RefreshDayChart()
        {
            _stockChart.Series["Price"].Points.Clear();
            _stockChart.Series["Volume"].Points.Clear();
            //
            _dayStocks = _stocks.Take(_stockCount).OrderBy(m => m.Date).ToList();
            _currentCycleStocks = _dayStocks;


            int day = 0;
            foreach (var d in _dayStocks)
            {
                if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                {
                    day = _stockChart.Series["Price"].Points.AddXY(d.Date.ToString("yyyyMMdd"), d.Close);
                }
                else
                {
                    day = _stockChart.Series["Price"].Points.AddXY(d.Date.ToString("yyyyMMdd"), d.Low);
                    _stockChart.Series["Price"].Points[day].YValues[1] = Convert.ToDouble(d.High);
                    _stockChart.Series["Price"].Points[day].YValues[2] = Convert.ToDouble(d.Close);
                    _stockChart.Series["Price"].Points[day].YValues[3] = Convert.ToDouble(d.Open);
                }
                //_stockChart.Series["Price"].Points[day].ToolTip = "日期：" + d.Date + "\n开盘价：" + d.Open.ToString("0.00") + "\n最高价：" + d.High.ToString("0.00") + "\n最低价：" + d.Low.ToString("0.00") + "\n收盘价：" + d.Close.ToString("0.00") + "\n成交额：" + d.Volume + "\n利润：" + (d.Close - d.Open).ToString("0.00");
                _stockChart.Series["Volume"].Points.AddXY(d.Date.ToString("yyyyMMdd"), d.Volume);

                if (d.Open <= d.Close)
                {
                    _stockChart.Series["Volume"].Points[day].Color = VolumeColor1;
                    _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor1;
                    _stockChart.Series["Price"].Points[day].BackSecondaryColor = PriceBackSecondaryColor1;
                    _stockChart.Series["Price"].Points[day].Color = PriceColor1;
                }
                else
                {
                    _stockChart.Series["Volume"].Points[day].Color = VolumeColor2;
                    _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor2;
                    _stockChart.Series["Price"].Points[day].Color = PriceColor2;
                }
            }

            // 图表右边距
            for (int i = 0; i < 10; i++)
            {
                DataPoint dp = new DataPoint();
                dp.SetValueXY(" ", 0);
                dp.IsEmpty = true;
                if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                {
                    dp.SetValueXY(" ", 0);
                }
                else
                {
                    dp.SetValueXY(" ", 0, 0, 0, 0);
                }
                _stockChart.Series["Price"].Points.Add(dp);
                _stockChart.Series["Volume"].Points.Add(dp);
            }
        }

        private void RefreshWeekChart()
        {
            _weekStocks.Clear();
            _stockChart.Series["Price"].Points.Clear();
            _stockChart.Series["Volume"].Points.Clear();

            if (_stocks != null && _stocks.Count > 0)
            {
                var firstStock = _stocks.FirstOrDefault();
                DateTime date = firstStock.Date.Date;
                decimal high = firstStock.High;
                decimal open = firstStock.Open;
                decimal close = firstStock.Close;
                decimal low = firstStock.Low;
                decimal volume = 0;

                Stock s = null;
                int count = 0;
                foreach (var ds in _stocks)
                {
                    /*
                     * 星期变小，新的一周开始
                     * 星期不变，新的一周开始（间隔等于7天）
                     * 星期增大
                     *      情况一：一周内数据连续。（间隔小于7天）
                     *      情况二：跳过一周，与下一周数据连续（间隔大于7天）
                     * */
                    //if (ds.Date == new DateTime(2014, 4, 21).Date)
                    //    this._average1Color = this._average1Color;
                    if (ds.Date.DayOfWeek > date.DayOfWeek) //星期变小，新的一周开始
                    {
                        if (++count >= _stockCount)
                            break;
                        s = new Stock();
                        s.Date = date;
                        s.High = high;
                        s.Open = open;
                        s.Close = close;
                        s.Low = low;
                        s.Volume = volume;
                        _weekStocks.Add(s);

                        date = ds.Date.Date;
                        high = ds.High;
                        low = ds.Low;
                        close = ds.Close;
                        open = ds.Open;
                        volume = ds.Volume;
                    }
                    else if ((ds.Date.Date - date.Date).Days >= 7) //间隔大于等于7天，新的一周开始
                    {
                        if (++count >= _stockCount)
                            break;
                        s = new Stock();
                        s.Date = date;
                        s.High = high;
                        s.Open = open;
                        s.Close = close;
                        s.Low = low;
                        s.Volume = volume;
                        _weekStocks.Add(s);

                        date = ds.Date.Date;
                        high = ds.High;
                        low = ds.Low;
                        close = ds.Close;
                        open = ds.Open;
                        volume = ds.Volume;
                    }
                    else
                    {
                        date = ds.Date.Date;
                        high = ds.High > high ? ds.High : high;
                        open = ds.Open;
                        low = ds.Low < low ? ds.Low : low;
                        volume += ds.Volume;
                    }
                }
                s = new Stock();
                s.Date = date;
                s.High = high;
                s.Open = open;
                s.Close = close;
                s.Low = low;
                s.Volume = volume;
                _weekStocks.Add(s);
                //
                _weekStocks = _weekStocks.OrderBy(m => m.Date).ToList();
                _currentCycleStocks = _weekStocks;

                int day = 0;
                foreach (var ws in _weekStocks)
                {
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ws.Date.ToString("yyyyMMdd"), ws.Close);
                    }
                    else
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ws.Date.ToString("yyyyMMdd"), ws.Low);
                        _stockChart.Series["Price"].Points[day].YValues[1] = Convert.ToDouble(ws.High);
                        _stockChart.Series["Price"].Points[day].YValues[2] = Convert.ToDouble(ws.Close);
                        _stockChart.Series["Price"].Points[day].YValues[3] = Convert.ToDouble(ws.Open);
                    }
                    //_stockChart.Series["Price"].Points[day].ToolTip = "日期：" + ws.Date.ToShortDateString() + "\n开盘价：" + ws.Open.ToString("0.00") + "\n最高价：" + ws.High.ToString("0.00") + "\n最低价：" + ws.Low.ToString("0.00") + "\n收盘价：" + ws.Close.ToString("0.00") + "\n成交额：" + ws.Volume + "\n利润：" + (ws.Close - ws.Open).ToString("0.00");
                    _stockChart.Series["Volume"].Points.AddXY(ws.Date.ToString("yyyyMMdd"), ws.Volume);

                    if (ws.Open <= ws.Close)
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor1;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor1;
                        _stockChart.Series["Price"].Points[day].BackSecondaryColor = PriceBackSecondaryColor1;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor1;
                    }
                    else
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor2;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor2;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor2;
                    }
                }

                // 图表右边距
                for (int i = 0; i < 10; i++)
                {
                    DataPoint dp = new DataPoint();
                    dp.SetValueXY(" ", 0);
                    dp.IsEmpty = true;
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        dp.SetValueXY(" ", 0);
                    }
                    else
                    {
                        dp.SetValueXY(" ", 0, 0, 0, 0);
                    }
                    _stockChart.Series["Price"].Points.Add(dp);
                    _stockChart.Series["Volume"].Points.Add(dp);
                }
            }
        }

        private void RefreshMonthChart()
        {
            _monthStocks.Clear();
            _stockChart.Series["Price"].Points.Clear();
            _stockChart.Series["Volume"].Points.Clear();

            if (_stocks != null && _stocks.Count > 0)
            {
                var firstStock = _stocks.FirstOrDefault();
                DateTime date = firstStock.Date.Date;
                decimal high = firstStock.High;
                decimal open = firstStock.Open;
                decimal close = firstStock.Close;
                decimal low = firstStock.Low;
                decimal volume = 0;

                Stock s = null;
                int count = 0;
                foreach (var ds in _stocks)
                {
                    if (ds.Date.Month != date.Month)
                    {
                        if (++count >= _stockCount)
                            break;
                        s = new Stock();
                        s.Date = date;
                        s.High = high;
                        s.Open = open;
                        s.Close = close;
                        s.Low = low;
                        s.Volume = volume;
                        _monthStocks.Add(s);

                        date = ds.Date.Date;
                        high = ds.High;
                        low = ds.Low;
                        close = ds.Close;
                        open = ds.Open;
                        volume = ds.Volume;
                    }
                    else
                    {
                        date = ds.Date.Date;
                        high = ds.High > high ? ds.High : high;
                        open = ds.Open;
                        low = ds.Low < low ? ds.Low : low;
                        volume += ds.Volume;
                    }
                }
                s = new Stock();
                s.Date = date;
                s.High = high;
                s.Open = open;
                s.Close = close;
                s.Low = low;
                s.Volume = volume;
                _monthStocks.Add(s);
                //
                _monthStocks = _monthStocks.OrderBy(m => m.Date).ToList();
                _currentCycleStocks = _monthStocks;

                int day = 0;
                foreach (var ms in _monthStocks)
                {
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ms.Date.ToString("yyyyMM"), ms.Close);
                    }
                    else
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ms.Date.ToString("yyyyMM"), ms.Low);
                        _stockChart.Series["Price"].Points[day].YValues[1] = Convert.ToDouble(ms.High);
                        _stockChart.Series["Price"].Points[day].YValues[2] = Convert.ToDouble(ms.Close);
                        _stockChart.Series["Price"].Points[day].YValues[3] = Convert.ToDouble(ms.Open);
                    }
                    //_stockChart.Series["Price"].Points[day].ToolTip = "日期：" + ms.Date.ToShortDateString() + "\n开盘价：" + ms.Open.ToString("0.00") + "\n最高价：" + ms.High.ToString("0.00") + "\n最低价：" + ms.Low.ToString("0.00") + "\n收盘价：" + ms.Close.ToString("0.00") + "\n成交额：" + ms.Volume + "\n利润：" + (ms.Close - ms.Open).ToString("0.00");
                    _stockChart.Series["Volume"].Points.AddXY(ms.Date.ToString("yyyyMM"), ms.Volume);

                    if (ms.Open <= ms.Close)
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor1;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor1;
                        _stockChart.Series["Price"].Points[day].BackSecondaryColor = PriceBackSecondaryColor1;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor1;
                    }
                    else
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor2;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor2;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor2;
                    }
                }
                // 图表右边距
                for (int i = 0; i < 10; i++)
                {
                    DataPoint dp = new DataPoint();
                    dp.SetValueXY(" ", 0);
                    dp.IsEmpty = true;
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        dp.SetValueXY(" ", 0);
                    }
                    else
                    {
                        dp.SetValueXY(" ", 0, 0, 0, 0);
                    }
                    _stockChart.Series["Price"].Points.Add(dp);
                    _stockChart.Series["Volume"].Points.Add(dp);
                }
            }
        }

        private void RefreshYearChart()
        {
            _yearStocks.Clear();
            _stockChart.Series["Price"].Points.Clear();
            _stockChart.Series["Volume"].Points.Clear();

            if (_stocks != null && _stocks.Count > 0)
            {
                var firstStock = _stocks.FirstOrDefault();
                DateTime date = firstStock.Date.Date;
                decimal high = firstStock.High;
                decimal open = firstStock.Open;
                decimal close = firstStock.Close;
                decimal low = firstStock.Low;
                decimal volume = 0;

                Stock s = null;
                int count = 0;
                foreach (var ds in _stocks)
                {
                    if (ds.Date.Year != date.Year)
                    {
                        if (++count >= _stockCount)
                            break;
                        s = new Stock();
                        s.Date = date;
                        s.High = high;
                        s.Open = open;
                        s.Close = close;
                        s.Low = low;
                        s.Volume = volume;
                        _yearStocks.Add(s);

                        date = ds.Date.Date;
                        high = ds.High;
                        low = ds.Low;
                        close = ds.Close;
                        open = ds.Open;
                        volume = ds.Volume;
                    }
                    else
                    {
                        date = ds.Date.Date;
                        high = ds.High > high ? ds.High : high;
                        open = ds.Open;
                        low = ds.Low < low ? ds.Low : low;
                        volume += ds.Volume;
                    }
                }
                s = new Stock();
                s.Date = date;
                s.High = high;
                s.Open = open;
                s.Close = close;
                s.Low = low;
                s.Volume = volume;
                _yearStocks.Add(s);
                //
                _yearStocks = _yearStocks.OrderBy(m => m.Date).ToList();
                _currentCycleStocks = _yearStocks;


                int day = 0;
                foreach (var ys in _yearStocks)
                {
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ys.Date.ToString("yyyy"), ys.Close);
                    }
                    else
                    {
                        day = _stockChart.Series["Price"].Points.AddXY(ys.Date.ToString("yyyy"), ys.Low);
                        _stockChart.Series["Price"].Points[day].YValues[1] = Convert.ToDouble(ys.High);
                        _stockChart.Series["Price"].Points[day].YValues[2] = Convert.ToDouble(ys.Close);
                        _stockChart.Series["Price"].Points[day].YValues[3] = Convert.ToDouble(ys.Open);
                    }
                    //_stockChart.Series["Price"].Points[day].ToolTip = "日期：" + ys.Date.ToShortDateString() + "\n开盘价：" + ys.Open.ToString("0.00") + "\n最高价：" + ys.High.ToString("0.00") + "\n最低价：" + ys.Low.ToString("0.00") + "\n收盘价：" + ys.Close.ToString("0.00") + "\n成交额：" + ys.Volume + "\n利润：" + (ys.Close - ys.Open).ToString("0.00");
                    _stockChart.Series["Volume"].Points.AddXY(ys.Date.ToString("yyyy"), ys.Volume);

                    if (ys.Open <= ys.Close)
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor1;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor1;
                        _stockChart.Series["Price"].Points[day].BackSecondaryColor = PriceBackSecondaryColor1;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor1;
                    }
                    else
                    {
                        _stockChart.Series["Volume"].Points[day].Color = VolumeColor2;
                        _stockChart.Series["Price"].Points[day].BorderColor = PriceBorderColor2;
                        _stockChart.Series["Price"].Points[day].Color = PriceColor2;
                    }
                }
                // 图表右边距
                for (int i = 0; i < 10; i++)
                {
                    DataPoint dp = new DataPoint();
                    dp.SetValueXY(" ", 0);
                    dp.IsEmpty = true;
                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        dp.SetValueXY(" ", 0);
                    }
                    else
                    {
                        dp.SetValueXY(" ", 0, 0, 0, 0);
                    }
                    _stockChart.Series["Price"].Points.Add(dp);
                    _stockChart.Series["Volume"].Points.Add(dp);
                }
            }
        }

        private void RefreshAverageTitle(double cursorX, Stock stock)
        {
            int index = (int)cursorX - 1;

            bool indexValid = true;
            if (_stockChart.Series["Average1"].Enabled && index >= _stockChart.Series["Average1"].Points.Count)
            {
                indexValid = false;
            }
            if (_stockChart.Series["Average2"].Enabled && index >= _stockChart.Series["Average2"].Points.Count)
            {
                indexValid = false;
            }
            if (_stockChart.Series["Average3"].Enabled && index >= _stockChart.Series["Average3"].Points.Count)
            {
                indexValid = false;
            }
            if (index < 0)
            {
                indexValid = false;
            }
            if (_showCursorXY && indexValid)
            {
                double cunAverage1 = _stockChart.Series["Average1"].Points.Count > 0 ? _stockChart.Series["Average1"].Points[(int)cursorX - 1].YValues[0] : 0;
                double cunAverage2 = _stockChart.Series["Average2"].Points.Count > 0 ? _stockChart.Series["Average2"].Points[(int)cursorX - 1].YValues[0] : 0;
                double cunAverage3 = _stockChart.Series["Average3"].Points.Count > 0 ? _stockChart.Series["Average3"].Points[(int)cursorX - 1].YValues[0] : 0;
                string titleText =
                    string.Format(
                    "均线（{0}，{2}，{4}）（{1}，{3}，{5}）",
                    _average1Value,
                    cunAverage1.ToString("0"),
                    _average2Value,
                    cunAverage2.ToString("0"),
                    _average3Value,
                    cunAverage3.ToString("0")
                    );

                //
                var ganZhi = new GanZhi(stock.Date, SolarTermDictionary);
                switch (_cycle)
                {
                    case ChartCycle.日线:
                        titleText += string.Format("{0}{1}年 {2}{3}月 {4}{5}日", ganZhi.TianGanYear, ganZhi.DiZhiYear, ganZhi.TianGanMonth, ganZhi.DiZhiMonth, ganZhi.TianGanDay, ganZhi.DiZhiDay);
                        break;
                    case ChartCycle.周线:
                        titleText += string.Format("{0}{1}年 {2}{3}月", ganZhi.TianGanYear, ganZhi.DiZhiYear, ganZhi.TianGanMonth, ganZhi.DiZhiMonth);
                        break;
                    case ChartCycle.月线:
                        titleText += string.Format("{0}{1}年 {2}{3}月", ganZhi.TianGanYear, ganZhi.DiZhiYear, ganZhi.TianGanMonth, ganZhi.DiZhiMonth);
                        break;
                    case ChartCycle.年线:
                        titleText += string.Format("{0}{1}年", ganZhi.TianGanYear, ganZhi.DiZhiYear);
                        break;
                    default: break;
                }

                    _stockChart.Titles["Average"].Text = titleText;
            }
        }

        private void RefreshInfoBoard(Stock stock)
        {
            if (stock != null)
            {
                _label日期.Text = stock.Date.ToString("yyyy/MM/dd");
                _label开盘.Text = stock.Open.ToString("N");
                _label收盘.Text = stock.Close.ToString("N");
                _label最高.Text = stock.High.ToString("N");
                _label最低.Text = stock.Low.ToString("N");
                _label量.Text = stock.Volume.ToString("N");
                _label利润.Text = (stock.Close - stock.Open).ToString("N");
                _label涨幅.Text = stock.Open <= 0 ? "" : ((stock.Close - stock.Open) / stock.Open * 100).ToString("0.00") + "%"; ;
                if (stock.Close > stock.Open)
                {
                    _label收盘.ForeColor = System.Drawing.Color.Red;
                    _label利润.ForeColor = System.Drawing.Color.Red;
                    _label涨幅.ForeColor = System.Drawing.Color.Red;
                }
                else if (stock.Close < stock.Open)
                {
                    _label收盘.ForeColor = System.Drawing.Color.Cyan;
                    _label利润.ForeColor = System.Drawing.Color.Cyan;
                    _label涨幅.ForeColor = System.Drawing.Color.Cyan;
                }
                else
                {
                    _label收盘.ForeColor = System.Drawing.Color.White;
                    _label利润.ForeColor = System.Drawing.Color.White;
                    _label涨幅.ForeColor = System.Drawing.Color.White;
                }
                if (stock.High > stock.Open)
                {
                    _label最高.ForeColor = System.Drawing.Color.Red;
                }
                else
                {
                    _label最高.ForeColor = System.Drawing.Color.White;
                }

                if (stock.Low < stock.Open)
                {
                    _label最低.ForeColor = System.Drawing.Color.Cyan;
                }
                else
                {
                    _label最低.ForeColor = System.Drawing.Color.White;
                }

            }
            _label当前值.Text = _stockChart.ChartAreas["Price"].CursorY.Position.ToString("N");
        }

        private void LatestAverageTitle()
        {
            if (!_showCursorXY)
            {
                double cunAverage1 = 0;
                double cunAverage2 = 0;
                double cunAverage3 = 0;
                cunAverage1 = _stockChart.Series["Average1"].Points.Count > 0 ? _stockChart.Series["Average1"].Points[_stockChart.Series["Average1"].Points.Count - 1].YValues[0] : 0;
                cunAverage2 = _stockChart.Series["Average2"].Points.Count > 0 ? _stockChart.Series["Average2"].Points[_stockChart.Series["Average2"].Points.Count - 1].YValues[0] : 0;
                cunAverage3 = _stockChart.Series["Average3"].Points.Count > 0 ? _stockChart.Series["Average3"].Points[_stockChart.Series["Average3"].Points.Count - 1].YValues[0] : 0;
                _stockChart.Titles["Average"].Text = string.Format("均线（{0}，{2}，{4}）（{1}，{3}，{5}）",
                    _average1Value, cunAverage1.ToString("0"),
                    _average2Value, cunAverage2.ToString("0"),
                    _average3Value, cunAverage3.ToString("0"));
            }
        }

        private void _chart_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_showCursorXY || _showStatisticsCursorXY)
            {
                Stock stock = null;
                var cursorX = _stockChart.ChartAreas["Price"].AxisX.PixelPositionToValue(e.X)._ToStockPosition();
                var cursorY = _stockChart.ChartAreas["Price"].AxisY.PixelPositionToValue(e.Y)._ToStockPosition();
                _stockChart.ChartAreas["Price"].CursorX.Position = cursorX;
                _stockChart.ChartAreas["Price"].CursorY.Position = cursorY;
                if (cursorX > 0 && cursorX <= _currentCycleStocks.Count)
                    stock = _currentCycleStocks[(int)cursorX - 1];
                //
                RefreshInfoBoard(stock);
                RefreshAverageTitle(cursorX, stock);
            }
        }

        private void _chart_MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_showCursorXY && !_statisticsBetween)
            {
                ShowCursorXY(false);
                LatestAverageTitle();
            }
            else
            {
                ShowCursorXY(true);
            }
        }

        private void _stockChart_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            switch (e.Delta)
            {
                case 120:
                    {
                        if (!_showCursorXY)
                        {
                            switch (_cycle)
                            {
                                case ChartCycle.日线:
                                    break;
                                case ChartCycle.周线:
                                    _cycle = ChartCycle.日线;
                                    break;
                                case ChartCycle.月线:
                                    _cycle = ChartCycle.周线;
                                    break;
                                case ChartCycle.年线:
                                    _cycle = ChartCycle.月线;
                                    break;
                                default: break;
                            }
                            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
                            RefreshChart();
                        }
                        break;
                    }
                case -120:
                    {
                        if (!_showCursorXY)
                        {
                            switch (_cycle)
                            {
                                case ChartCycle.日线:
                                    _cycle = ChartCycle.周线;
                                    break;
                                case ChartCycle.周线:
                                    _cycle = ChartCycle.月线;
                                    break;
                                case ChartCycle.月线:
                                    _cycle = ChartCycle.年线;
                                    break;
                                case ChartCycle.年线:
                                    break;
                                default: break;
                            }
                            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
                            RefreshChart();
                        }
                        break;
                    }
            }
        }

        private void ShowCursorXY(bool status)
        {
            if (status)
            {
                _stockChart.ChartAreas["Price"].CursorX.LineDashStyle = ChartDashStyle.Solid;
                _stockChart.ChartAreas["Price"].CursorY.LineDashStyle = ChartDashStyle.Solid;
                _stockChart.ChartAreas["Volume"].CursorX.LineDashStyle = ChartDashStyle.Solid;
            }
            else
            {
                _stockChart.ChartAreas["Price"].CursorX.LineDashStyle = ChartDashStyle.NotSet;
                _stockChart.ChartAreas["Price"].CursorY.LineDashStyle = ChartDashStyle.NotSet;
                _stockChart.ChartAreas["Volume"].CursorX.LineDashStyle = ChartDashStyle.NotSet;
            }
            _showCursorXY = status;
            _panel信息板.Visible = status;
        }

        private void ShowStatisticsCursorXY(bool status)
        {
            if (status)
            {
                _stockChart.ChartAreas["Price"].CursorX.LineDashStyle = ChartDashStyle.Dash;
                _stockChart.ChartAreas["Price"].CursorY.LineDashStyle = ChartDashStyle.NotSet;
                _stockChart.ChartAreas["Volume"].CursorX.LineDashStyle = ChartDashStyle.Dash;
                _showStatisticsCursorXY = true;
                _statisticsBetween = true;
                _panel信息板.Visible = true;
            }
            else if (_showCursorXY)
            {
                _stockChart.ChartAreas["Price"].CursorX.LineDashStyle = ChartDashStyle.Solid;
                _stockChart.ChartAreas["Price"].CursorY.LineDashStyle = ChartDashStyle.Solid;
                _stockChart.ChartAreas["Volume"].CursorX.LineDashStyle = ChartDashStyle.Solid;
                _statisticsBetween = false;
                _showStatisticsCursorXY = false;
                _panel信息板.Visible = true;
            }
            else
            {
                _stockChart.ChartAreas["Price"].CursorX.LineDashStyle = ChartDashStyle.NotSet;
                _stockChart.ChartAreas["Price"].CursorY.LineDashStyle = ChartDashStyle.NotSet;
                _stockChart.ChartAreas["Volume"].CursorX.LineDashStyle = ChartDashStyle.NotSet;
                _statisticsBetween = false;
                _showStatisticsCursorXY = false;
                _panel信息板.Visible = false;
            }
        }

        private Stock GetStockByCurrentCursorX()
        {
            int index = (int)_stockChart.ChartAreas["Price"].CursorX.Position - 1;
            if (index < 0 || index >= _currentCycleStocks.Count)
                return null;
            else
                return _currentCycleStocks[index];
        }

        private Stock GetStockByCursorX(double cursorX)
        {
            int index = (int)cursorX - 1;
            if (index < 0 || index >= _currentCycleStocks.Count)
                return null;
            else
                return _currentCycleStocks[index];
        }

        private void _panel信息板_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _chartInfoBoardMove = true;
            _panel信息板.Cursor = System.Windows.Forms.Cursors.SizeAll;
        }

        private void _panel信息板_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            _chartInfoBoardMove = false;
            _panel信息板.Cursor = System.Windows.Forms.Cursors.Default;
        }

        private void _panel信息板_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_chartInfoBoardMove)
            {
                _panel信息板.Top += System.Windows.Forms.Control.MousePosition.Y - _mouseY;
                _panel信息板.Left += System.Windows.Forms.Control.MousePosition.X - _mouseX;
            }
            _mouseX = System.Windows.Forms.Control.MousePosition.X;
            _mouseY = System.Windows.Forms.Control.MousePosition.Y;
        }

        private void _toolStripLabel日线_Click(object sender, EventArgs e)
        {
            //
            _cycle = ChartCycle.日线;
            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
            //
            _ToolStripMenuItem日线.Enabled = false;
            _btn日.IsEnabled = false;
            _ToolStripMenuItem周线.Enabled = true;
            _btn周.IsEnabled = true;
            _ToolStripMenuItem月线.Enabled = true;
            _btn月.IsEnabled = true;
            _ToolStripMenuItem年线.Enabled = true;
            _btn年.IsEnabled = true;
            //
            RefreshChart();

        }

        private void _toolStripLabel周线_Click(object sender, EventArgs e)
        {
            //
            _cycle = ChartCycle.周线;
            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
            //
            _ToolStripMenuItem日线.Enabled = true;
            _btn日.IsEnabled = true;
            _ToolStripMenuItem周线.Enabled = false;
            _btn周.IsEnabled = false;
            _ToolStripMenuItem月线.Enabled = true;
            _btn月.IsEnabled = true;
            _ToolStripMenuItem年线.Enabled = true;
            _btn年.IsEnabled = true;
            //
            RefreshChart();
        }

        private void _toolStripLabel月线_Click(object sender, EventArgs e)
        {
            //
            _cycle = ChartCycle.月线;
            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
            //
            _ToolStripMenuItem日线.Enabled = true;
            _btn日.IsEnabled = true;
            _ToolStripMenuItem周线.Enabled = true;
            _btn周.IsEnabled = true;
            _ToolStripMenuItem月线.Enabled = false;
            _btn月.IsEnabled = false;
            _ToolStripMenuItem年线.Enabled = true;
            _btn年.IsEnabled = true;
            //
            RefreshChart();
        }

        private void _toolStripLabel年线_Click(object sender, EventArgs e)
        {
            //
            _cycle = ChartCycle.年线;
            _Helper.SetParameter(ParameterName.StockChart周期.ToString(), _cycle.ToString());
            //
            _ToolStripMenuItem日线.Enabled = true;
            _btn日.IsEnabled = true;
            _ToolStripMenuItem周线.Enabled = true;
            _btn周.IsEnabled = true;
            _ToolStripMenuItem月线.Enabled = true;
            _btn月.IsEnabled = true;
            _ToolStripMenuItem年线.Enabled = false;
            _btn年.IsEnabled = false;
            //
            RefreshChart();
        }

        private void _ToolStripMenuItem蜡烛线_Click(object sender, EventArgs e)
        {
            _stockChart.Series["Price"].ChartType = SeriesChartType.Candlestick;
            _ToolStripMenuItem蜡烛线.Enabled = false;
            _btnK.IsEnabled = false;
            _ToolStripMenuItem收盘线.Enabled = true;
            _btnC.IsEnabled = true;

            //
            _Helper.SetParameter(ParameterName.StockChart风格.ToString(), _stockChart.Series["Price"].ChartType.ToString());

            RefreshChart();
        }

        private void _ToolStripMenuItem收盘线_Click(object sender, EventArgs e)
        {
            _stockChart.Series["Price"].ChartType = SeriesChartType.Line;
            _ToolStripMenuItem蜡烛线.Enabled = true;
            _btnK.IsEnabled = true;
            _ToolStripMenuItem收盘线.Enabled = false;
            _btnC.IsEnabled = false;

            //
            _Helper.SetParameter(ParameterName.StockChart风格.ToString(), _stockChart.Series["Price"].ChartType.ToString());

            RefreshChart();
        }

        private void _ToolStripMenuItem保存到_Click(object sender, EventArgs e)
        {
            try
            {
                using (StatementContext statement = new StatementContext(typeof(Account)))
                {
                    var acc = statement.Accounts.FirstOrDefault(a => a.Id == _Session.SelectedAccountId);
                    if (acc == null)
                    {
                        System.Windows.MessageBox.Show(string.Format("无法保存图片，系统中不存在ID<{0}>为资金账户，数据库有错误，请联系管理员！", _Session.SelectedAccountId));
                        return;
                    }
                    _saveFileDialog保存到图片文件.Filter = "图片(*.png)|*.png|图片(*.bmp)|*.bmp|图片(*.Jpeg)|*.Jpeg|图片(*.Gif)|*.Gif|图片(*.Tif)|*.Tif|所有文件(*.*)|*.*";
                    _saveFileDialog保存到图片文件.FilterIndex = 1;
                    _saveFileDialog保存到图片文件.FileName = string.Format("{0}({1}).png", acc.CustomerName, acc.AccountNumber);
                    if (_saveFileDialog保存到图片文件.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = _saveFileDialog保存到图片文件.FileName;
                        Bitmap bt = new Bitmap(_stockChart.Width, _stockChart.Height);
                        _stockChart.DrawToBitmap(bt, new System.Drawing.Rectangle(0, 0, _stockChart.Width, _stockChart.Height));
                        switch (_saveFileDialog保存到图片文件.FilterIndex)
                        {
                            case 1:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Png); break;
                            case 2:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                            case 3:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg); break;
                            case 4:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Gif); break;
                            case 5:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Tiff); break;
                            default:
                                bt.Save(filePath, System.Drawing.Imaging.ImageFormat.Bmp); break;
                        }
                        bt.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

#if false
        private void _ToolStripMenuItem图表起始时间_Click(object sender, EventArgs e)
        {
            SetStockStartDateDialog gdf = new SetStockStartDateDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(gdf)
                {
                    Owner = winformWindow.Handle
                };
            }
            gdf.ShowDialog();
            if (gdf.DialogResult.HasValue && gdf.DialogResult.Value)
            {
                _Helper.SetParameter(ParameterName.StockChart图表开始日期.ToString(), gdf.Value.ToShortDateString());
                this._chartStartDate = gdf.Value;
                this.RefreshChart();
            }
        } 
#endif

        private void _toolStripMenuItem均线_Click(object sender, EventArgs e)
        {
            ShowAverageLine();
        }

        private void _toolStripMenuItem均线参数_Click(object sender, EventArgs e)
        {
            AverageParameterWindow asf = new AverageParameterWindow();
            asf.Average1Value = _average1Value;
            asf.Average2Value = _average2Value;
            asf.Average3Value = _average3Value;
            asf.Average1Color = _average1Color;
            asf.Average2Color = _average2Color;
            asf.Average3Color = _average3Color;
            asf.Average1Visibility = _average1Visibility;
            asf.Average2Visibility = _average2Visibility;
            asf.Average3Visibility = _average3Visibility;
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(asf)
                {
                    Owner = winformWindow.Handle
                };
            }
            asf.ShowDialog();
            if (asf.DialogResult.HasValue && asf.DialogResult == true)
            {
                _average1Value = asf.Average1Value;
                _average2Value = asf.Average2Value;
                _average3Value = asf.Average3Value;
                _average1Color = asf.Average1Color;
                _average2Color = asf.Average2Color;
                _average3Color = asf.Average3Color;
                _average1Visibility = asf.Average1Visibility;
                _average2Visibility = asf.Average2Visibility;
                _average3Visibility = asf.Average3Visibility;
                _Helper.SetParameter(ParameterName.Average1Value.ToString(), _average1Value.ToString());
                _Helper.SetParameter(ParameterName.Average2Value.ToString(), _average2Value.ToString());
                _Helper.SetParameter(ParameterName.Average3Value.ToString(), _average3Value.ToString());
                _Helper.SetParameter(ParameterName.Average1Color.ToString(), _average1Color.ToArgb().ToString());
                _Helper.SetParameter(ParameterName.Average2Color.ToString(), _average2Color.ToArgb().ToString());
                _Helper.SetParameter(ParameterName.Average3Color.ToString(), _average3Color.ToArgb().ToString());
                _Helper.SetParameter(ParameterName.Average1Visibility.ToString(), _average1Visibility.ToString());
                _Helper.SetParameter(ParameterName.Average2Visibility.ToString(), _average2Visibility.ToString());
                _Helper.SetParameter(ParameterName.Average3Visibility.ToString(), _average3Visibility.ToString());
                //this._stockChart.Titles["Average"].Text = string.Format("均线（{0}，{1}，{2}）", _average1Value, _average2Value, _average3Value);
                RefreshChart();
                LatestAverageTitle();
            }
        }

        private void _stockChart_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_statisticsBetween && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                _beginStock = GetStockByCurrentCursorX();
                if (_beginStock == null)
                {
                    System.Windows.MessageBox.Show("开始点不是有效位置！");
                }
            }
        }

        private void _stockChart_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (_statisticsBetween && e.Button == System.Windows.Forms.MouseButtons.Left && _beginStock != null)
            {
                _stopStock = GetStockByCurrentCursorX();
                if (_stopStock == null)
                {
                    System.Windows.MessageBox.Show("结束点不是有效位置！");
                    return;
                }

                if (_beginStock.Date > _stopStock.Date)
                {
                    //
                    ShowStatisticsCursorXY(false);
                    return;
                }
                else
                {
                    var stocks = _currentCycleStocks.Where(s => s.Date >= _beginStock.Date && s.Date <= _stopStock.Date).OrderBy(s => s.Date);
                    DateTime beginDate, stopDate;//起始时间、结束时间
                    decimal open, high, low, close, volume;//期初价、最高价、最低价、期末价、成交额、涨幅、振幅
                    int count = stocks.Count();
                    beginDate = _beginStock.Date;
                    stopDate = _stopStock.Date;

                    if (_stockChart.Series["Price"].ChartType == SeriesChartType.Line)
                    {
                        open = _beginStock.Close;
                    }
                    else
                    {
                        open = _beginStock.Open;
                    }
                    high = _beginStock.High;
                    low = _beginStock.Low;
                    close = _stopStock.Close;
                    volume = 0;
                    foreach (var st in stocks)
                    {
                        high = st.High > high ? st.High : high;
                        low = st.Low < low ? st.Low : low;
                        volume += st.Volume;
                    }
                    var span = stopDate - beginDate;
                    int year = (int)span.TotalDays / 365;
                    int month = (int)span.TotalDays % 365 / 30;
                    int day = (int)span.TotalDays % 30;
                    //
                    ShowStatisticsCursorXY(false);
                    //
                    StatisticsBetweenDialog sbf = new StatisticsBetweenDialog(beginDate, stopDate, open, high, low, close, volume, string.Concat(year > 0 ? year + "年" : "", month > 0 ? month + "个月" : "", day, "天"));
                    HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
                    if (winformWindow != null)
                    {
                        new WindowInteropHelper(sbf)
                        {
                            Owner = winformWindow.Handle
                        };
                    }
                    sbf.ShowDialog();
                }
            }
        }

        private void _toolStripMenuItem区间统计_Click(object sender, EventArgs e)
        {
            ShowStatisticsCursorXY(true);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.LeftShift:
                case Key.RightShift:
                    {
                        _toolStripMenuItem区间统计_Click(sender, e); break;
                    }
                case Key.D:
                    {
                        _toolStripLabel日线_Click(sender, e); break;
                    }
                case Key.W:
                    {
                        _toolStripLabel周线_Click(sender, e); break;
                    }
                case Key.M:
                    {
                        _toolStripLabel月线_Click(sender, e); break;
                    }
                case Key.Y:
                    {
                        _toolStripLabel年线_Click(sender, e); break;
                    }
                case Key.K:
                    {
                        _ToolStripMenuItem蜡烛线_Click(sender, e); break;
                    }
                case Key.C:
                    {
                        _ToolStripMenuItem收盘线_Click(sender, e); break;
                    }
                case Key.Space:
                    {
                        ShowAverageLine();
                        break;
                    }
                case Key.PageUp:
                    {
                        switch (_cycle)
                        {
                            case ChartCycle.日线: break;
                            case ChartCycle.周线: _cycle = ChartCycle.日线; break;
                            case ChartCycle.月线: _cycle = ChartCycle.周线; break;
                            case ChartCycle.年线: _cycle = ChartCycle.月线; break;
                            default: break;
                        }
                        RefreshChart(); break;
                    }
                case Key.PageDown:
                    {
                        switch (_cycle)
                        {
                            case ChartCycle.日线: _cycle = ChartCycle.周线; break;
                            case ChartCycle.周线: _cycle = ChartCycle.月线; break;
                            case ChartCycle.月线: _cycle = ChartCycle.年线; break;
                            case ChartCycle.年线: break;
                            default: break;
                        }
                        RefreshChart(); break;
                    }
                case Key.Left:
                    {
                        if (_showCursorXY)
                        {
                            var oldCursorX = _stockChart.ChartAreas["Price"].CursorX.Position;
                            Stock stock = GetStockByCurrentCursorX();
                            //左
                            if (oldCursorX - 1 > 0)
                            {
                                _stockChart.ChartAreas["Price"].CursorX.Position = (oldCursorX - 1)._ToStockPosition();
                                stock = GetStockByCurrentCursorX();
                            }
                            //
                            if (stock != null)
                            {
                                _stockChart.ChartAreas["Price"].CursorY.Position = (double)stock.Close;
                                RefreshInfoBoard(stock);
                                RefreshAverageTitle(_stockChart.ChartAreas["Price"].CursorX.Position, stock);
                            }
                        }
                        break;
                    }
                case Key.Right:
                    {
                        if (_showCursorXY)
                        {
                            var oldCursorX = _stockChart.ChartAreas["Price"].CursorX.Position;
                            Stock stock = GetStockByCurrentCursorX();
                            //右
                            if (oldCursorX + 1 <= _currentCycleStocks.Count)
                            {
                                _stockChart.ChartAreas["Price"].CursorX.Position = (oldCursorX + 1)._ToStockPosition();
                                stock = GetStockByCurrentCursorX();
                            }
                            //
                            if (stock != null)
                            {
                                _stockChart.ChartAreas["Price"].CursorY.Position = (double)stock.Close;
                                RefreshInfoBoard(stock);
                                RefreshAverageTitle(_stockChart.ChartAreas["Price"].CursorX.Position, stock);
                            }
                        }
                        break;
                    }

                case Key.Up: // 更少图表
                    {
                        if (_stocks.Count >= 0)
                        {
                            if (_stockCount > _currentCycleStocks.Count)
                            {
                                _stockCount = _currentCycleStocks.Count;
                            }
                            if (_stockCount > 2)
                            {
                                _stockCount = _stockCount * 3 / 4;
                                RefreshChart();
                                _Helper.SetParameter(ParameterName.StockCount.ToString(), _stockCount.ToString());
                            }
                        }
                        break;
                    }
                case Key.Down: // 更多图表
                    {
                        if (_stocks.Count != 0)
                        {
                            //
                            DateTime dt1 = _currentCycleStocks.FirstOrDefault().Date;
                            DateTime dt2 = _stocks.LastOrDefault().Date;
                            if (_currentCycleStocks.FirstOrDefault().Date > _stocks.LastOrDefault().Date)
                            {
                                _stockCount = _stockCount * 5 / 4;
                                RefreshChart();
                                _Helper.SetParameter(ParameterName.StockCount.ToString(), _stockCount.ToString());

                            }
                        }
                        break;
                    }
            }

        }

        /// <summary>
        /// 根据this._toolStripMenuItem均线.Checked属性值自动判断显示或隐藏均线
        /// </summary>
        private void ShowAverageLine()
        {
            _toolStripMenuItem均线.Checked = !_toolStripMenuItem均线.Checked;
            _Helper.SetParameter(ParameterName.StockChart显示均线.ToString(), _toolStripMenuItem均线.Checked.ToString());
            //
            _stockChart.Series["Average1"].Enabled = _toolStripMenuItem均线.Checked;
            _stockChart.Series["Average2"].Enabled = _toolStripMenuItem均线.Checked;
            _stockChart.Series["Average3"].Enabled = _toolStripMenuItem均线.Checked;
            _stockChart.Titles["Average"].Visible = _toolStripMenuItem均线.Checked;
        }

        private IDictionary<DateTime, SolarTerm> SolarTermDictionary = new Dictionary<DateTime, SolarTerm>();

        private string solarFilePath = System.Windows.Forms.Application.StartupPath + "\\statement\\solar.dat";
        private void LoadSolarTerm()
        {
            SolarTermDictionary = SolarTremStream.Load(solarFilePath);
        }


        private void _btn日_Click(object sender, RoutedEventArgs e)
        {
            _toolStripLabel日线_Click(sender, e);
        }

        private void _btn周_Click(object sender, RoutedEventArgs e)
        {
            _toolStripLabel周线_Click(sender, e);
        }

        private void _btn月_Click(object sender, RoutedEventArgs e)
        {
            _toolStripLabel月线_Click(sender, e);
        }
        private void _btn年_Click(object sender, RoutedEventArgs e)
        {
            _toolStripLabel年线_Click(sender, e);
        }

        private void _btnK_Click(object sender, RoutedEventArgs e)
        {
            _ToolStripMenuItem蜡烛线_Click(sender, e);
        }

        private void _btnC_Click(object sender, RoutedEventArgs e)
        {
            _ToolStripMenuItem收盘线_Click(sender, e);
        }

        private void _btn均_Click(object sender, RoutedEventArgs e)
        {
            _toolStripMenuItem均线_Click(sender, e);
        }

        private void _btn合_Click(object sender, RoutedEventArgs e)
        {
            _toolStripLabel账户合并显示_Click(sender, e);
        }


#if false
        private void _chart_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (this._showCursorXY && e.KeyValue == 37 || e.KeyValue == 39)
            {
                var oldCursorX = this._stockChart.ChartAreas["Price"].CursorX.Position;
                Stock stock = this.GetStockByCurrentCursorX();
                //左
                if (e.KeyValue == 37 && oldCursorX - 1 > 0)
                {
                    this._stockChart.ChartAreas["Price"].CursorX.Position = (oldCursorX - 1).ToStockPosition();
                    stock = this.GetStockByCurrentCursorX();
                }
                //右
                if (e.KeyValue == 39 && oldCursorX + 1 <= _currentStocks.Count)
                {
                    this._stockChart.ChartAreas["Price"].CursorX.Position = (oldCursorX + 1).ToStockPosition();
                    stock = this.GetStockByCurrentCursorX();
                }
                //
                this._stockChart.ChartAreas["Price"].CursorY.Position = (double)stock.Close;
                this.RefreshInfoBoard(stock);
                this.RefreshAverageTitle(this._stockChart.ChartAreas["Price"].CursorX.Position);
            }
            //if ((e.KeyValue == 38 || e.KeyValue == 40) && this._dayStocks.Count != 0)
            //{
            //    // 上
            //    int newNum = this._dayStocks.Count / 10 == 0 ? 1 : this._dayStocks.Count / 10;
            //    if (e.KeyValue == 38 && new StatementContext().Stocks.Where(s => s.AccountId == FASession.AccountId && s.Date < this._startDate.Date).Count() > 0)
            //    {
            //        this._startDate = this._startDate.AddDays(newNum * -1);
            //    }
            //    // 下
            //    if (e.KeyValue == 40 && this._currentStocks.Count > 2)
            //    {
            //        this._startDate = this._startDate.AddDays(newNum);
            //    }
            //    this.RefreshChart();
            //}
        }
#endif
    }
}
