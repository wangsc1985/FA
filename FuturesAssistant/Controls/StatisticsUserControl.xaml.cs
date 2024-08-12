using FuturesAssistant.Controls.Types;
using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.DataVisualization.Charting;
using System.Windows.Input;
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
    public partial class StatisticsUserControl : UserControl
    {
        private delegate void FormControlInvoker();

        // 平仓单统计
        #region
        //ChartArea chartArea平仓单统计 = new ChartArea();
        //Series series平仓单亏损 = new Series();
        //Series series平仓单盈利 = new Series();
        #endregion

        // 盈亏比例
        #region
        ChartArea chartArea1 = new ChartArea();
        Legend legend1 = new Legend();
        Series series盈亏比例 = new Series();
        DataPoint dataPoint盈比 = new DataPoint();
        DataPoint dataPoint亏比 = new DataPoint();
        DataPoint dataPoint手续费比 = new DataPoint();
        Title title1 = new Title();
        #endregion

        // 多空比例
        #region
        ChartArea chartArea2 = new ChartArea();
        Legend legend2 = new Legend();
        Series series多空比例 = new Series();
        DataPoint dataPoint空方比例 = new DataPoint();
        DataPoint dataPoint多方比例 = new DataPoint();
        Title title2 = new Title();
        #endregion

        // 多空盈亏
        #region
        ChartArea chartArea3 = new ChartArea();
        ChartArea chartArea4 = new ChartArea();
        ChartArea chartArea5 = new ChartArea();
        ChartArea chartArea6 = new ChartArea();
        Legend legend3 = new Legend();
        Legend legend4 = new Legend();
        Legend legend5 = new Legend();
        Legend legend6 = new Legend();
        Series series多空盈亏1 = new Series();
        Series series多空盈亏2 = new Series();
        Series series多空盈亏3 = new Series();
        Series series多空盈亏4 = new Series();
        DataPoint dataPoint7 = new DataPoint();
        DataPoint dataPoint8 = new DataPoint();
        DataPoint dataPoint9 = new DataPoint();
        DataPoint dataPoint10 = new DataPoint();
        DataPoint dataPoint6 = new DataPoint();
        DataPoint dataPoint11 = new DataPoint();
        DataPoint dataPoint12 = new DataPoint();
        DataPoint dataPoint13 = new DataPoint();
        #endregion

        // 持仓偏好
        #region
        ChartArea chartArea7 = new ChartArea();
        Series series持仓偏好 = new Series();
        Title title7 = new Title();
        #endregion

        // 成交偏好
        #region
        ChartArea chartArea8 = new ChartArea();
        Series series成交偏好 = new Series();
        Title title8 = new Title();
        #endregion

        // 每日持仓
        #region
        ChartArea chartArea11 = new ChartArea();
        Series series每日持仓 = new Series();
        Title title11 = new Title();
        #endregion

        // 品种盈亏
        #region
        ChartArea chartArea12 = new ChartArea();
        Series series品种盈亏 = new Series();
        Title title12 = new Title();
        #endregion

        // 每周盈亏
        #region
        ChartArea chartArea9 = new ChartArea();
        Series series每周盈亏 = new Series();
        Title title9 = new Title();
        #endregion

        // 每月盈亏
        #region
        ChartArea chartArea10 = new ChartArea();
        Series series每月盈亏 = new Series();
        Title title10 = new Title();
        #endregion

        public StatisticsUserControl()
        {
            InitializeComponent();
            InitializeCustomComponent();
        }

        public void Initialize(bool sync = true)
        {
            try
            {
                //this.Dispatcher.Invoke(new FormControlInvoker(() =>
                //{
                //this.UpdateUIEvents();

                ////平面立体按钮
                //var style = FAHelper.GetParameter(ParameterName.Statistics风格.ToString(), this._button平面图.Content.ToString());
                //if (style.Equals(this._button平面图.Content.ToString()))
                //{
                //    this._button平面图.IsEnabled = false;
                //}
                //else
                //{
                //    this._button立体图.IsEnabled = false;
                //    To3D();
                //}
                //}));
                //
                //this._buttonMonthStatistics_Click(null, null);
                if (sync)
                {
                    new Thread(new ThreadStart(() =>
                    {
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, _Session.LatestFundStatusDate.Month, 1);
                            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
                        }));
                        RefreshStatistics();
                    })).Start();
                }
                else
                {
                    _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, _Session.LatestFundStatusDate.Month, 1);
                    _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
                    RefreshStatistics();
                }
                //this._buttonYearStatistics_Click(null, null);
                var fds = new StatementContext().FundStatus.OrderBy(fs => fs.Date).ToList().FirstOrDefault();
                if (fds != null)
                {
                    _dateTimePicker开始.DisplayDateStart = _dateTimePicker结束.DisplayDateStart = fds.Date;
                }
                _dateTimePicker结束.DisplayDateEnd = DateTime.Today;

                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void InitializeCustomComponent()
        {
            // 平仓单统计
            #region
            //chartArea平仓单统计.AxisX.IsLabelAutoFit = false;
            //chartArea平仓单统计.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //chartArea平仓单统计.AxisX.MajorGrid.Enabled = false;
            //chartArea平仓单统计.AxisX.MajorTickMark.Enabled = true;
            //chartArea平仓单统计.AxisY.IsLabelAutoFit = false;
            //chartArea平仓单统计.AxisY.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //chartArea平仓单统计.AxisY.MajorGrid.Enabled = false;
            //chartArea平仓单统计.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            //chartArea平仓单统计.AxisY.MajorTickMark.Enabled = true;
            //chartArea平仓单统计.BackColor = System.Drawing.Color.White;
            //chartArea平仓单统计.Name = "_ChartArea平仓单统计";
            //this._chart平仓单统计.ChartAreas.Add(chartArea平仓单统计);
            //this._chart平仓单统计.Location = new System.Drawing.Point(3, 3);
            //this._chart平仓单统计.Name = "_chart平仓单统计";
            //series平仓单亏损.ChartArea = "_ChartArea平仓单统计";
            //series平仓单亏损.CustomProperties = "PieLabelStyle=Outside";
            //series平仓单亏损.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //series平仓单亏损.Name = "_Series平仓单亏损";
            //this._chart平仓单统计.Series.Add(series平仓单亏损);
            //series平仓单盈利.ChartArea = "_ChartArea平仓单统计";
            //series平仓单盈利.CustomProperties = "PieLabelStyle=Outside";
            //series平仓单盈利.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //series平仓单盈利.Name = "_Series平仓单盈利";
            //this._chart平仓单统计.Series.Add(series平仓单盈利);
            //this._chart平仓单统计.TabIndex = 4;
            //this._chart平仓单统计.Text = "平仓单统计";
            #endregion

            // 盈亏比例
            #region
            //chartArea1.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea1.BackColor = System.Drawing.Color.White;
            chartArea1.Name = "_ChartArea盈亏比例";
            _chart盈亏比例.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend盈亏比例";
            _chart盈亏比例.Legends.Add(legend1);
            _chart盈亏比例.Location = new System.Drawing.Point(3, 3);
            _chart盈亏比例.Name = "_chart盈亏比例";
            series盈亏比例.ChartArea = "_ChartArea盈亏比例";
            series盈亏比例.ChartType = SeriesChartType.Pie;
            series盈亏比例.CustomProperties = "PieLabelStyle=Outside";
            series盈亏比例.Legend = "Legend盈亏比例";
            series盈亏比例.Name = "_Series盈亏比例";
            dataPoint盈比.Label = "盈利";
            dataPoint盈比.BackGradientStyle = GradientStyle.None;
            dataPoint盈比.BackHatchStyle = ChartHatchStyle.None;
            dataPoint盈比.BorderColor = System.Drawing.Color.Transparent;
            dataPoint盈比.CustomProperties = "PieLineColor=Black";
            dataPoint盈比.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint亏比.Label = "亏损";
            dataPoint亏比.BorderColor = System.Drawing.Color.Transparent;
            dataPoint亏比.CustomProperties = "PieLineColor=Black";
            dataPoint亏比.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataPoint亏比.LabelForeColor = System.Drawing.Color.Black;
            dataPoint手续费比.Label = "手续费";
            dataPoint手续费比.CustomProperties = "PieLineColor=Black";
            dataPoint手续费比.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _chart盈亏比例.Series.Add(series盈亏比例);
            _chart盈亏比例.TabIndex = 4;
            _chart盈亏比例.Text = "盈亏比例";
            title1.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title1.DockedToChartArea = "_ChartArea盈亏比例";
            title1.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title1.Name = "_Title盈亏比例";
            title1.Text = "盈亏比例";
            _chart盈亏比例.Titles.Add(title1);
            #endregion

            // 多空比例
            #region
            //chartArea2.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea2.BackColor = System.Drawing.Color.White;
            chartArea2.Name = "_ChartArea多空比例";
            _chart多空比例.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend多空比例";
            _chart多空比例.Legends.Add(legend2);
            _chart多空比例.Location = new System.Drawing.Point(3, 3);
            _chart多空比例.Name = "_chart多空比例";
            series多空比例.ChartArea = "_ChartArea多空比例";
            series多空比例.ChartType = SeriesChartType.Pie;
            series多空比例.CustomProperties = "PieLabelStyle=Outside";
            series多空比例.Legend = "Legend多空比例";
            series多空比例.Name = "_Series多空";
            dataPoint空方比例.Label = "空头";
            dataPoint空方比例.BackGradientStyle = GradientStyle.None;
            dataPoint空方比例.BackHatchStyle = ChartHatchStyle.None;
            dataPoint空方比例.BorderColor = System.Drawing.Color.Transparent;
            dataPoint空方比例.CustomProperties = "PieLineColor=Black";
            dataPoint空方比例.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint多方比例.Label = "多头";
            dataPoint多方比例.BorderColor = System.Drawing.Color.Transparent;
            dataPoint多方比例.CustomProperties = "PieLineColor=Black";
            dataPoint多方比例.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataPoint多方比例.LabelForeColor = System.Drawing.Color.Black;
            _chart多空比例.Series.Add(series多空比例);
            _chart多空比例.TabIndex = 3;
            _chart多空比例.Text = "多空比例";
            title2.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title2.DockedToChartArea = "_ChartArea多空比例";
            title2.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title2.Name = "_Title多空比例";
            title2.Text = "多空比例";
            _chart多空比例.Titles.Add(title2);
            #endregion

            // 多空盈亏
            #region
            //chartArea3.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea3.BackColor = System.Drawing.Color.White;
            chartArea3.BorderDashStyle = ChartDashStyle.Dash;
            //chartArea3.BorderColor = System.Drawing.Color.#FFD5D891;
            chartArea3.Name = "_ChartArea盈利";
            //chartArea4.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea4.BackColor = System.Drawing.Color.White;
            chartArea4.BorderDashStyle = ChartDashStyle.Dash;
            //chartArea4.BorderColor = System.Drawing.Color.#FFD5D891;
            chartArea4.Name = "_ChartArea亏损";
            //chartArea5.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea5.BackColor = System.Drawing.Color.White;
            chartArea5.BorderDashStyle = ChartDashStyle.Dash;
            //chartArea5.BorderColor = System.Drawing.Color.#FFD5D891;
            chartArea5.Name = "_ChartArea多头";
            //chartArea6.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea6.BackColor = System.Drawing.Color.White;
            chartArea6.BorderDashStyle = ChartDashStyle.Dash;
            //chartArea6.BorderColor = System.Drawing.Color.#FFD5D891;
            chartArea6.Name = "_ChartArea空头";
            _chart多空盈亏.ChartAreas.Add(chartArea3);
            _chart多空盈亏.ChartAreas.Add(chartArea4);
            _chart多空盈亏.ChartAreas.Add(chartArea5);
            _chart多空盈亏.ChartAreas.Add(chartArea6);
            legend3.DockedToChartArea = "_ChartArea盈利";
            legend3.Name = "_Legend盈利";
            legend4.DockedToChartArea = "_ChartArea亏损";
            legend4.Name = "_Legend亏损";
            legend5.DockedToChartArea = "_ChartArea多头";
            legend5.Name = "_Legend多头";
            legend6.DockedToChartArea = "_ChartArea空头";
            legend6.Name = "_Legend空头";
            _chart多空盈亏.Legends.Add(legend3);
            _chart多空盈亏.Legends.Add(legend4);
            _chart多空盈亏.Legends.Add(legend5);
            _chart多空盈亏.Legends.Add(legend6);
            _chart多空盈亏.Location = new System.Drawing.Point(3, 3);
            _chart多空盈亏.Name = "_chart多空盈亏";
            series多空盈亏1.ChartArea = "_ChartArea盈利";
            series多空盈亏1.ChartType = SeriesChartType.Pie;
            series多空盈亏1.CustomProperties = "PieLabelStyle=Outside";
            series多空盈亏1.Legend = "_Legend盈利";
            series多空盈亏1.Name = "_Series盈利";
            dataPoint6.Label = "多头";
            dataPoint6.BackGradientStyle = GradientStyle.None;
            dataPoint6.BackHatchStyle = ChartHatchStyle.None;
            dataPoint6.BorderColor = System.Drawing.Color.Transparent;
            dataPoint6.CustomProperties = "PieLineColor=Black";
            dataPoint6.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint7.Label = "空头";
            dataPoint7.BorderColor = System.Drawing.Color.Transparent;
            dataPoint7.CustomProperties = "PieLineColor=Black";
            dataPoint7.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataPoint7.LabelForeColor = System.Drawing.Color.Black;
            series多空盈亏2.ChartArea = "_ChartArea亏损";
            series多空盈亏2.ChartType = SeriesChartType.Pie;
            series多空盈亏2.CustomProperties = "PieLabelStyle=Outside";
            series多空盈亏2.Legend = "_Legend亏损";
            series多空盈亏2.Name = "_Series亏损";
            dataPoint8.Label = "多头";
            dataPoint8.CustomProperties = "PieLabelStyle=Outside, PieLineColor=Black";
            dataPoint8.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint9.Label = "空头";
            dataPoint9.CustomProperties = "PieLineColor=Black";
            dataPoint9.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series多空盈亏3.ChartArea = "_ChartArea多头";
            series多空盈亏3.ChartType = SeriesChartType.Pie;
            series多空盈亏3.Legend = "_Legend多头";
            series多空盈亏3.Name = "_Series多头";
            dataPoint10.Label = "盈利";
            dataPoint10.CustomProperties = "PieLabelStyle=Outside, PieLineColor=Black";
            dataPoint10.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint11.Label = "亏损";
            dataPoint11.CustomProperties = "PieLabelStyle=Outside, PieLineColor=Black";
            dataPoint11.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series多空盈亏4.ChartArea = "_ChartArea空头";
            series多空盈亏4.ChartType = SeriesChartType.Pie;
            series多空盈亏4.CustomProperties = "PieLabelStyle=Outside";
            series多空盈亏4.Legend = "_Legend空头";
            series多空盈亏4.Name = "_Series空头";
            dataPoint12.Label = "盈利";
            dataPoint12.BorderColor = System.Drawing.Color.Transparent;
            dataPoint12.CustomProperties = "PieLineColor=Black";
            dataPoint12.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataPoint13.Label = "亏损";
            dataPoint13.CustomProperties = "PieLineColor=Black";
            dataPoint13.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            _chart多空盈亏.Series.Add(series多空盈亏1);
            _chart多空盈亏.Series.Add(series多空盈亏2);
            _chart多空盈亏.Series.Add(series多空盈亏3);
            _chart多空盈亏.Series.Add(series多空盈亏4);
            _chart多空盈亏.TabIndex = 1;
            _chart多空盈亏.Text = "多空盈亏";
            Title title3 = new Title();
            Title title4 = new Title();
            Title title5 = new Title();
            Title title6 = new Title();
            title3.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title3.DockedToChartArea = "_ChartArea盈利";
            title3.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title3.Name = "_Title盈利";
            title3.Text = "盈利";
            title4.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title4.DockedToChartArea = "_ChartArea亏损";
            title4.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title4.Name = "_Title亏损";
            title4.Text = "亏损";
            title5.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title5.DockedToChartArea = "_ChartArea多头";
            title5.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title5.Name = "_Title多头";
            title5.Text = "多头";
            title6.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title6.DockedToChartArea = "_ChartArea空头";
            title6.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            title6.Name = "_Title空头";
            title6.Text = "空头";
            _chart多空盈亏.Titles.Add(title3);
            _chart多空盈亏.Titles.Add(title4);
            _chart多空盈亏.Titles.Add(title5);
            _chart多空盈亏.Titles.Add(title6);
            #endregion

            // 持仓偏好
            #region
            //chartArea7.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea7.BackColor = System.Drawing.Color.White;
            chartArea7.Name = "_ChartArea持仓偏好";
            _chart持仓偏好.ChartAreas.Add(chartArea7);
            _chart持仓偏好.Location = new System.Drawing.Point(3, 3);
            _chart持仓偏好.Name = "_chart持仓偏好";
            series持仓偏好.ChartArea = "_ChartArea持仓偏好";
            series持仓偏好.ChartType = SeriesChartType.Pie;
            series持仓偏好.CustomProperties = "PieLabelStyle=Outside";
            series持仓偏好.Legend = "Legend1";
            series持仓偏好.Name = "_Series持仓偏好";
            _chart持仓偏好.Series.Add(series持仓偏好);
            _chart持仓偏好.TabIndex = 2;
            _chart持仓偏好.Text = "持仓偏好";

            title7.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title7.DockedToChartArea = "_ChartArea持仓偏好";
            title7.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title7.Name = "_Title持仓偏好";
            title7.Text = "持仓偏好";
            _chart持仓偏好.Titles.Add(title7);
            #endregion

            // 成交偏好
            #region
            //chartArea8.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea8.BackColor = System.Drawing.Color.White;
            chartArea8.Name = "_ChartArea持仓偏好";
            _chart成交偏好.ChartAreas.Add(chartArea8);
            _chart成交偏好.Location = new System.Drawing.Point(3, 3);
            _chart成交偏好.Name = "_chart成交偏好";
            series成交偏好.ChartArea = "_ChartArea持仓偏好";
            series成交偏好.ChartType = SeriesChartType.Pie;
            series成交偏好.CustomProperties = "PieLabelStyle=Outside";
            series成交偏好.Name = "_Series成交偏好";
            _chart成交偏好.Series.Add(series成交偏好);
            _chart成交偏好.TabIndex = 3;
            _chart成交偏好.Text = "成交偏好";
            title8.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title8.DockedToChartArea = "_ChartArea持仓偏好";
            title8.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title8.Name = "_Title成交偏好";
            title8.Text = "成交偏好";
            _chart成交偏好.Titles.Add(title8);
            #endregion

            // 每日持仓
            #region
            //chartArea11.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea11.AxisX.IsLabelAutoFit = false;
            chartArea11.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea11.AxisX.MajorGrid.Enabled = false;
            chartArea11.AxisX.MajorTickMark.Enabled = true;
            chartArea11.AxisY.IsLabelAutoFit = false;
            chartArea11.AxisY.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea11.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea11.AxisY.MajorTickMark.Enabled = true;
            chartArea11.BackColor = System.Drawing.Color.White;
            chartArea11.Name = "_ChartArea每日持仓";
            _chart每日持仓.ChartAreas.Add(chartArea11);
            _chart每日持仓.Location = new System.Drawing.Point(3, 3);
            _chart每日持仓.Name = "_chart每日持仓";
            series每日持仓.ChartArea = "_ChartArea每日持仓";
            series每日持仓.CustomProperties = "PieLabelStyle=Outside";
            series每日持仓.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series每日持仓.Name = "_Series每日持仓";
            series每日持仓.ChartType = SeriesChartType.Column;
            _chart每日持仓.Series.Add(series每日持仓);
            _chart每日持仓.TabIndex = 4;
            _chart每日持仓.Text = "每日持仓";
            title11.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title11.DockedToChartArea = "_ChartArea每日持仓";
            title11.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title11.Name = "_Title每日持仓";
            title11.Text = "每日持仓";
            _chart每日持仓.Titles.Add(title11);
            #endregion

            // 品种盈亏
            #region
            //chartArea12.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea12.AxisX.IsLabelAutoFit = false;
            chartArea12.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea12.AxisX.MajorGrid.Enabled = false;
            chartArea12.AxisX.MajorTickMark.Enabled = true;
            chartArea12.AxisY.IsLabelAutoFit = false;
            chartArea12.AxisY.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea12.AxisY.MajorGrid.Enabled = false;
            chartArea12.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea12.AxisY.MajorTickMark.Enabled = true;
            chartArea12.BackColor = System.Drawing.Color.White;
            chartArea12.Name = "_ChartArea品种盈亏";
            _chart品种盈亏.ChartAreas.Add(chartArea12);
            _chart品种盈亏.Location = new System.Drawing.Point(3, 3);
            _chart品种盈亏.Name = "_chart品种盈亏";
            series品种盈亏.ChartArea = "_ChartArea品种盈亏";
            series品种盈亏.CustomProperties = "PieLabelStyle=Outside";
            series品种盈亏.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series品种盈亏.Name = "_Series品种盈亏";
            _chart品种盈亏.Series.Add(series品种盈亏);
            _chart品种盈亏.TabIndex = 4;
            _chart品种盈亏.Text = "品种盈亏";
            title12.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title12.DockedToChartArea = "_ChartArea品种盈亏";
            title12.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title12.Name = "_Title品种盈亏";
            title12.Text = "品种盈亏";
            _chart品种盈亏.Titles.Add(title12);
            #endregion

            // 每周盈亏
            #region
            //chartArea9.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea9.AxisX.IsLabelAutoFit = false;
            chartArea9.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea9.AxisX.MajorGrid.Enabled = false;
            chartArea9.AxisX.MajorTickMark.Enabled = true;
            chartArea9.AxisY.IsLabelAutoFit = true;
            chartArea9.AxisY.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //chartArea9.AxisY.MajorGrid.Enabled = false;
            chartArea9.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dash;
            chartArea9.AxisY.MajorTickMark.Enabled = true;
            chartArea9.BackColor = System.Drawing.Color.White;
            chartArea9.Name = "_ChartArea每周盈亏";
            _chart每周盈亏.ChartAreas.Add(chartArea9);
            _chart每周盈亏.Location = new System.Drawing.Point(3, 3);
            _chart每周盈亏.Name = "_chart每周盈亏";
            series每周盈亏.ChartArea = "_ChartArea每周盈亏";
            series每周盈亏.CustomProperties = "PieLabelStyle=Outside";
            series每周盈亏.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series每周盈亏.Name = "_Series每周盈亏";
            _chart每周盈亏.Series.Add(series每周盈亏);
            _chart每周盈亏.TabIndex = 4;
            _chart每周盈亏.Text = "每周盈亏";
            title9.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title9.DockedToChartArea = "_ChartArea每周盈亏";
            title9.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title9.Name = "_Title每周盈亏";
            title9.Text = "每周盈亏";
            _chart每周盈亏.Titles.Add(title9);
            #endregion

            // 每月盈亏
            #region
            //chartArea10.Area3DStyle.Enable3D = !this._button立体图.Enabled;
            chartArea10.AxisX.IsLabelAutoFit = false;
            chartArea10.AxisX.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea10.AxisX.MajorGrid.Enabled = false;
            chartArea10.AxisX.MajorTickMark.Enabled = true;
            chartArea10.AxisX.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea10.AxisX2.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea10.AxisY.IsLabelAutoFit = false;
            chartArea10.AxisY.LabelStyle.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //chartArea10.AxisY.MajorGrid.Enabled = false;
            chartArea10.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.DashDot;
            chartArea10.AxisY.MajorTickMark.Enabled = true;
            chartArea10.AxisY.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea10.AxisY2.TitleFont = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            chartArea10.BackColor = System.Drawing.Color.White;
            chartArea10.Name = "_ChartArea每月盈亏";
            _chart每月盈亏.ChartAreas.Add(chartArea10);
            _chart每月盈亏.Location = new System.Drawing.Point(3, 3);
            _chart每月盈亏.Name = "_chart每月盈亏";
            series每月盈亏.ChartArea = "_ChartArea每月盈亏";
            series每月盈亏.CustomProperties = "PieLabelStyle=Outside";
            series每月盈亏.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            series每月盈亏.Name = "_Series每月盈亏";
            _chart每月盈亏.Series.Add(series每月盈亏);
            _chart每月盈亏.TabIndex = 5;
            _chart每月盈亏.Text = "每月盈亏";
            title10.Alignment = System.Drawing.ContentAlignment.TopLeft;
            title10.DockedToChartArea = "_ChartArea每月盈亏";
            title10.Font = new System.Drawing.Font("华文细黑", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            title10.Name = "_Title每月盈亏";
            title10.Text = "每月盈亏";
            _chart每月盈亏.Titles.Add(title10);
            #endregion
        }

        private void _buttonStatistics_Click(object sender, RoutedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();
            AllButtonEnabled();
        }

        private void AllButtonEnabled()
        {
            _buttonDayStatistics.IsEnabled = true;
            _buttonWeekStatistics.IsEnabled = true;
            _buttonMonthStatistics.IsEnabled = true;
            _buttonYearStatistics.IsEnabled = true;
            _buttonAllStatistics.IsEnabled = true;
        }

        /// <summary>
        /// 统计数据
        /// </summary>
        public void RefreshStatistics()
        {
            using (StatementContext statement = new StatementContext())
            {
                var accc = statement.Account.ToList().FirstOrDefault(m => m.Id == _Session.SelectedAccountId);

                var commodifys = statement.Commodity;
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

                //
                if (startDate.HasValue && endDate.HasValue)
                {
#if true 
                    //当月开仓平仓单统计
                    if (accc.Type == 1)
                    {
                        #region
                        decimal profitTotal = 0;
                        decimal lossTotal = 0;
                        int longVolume = 0;
                        int shortVolume = 0;
                        int profitVolume = 0;
                        int lossVolume = 0;
                        decimal maxProfit = 0;
                        decimal maxLoss = 0;
                        //
                        List<ClosedTradeStatistics> closedTradeStatistics = new List<ClosedTradeStatistics>();
                        DateTime startDate1 = startDate.Value.Date;
                        DateTime endDate1 = endDate.Value.Date.AddDays(1);
                        //
                        var closedTradeDetails = statement.ClosedTradeDetail
                            .Where(ctd => ctd.AccountId == _Session.SelectedAccountId && ctd.ActualDate <= endDate1 && ctd.ActualDate >= startDate1)
                            .OrderBy(ctd => ctd.ActualDate).ToList();

                        foreach (var ct in closedTradeDetails)
                        {
                            // 开平手续费
                            var tradeDetailForOpen = statement.TradeDetail.ToList().FirstOrDefault(td => td.Ticket.Equals(ct.TicketForOpen));
                            if (tradeDetailForOpen == null)
                                continue;
                            if (tradeDetailForOpen.ActualTime >= new DateTime(endDate1.Year, endDate1.Month, endDate1.Day, 20, 0, 0))
                            {
                                continue;
                            }
                            var tradeDetailForClose = statement.TradeDetail.ToList().FirstOrDefault(td => td.Ticket.Equals(ct.TicketForClose));
                            decimal commission1 = tradeDetailForOpen.Commission + tradeDetailForClose.Commission;

                            // 平仓盈亏
                            decimal profit = tradeDetailForOpen.ActualTime.Date == tradeDetailForClose.ActualTime.Date ?
                                ct.ClosedProfit : ct.PriceForClose - ct.YesterdaySettlementPrice == 0 ? 0 : ct.ClosedProfit / (ct.PriceForClose - ct.YesterdaySettlementPrice) * (ct.PriceForClose - ct.PriceForOpen);
                            //var poss = statement.PositionDetails.Where(pos => pos.Ticket.Equals(ct.TicketForOpen));
                            //foreach (var pos in poss)
                            //{
                            //    profit += pos.Profit;
                            //}

                            // 净盈亏
                            profit -= commission1;

                            // 数量
                            ClosedTradeStatistics tmp = new ClosedTradeStatistics();
                            tmp.Item = ct.Item;
                            tmp.Size = ct.Size;

                            if (profit >= 0) // 如果此单盈利
                            {
                                maxProfit = profit > maxProfit ? profit : maxProfit;
                                profitTotal += profit;
                                profitVolume += ct.Size;
                                tmp.Profit += profit;
                            }
                            else // 如果此单亏损
                            {
                                maxLoss = profit < maxLoss ? profit : maxLoss;
                                lossTotal += profit;
                                lossVolume += ct.Size;
                                tmp.Loss += profit;
                            }
                            if (tradeDetailForOpen.BS.Equals("买")) // 如果是多头交易
                            {
                                longVolume += ct.Size;
                            }
                            else // 如果是空头交易
                            {
                                shortVolume += ct.Size;
                            }

                            //
                            tmp.Total = tmp.Profit + tmp.Loss;

                            // 如果已存在该合约的盈亏数据
                            var clost = closedTradeStatistics.ToList().FirstOrDefault(ctt => ctt.Item.Equals(tmp.Item));
                            if (clost != null)
                            {
                                clost.Size += tmp.Size;
                                clost.Profit += tmp.Profit;
                                clost.Loss += tmp.Loss;
                                clost.Total += tmp.Total;
                            }
                            else
                            {
                                closedTradeStatistics.Add(tmp);
                            }
                        }
                        var account = statement.Account.ToList().FirstOrDefault(acc => acc.Id == _Session.SelectedAccountId);
                        var latestFundStatus = statement.FundStatus.Where(fs => fs.AccountId == _Session.SelectedAccountId).OrderByDescending(fs => fs.Date).ToList().FirstOrDefault();
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            if (latestFundStatus != null)
                            {
                                _textBlock账户.Text = string.Format("户名：{0}\t账号：{1}\t余额：{2}", account.CustomerName, account.AccountNumber, latestFundStatus.TodayBalance.ToString("c"));
                            }
                            Dispatcher.Invoke(new FormControlInvoker(() =>
                            {
                                _textBlock盈利总计.Text = profitTotal.ToString("n");
                                _textBlock亏损总计.Text = lossTotal.ToString("n");
                                _textBlock净利润.Text = (profitTotal + lossTotal).ToString("n");
                            }));
                            if (profitTotal + lossTotal > 0)
                            {
                                _textBlock净利润.Foreground = new SolidColorBrush(Colors.Red);
                            }
                            else if (profitTotal + lossTotal == 0)
                            {
                                _textBlock净利润.Foreground = new SolidColorBrush(Colors.Black);
                            }
                            else
                            {
                                _textBlock净利润.Foreground = new SolidColorBrush(Colors.DarkCyan);
                            }
                            _textBlock交易笔数.Text = (longVolume + shortVolume).ToString();
                            _textBlock多头笔数.Text = string.Format("{0} ({1:p0})", longVolume.ToString(), (double)longVolume / (longVolume + shortVolume));
                            _textBlock空头笔数.Text = string.Format("{0} ({1:p0})", shortVolume.ToString(), (double)shortVolume / (longVolume + shortVolume));
                            _textBlock盈利笔数.Text = string.Format("{0} ({1:p0})", profitVolume.ToString(), (double)profitVolume / (profitVolume + lossVolume));
                            _textBlock亏损笔数.Text = string.Format("{0} ({1:p0})", lossVolume.ToString(), (double)lossVolume / (profitVolume + lossVolume));
                            _textBlock最大盈利单.Text = maxProfit.ToString("n");
                            _textBlock最大亏损单.Text = maxLoss.ToString("n");
                            _textBlock平均每笔盈利.Text = profitVolume == 0 ? "0.00" : (profitTotal / profitVolume).ToString("n");
                            _textBlock平均每笔亏损.Text = lossVolume == 0 ? "0.00" : (lossTotal / lossVolume).ToString("n");
                        }));

                        //this.Dispatcher.Invoke(new FormControlInvoker(() =>
                        //{
                        //    series平仓单盈利.Points.Clear();
                        //    series平仓单亏损.Points.Clear();
                        //    for (int i = 0; i < closedTradeStatistics.Count(); i++)
                        //    {
                        //        if (closedTradeStatistics[i].Profit != 0)
                        //        {
                        //            DataPoint dataPoint = new DataPoint(i + 1, closedTradeStatistics[i].Profit.ToString("0.00"));
                        //            dataPoint.AxisLabel = closedTradeStatistics[i].Item;
                        //            //dataPoint.AxisLabel = commodityProfits[i].Commodity;
                        //            dataPoint.ToolTip = string.Format("{0}盈利：{1:n}（净利润：{2:n}）", closedTradeStatistics[i].Item, closedTradeStatistics[i].Profit, closedTradeStatistics[i].Profit + closedTradeStatistics[i].Loss);
                        //            dataPoint.BackGradientStyle = GradientStyle.None;
                        //            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                        //            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                        //            dataPoint.CustomProperties = "PieLineColor=Black";
                        //            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //            dataPoint.Color = System.Drawing.Color.Red;
                        //            series平仓单亏损.Points.Add(dataPoint);
                        //        }
                        //        if (closedTradeStatistics[i].Loss != 0)
                        //        {
                        //            DataPoint dataPoint = new DataPoint(i + 1, (closedTradeStatistics[i].Loss * -1).ToString("0.00"));
                        //            dataPoint.AxisLabel = closedTradeStatistics[i].Item;
                        //            //dataPoint.AxisLabel = commodityProfits[i].Commodity;
                        //            dataPoint.ToolTip = string.Format("{0}亏损：{1:n}", closedTradeStatistics[i].Item, closedTradeStatistics[i].Loss);
                        //            dataPoint.BackGradientStyle = GradientStyle.None;
                        //            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                        //            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                        //            dataPoint.CustomProperties = "PieLineColor=Black";
                        //            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                        //            dataPoint.Color = System.Drawing.Color.DarkCyan;
                        //            series平仓单盈利.Points.Add(dataPoint);
                        //        }
                        //    }
                        //}));
                        #endregion
                    } 
#endif
                    // 统计数据
                    #region
                    //
                    // 统计变量
                    //
                    decimal won = 0, loss = 0, commission = 0;
                    decimal longAmount = 0, shortAmount = 0;
                    decimal longWon = 0, longLoss = 0, shortWon = 0, shortLoss = 0;
                    List<TradeInterest> tradeInterests = new List<TradeInterest>();
                    List<PositionInterest> positionInterests = new List<PositionInterest>();
                    List<DayPosition> dayPositions = new List<DayPosition>();
                    List<CommodityProfit> commodityProfits = new List<CommodityProfit>();
                    List<Stock> weekProfit = new List<Stock>();
                    List<Stock> monthProfit = new List<Stock>();
                    //
                    // 多、空、盈、亏、手续费、交易偏好统计
                    //
                    DateTime startDate2 = startDate.Value.Date;
                    DateTime endDate2 = endDate.Value.Date.AddDays(1);
                    var tradeDetails = statement.TradeDetail.Where(td => td.ActualTime <= endDate2 && td.ActualTime >= startDate2 && td.AccountId == _Session.SelectedAccountId).OrderBy(td => td.ActualTime);

                    foreach (var td in tradeDetails)
                    {
                        //
                        if (!td.OC.Equals("开")) //平仓数据
                        {
                            if (td.ClosedProfit >= 0)
                            {
                                won += td.ClosedProfit;
                                if (td.BS.Equals("卖"))
                                    longWon += td.ClosedProfit;
                                else
                                    shortWon += td.ClosedProfit;
                            }
                            else
                            {
                                loss += td.ClosedProfit;
                                if (td.BS.Equals("卖"))
                                    longLoss += td.ClosedProfit;
                                else
                                    shortLoss += td.ClosedProfit;
                            }
                        }
                        else
                        {
                            // 交易偏好
                            TradeInterest ti = new TradeInterest();
                            ti.Commodity = Item2Commodity(td.Item);
                            ti.Amount = td.Amount;
                            if (tradeInterests.Contains<TradeInterest>(ti))
                            {
                                tradeInterests.FirstOrDefault(t => t.Commodity.Equals(ti.Commodity)).Amount += ti.Amount;
                            }
                            else
                            {
                                tradeInterests.Add(ti);
                            }
                            // 多空比例
                            if (td.BS.Equals("买"))
                                longAmount += td.Amount;
                            if (td.BS.Equals("卖"))
                                shortAmount += td.Amount;
                        }
                        //
                        CommodityProfit cp = new CommodityProfit();
                        cp.Commodity = Item2Commodity(td.Item);
                        cp.Profit = td.ClosedProfit-td.Commission;
                        cp.Commission = td.Commission;
                        if (commodityProfits.Contains(cp))
                        {
                            var aa = commodityProfits.FirstOrDefault(c => c.Commodity.Equals(cp.Commodity));
                            aa.Profit += cp.Profit;
                            aa.Commission += cp.Commission;
                        }
                        else
                        {
                            commodityProfits.Add(cp);
                        }
                        commission += td.Commission;
                    }

                    //
                    // 持仓中的盈亏
                    //
                    var positionDetails = statement.Position.Where(p => p.Date < endDate2 && p.Date >= startDate2 && p.AccountId == _Session.SelectedAccountId).OrderBy(p => p.Date);
                    foreach (var pds in positionDetails)
                    {
                        if (pds.Profit >= 0)
                        {
                            won += pds.Profit;
                            if (pds.BuySize != 0)
                                longWon += pds.Profit;
                            else
                                shortWon += pds.Profit;
                        }
                        else
                        {
                            loss += pds.Profit;
                            if (pds.BuySize != 0)
                                longLoss += pds.Profit;
                            else
                                shortLoss += pds.Profit;
                        }
                        //
                        CommodityProfit cp = new CommodityProfit();
                        cp.Commodity = Item2Commodity(pds.Item);
                        cp.Profit = pds.Profit;
                        if (commodityProfits.Contains(cp))
                        {
                            commodityProfits.FirstOrDefault(c => c.Commodity.Equals(cp.Commodity)).Profit += cp.Profit;
                        }
                        else
                        {
                            commodityProfits.Add(cp);
                        }
                    }


                    //
                    // 持仓偏好
                    //
                    var positions = statement.Position.Where(p => p.Date < endDate2 && p.Date >= startDate2 && p.AccountId == _Session.SelectedAccountId).OrderBy(p => p.Date);
                    foreach (var pos in positions)
                    {
                        PositionInterest pi = new PositionInterest();
                        pi.Commodity = Item2Commodity(pos.Item);
                        pi.Night = 1;
                        if (positionInterests.Contains<PositionInterest>(pi))
                        {
                            positionInterests.FirstOrDefault(p => p.Commodity.Equals(pi.Commodity)).Night += pi.Night;
                        }
                        else
                        {
                            positionInterests.Add(pi);
                        }
                    }
                    //
                    // 每日持仓
                    //
                    var fundStatus = statement.FundStatus.Where(fs => fs.Date < endDate2 && fs.Date >= startDate2 && fs.AccountId == _Session.SelectedAccountId).OrderBy(p => p.Date).ToList();
                    foreach (var fs in fundStatus)
                    {
                        DayPosition dpo = new DayPosition();
                        dpo.PositionDate = fs.Date;
                        if (fs.Margin > 0)
                        {
                            var ps = statement.Position.Where(p => p.Date == fs.Date && p.AccountId == _Session.SelectedAccountId);
                            foreach (var pos in ps)
                            {
                                if (pos.BuySize > 0 && pos.SaleSize > 0)
                                {
                                    DayPosition dp = new DayPosition();
                                    dp.PositionDate = pos.Date;
                                    dp.BuyMargin = pos.Margin / (pos.BuySize + pos.SaleSize) * pos.BuySize;
                                    dp.SaleMargin = pos.Margin - dp.BuyMargin;
                                    if (dayPositions.Contains<DayPosition>(dp))
                                    {
                                        dayPositions.FirstOrDefault(d => d.PositionDate == dp.PositionDate).BuyMargin += dp.BuyMargin;
                                        dayPositions.FirstOrDefault(d => d.PositionDate == dp.PositionDate).SaleMargin += dp.SaleMargin;
                                    }
                                    else
                                    {
                                        dayPositions.Add(dp);
                                    }
                                }
                                else
                                {
                                    DayPosition dp = new DayPosition();
                                    dp.PositionDate = pos.Date;
                                    if (pos.BuySize > 0)
                                    {
                                        dp.BuyMargin = pos.Margin;
                                    }
                                    else
                                    {
                                        dp.SaleMargin = pos.Margin;
                                    }
                                    if (dayPositions.Contains<DayPosition>(dp))
                                    {
                                        if (pos.BuySize > 0)
                                            dayPositions.FirstOrDefault(d => d.PositionDate == dp.PositionDate).BuyMargin += dp.BuyMargin;
                                        else
                                            dayPositions.FirstOrDefault(d => d.PositionDate == dp.PositionDate).SaleMargin += dp.SaleMargin;
                                    }
                                    else
                                    {
                                        dayPositions.Add(dp);
                                    }
                                }
                            }
                        }
                        else
                        {
                            dayPositions.Add(dpo);
                        }
                    }

                    //
                    // 周收益统计
                    //
                    var _dayStocks = statement.Stock.Where(o => o.AccountId == _Session.SelectedAccountId && o.Date >= startDate2 && o.Date < endDate2).OrderBy(o => o.Date);
                    if (_dayStocks != null && _dayStocks.Count() > 0)
                    {
                        var firstStock = _dayStocks.OrderBy(m => m.Date).ToList().FirstOrDefault();
                        DateTime date = firstStock.Date.Date;
                        decimal open = firstStock.Open;
                        decimal close = firstStock.Close;

                        Stock s = null;
                        foreach (var ds in _dayStocks)
                        {
                            if (ds.Date.DayOfWeek < date.DayOfWeek)
                            {
                                s = new Stock();
                                s.Date = date;
                                s.Open = open;
                                s.Close = close;
                                weekProfit.Add(s);

                                date = ds.Date.Date;
                                open = ds.Open;
                                close = ds.Close;
                            }
                            else if ((ds.Date.Date - date.Date).Days >= 7)
                            {
                                s = new Stock();
                                s.Date = date;
                                s.Open = open;
                                s.Close = close;
                                weekProfit.Add(s);

                                date = ds.Date.Date;
                                open = ds.Open;
                                close = ds.Close;
                            }
                            else
                            {
                                close = ds.Close;
                            }
                        }
                        s = new Stock();
                        s.Date = date;
                        s.Open = open;
                        s.Close = close;
                        weekProfit.Add(s);
                        //
                        // 月收益统计
                        //
                        date = firstStock.Date.Date;
                        open = firstStock.Open;
                        close = firstStock.Close;
                        foreach (var ds in _dayStocks)
                        {
                            if (ds.Date.Month != date.Month)
                            {
                                s = new Stock();
                                s.Date = date;
                                s.Open = open;
                                s.Close = close;
                                monthProfit.Add(s);

                                date = ds.Date.Date;
                                open = ds.Open;
                                close = ds.Close;
                            }
                            else
                            {
                                close = ds.Close;
                            }
                        }
                        s = new Stock();
                        s.Date = date;
                        s.Open = open;
                        s.Close = close;
                        monthProfit.Add(s);
                    }
                    #endregion

                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        // 盈亏比例
                        #region
                        dataPoint盈比.YValues[0] = (double)won;
                        dataPoint亏比.YValues[0] = (double)loss;
                        dataPoint手续费比.YValues[0] = (double)commission;

                        dataPoint盈比.ToolTip = string.Format("盈利：{0:n}元，净盈利：{1:n}元", won, won + loss - commission);
                        dataPoint亏比.ToolTip = string.Format("亏损：{0:n}元", loss);
                        dataPoint手续费比.ToolTip = string.Format("手续费：{0:n}元", commission.ToString());

                        dataPoint盈比.LegendText = dataPoint盈比.ToolTip;
                        dataPoint亏比.LegendText = dataPoint亏比.ToolTip;
                        dataPoint手续费比.LegendText = dataPoint手续费比.ToolTip;

                        dataPoint盈比.Color = System.Drawing.Color.Red;
                        dataPoint亏比.Color = System.Drawing.Color.DarkCyan;
                        dataPoint手续费比.Color = System.Drawing.Color.DarkGreen;

                        series盈亏比例.Points.Clear();
                        series盈亏比例.Points.Add(dataPoint盈比);
                        series盈亏比例.Points.Add(dataPoint亏比);
                        series盈亏比例.Points.Add(dataPoint手续费比);
                        #endregion

                        // 多空比例
                        #region
                        dataPoint空方比例.YValues[0] = (double)shortAmount;
                        dataPoint多方比例.YValues[0] = (double)longAmount;

                        dataPoint空方比例.ToolTip = string.Format("空头成交额：{0:n}", shortAmount);
                        dataPoint多方比例.ToolTip = string.Format("多头成交额：{0:n}", longAmount);

                        dataPoint空方比例.LegendText = dataPoint空方比例.ToolTip;
                        dataPoint多方比例.LegendText = dataPoint多方比例.ToolTip;

                        dataPoint空方比例.Color = System.Drawing.Color.DarkCyan;
                        dataPoint多方比例.Color = System.Drawing.Color.Red;
                        series多空比例.Points.Clear();
                        series多空比例.Points.Add(dataPoint空方比例);
                        series多空比例.Points.Add(dataPoint多方比例);
                        #endregion

                        // 多空盈亏
                        #region
                        dataPoint6.YValues[0] = (double)longWon;
                        dataPoint6.ToolTip = string.Format("多头盈利：{0}", longWon.ToString("n"));
                        dataPoint7.YValues[0] = (double)shortWon;
                        dataPoint7.ToolTip = string.Format("空头盈利：{0}", shortWon.ToString("n"));
                        dataPoint8.YValues[0] = (double)longLoss;
                        dataPoint8.ToolTip = string.Format("多头亏损：{0}", longLoss.ToString("n"));
                        dataPoint9.YValues[0] = (double)shortLoss;
                        dataPoint9.ToolTip = string.Format("空头亏损：{0}", shortLoss.ToString("n"));
                        dataPoint10.YValues[0] = (double)longWon;
                        dataPoint10.ToolTip = string.Format("多头盈利：{0}", longWon.ToString("n"));
                        dataPoint11.YValues[0] = (double)longLoss;
                        dataPoint11.ToolTip = string.Format("多头亏损：{0}", longLoss.ToString("n"));
                        dataPoint12.YValues[0] = (double)shortWon;
                        dataPoint12.ToolTip = string.Format("空头盈利：{0}", shortWon.ToString("n"));
                        dataPoint13.YValues[0] = (double)shortLoss;
                        dataPoint13.ToolTip = string.Format("空头亏损：{0}", shortLoss.ToString("n"));
                        dataPoint6.Color = System.Drawing.Color.Red;
                        dataPoint7.Color = System.Drawing.Color.DarkCyan;
                        dataPoint8.Color = System.Drawing.Color.Red;
                        dataPoint9.Color = System.Drawing.Color.DarkCyan;
                        dataPoint10.Color = System.Drawing.Color.Red;
                        dataPoint11.Color = System.Drawing.Color.DarkCyan;
                        dataPoint12.Color = System.Drawing.Color.Red;
                        dataPoint13.Color = System.Drawing.Color.DarkCyan;
                        series多空盈亏1.Points.Clear();
                        series多空盈亏1.Points.Add(dataPoint6);
                        series多空盈亏1.Points.Add(dataPoint7);
                        series多空盈亏2.Points.Clear();
                        series多空盈亏2.Points.Add(dataPoint8);
                        series多空盈亏2.Points.Add(dataPoint9);
                        series多空盈亏3.Points.Clear();
                        series多空盈亏3.Points.Add(dataPoint10);
                        series多空盈亏3.Points.Add(dataPoint11);
                        series多空盈亏4.Points.Clear();
                        series多空盈亏4.Points.Add(dataPoint12);
                        series多空盈亏4.Points.Add(dataPoint13);
                        #endregion

                        // 持仓偏好
                        #region
                        series持仓偏好.Points.Clear();
                        for (int i = 0; i < positionInterests.Count(); i++)
                        {
                            DataPoint dataPoint = new DataPoint(i, positionInterests[i].Night);
                            var comm = statement.Commodity.ToList().FirstOrDefault(m => m.Code.ToLower().Equals(positionInterests[i].Commodity.ToLower()));
                            string label = positionInterests[i].Commodity;
                            if (comm != null)
                            {
                                label = comm.Name;
                            }
                            dataPoint.Label = label;
                            dataPoint.ToolTip = string.Format("总计持仓{0}天", positionInterests[i].Night);
                            dataPoint.BackGradientStyle = GradientStyle.None;
                            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                            dataPoint.CustomProperties = "PieLineColor=Black";
                            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            series持仓偏好.Points.Add(dataPoint);
                        }
                        #endregion

                        // 成交偏好
                        #region
                        series成交偏好.Points.Clear();
                        for (int i = 0; i < tradeInterests.Count(); i++)
                        {
                            DataPoint dataPoint = new DataPoint(i, tradeInterests[i].Amount.ToString("0.00"));
                            var comm = statement.Commodity.ToList().FirstOrDefault(m => m.Code.ToLower().Equals(tradeInterests[i].Commodity.ToLower()));
                            string label = tradeInterests[i].Commodity;
                            if (comm != null)
                            {
                                label = comm.Name;
                            }
                            dataPoint.Label = label;
                            dataPoint.ToolTip = string.Format("成交额：{0:n}元", tradeInterests[i].Amount);
                            dataPoint.BackGradientStyle = GradientStyle.None;
                            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                            dataPoint.CustomProperties = "PieLineColor=Black";
                            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            series成交偏好.Points.Add(dataPoint);
                        }
                        #endregion

                        // 每日持仓
                        #region
                        series每日持仓.Points.Clear();
                        for (int i = 0; i < dayPositions.Count(); i++)
                        {
                            DataPoint buyDataPoint = new DataPoint(i + 1, dayPositions[i].BuyMargin.ToString("0.00"));
                            //dataPoint.Label ="";
                            buyDataPoint.ToolTip = string.Format("（多单）{0}：{1:N}", dayPositions[i].PositionDate.ToShortDateString(), dayPositions[i].BuyMargin);
                            buyDataPoint.BackGradientStyle = GradientStyle.None;
                            buyDataPoint.BackHatchStyle = ChartHatchStyle.None;
                            buyDataPoint.BorderColor = System.Drawing.Color.Transparent;
                            buyDataPoint.Color = System.Drawing.Color.Red;
                            buyDataPoint.CustomProperties = "PieLineColor=Black";
                            buyDataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            series每日持仓.Points.Add(buyDataPoint);
                            DataPoint saleDataPoint = new DataPoint(i + 1, (dayPositions[i].SaleMargin * (-1)).ToString("0.00"));
                            //dataPoint.Label ="";
                            saleDataPoint.ToolTip = string.Format("（空单）{0}：{1:N}", dayPositions[i].PositionDate.ToShortDateString(), dayPositions[i].SaleMargin);
                            saleDataPoint.BackGradientStyle = GradientStyle.None;
                            saleDataPoint.BackHatchStyle = ChartHatchStyle.None;
                            saleDataPoint.BorderColor = System.Drawing.Color.Transparent;
                            saleDataPoint.Color = System.Drawing.Color.DarkCyan;
                            saleDataPoint.CustomProperties = "PieLineColor=Black";
                            saleDataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            series每日持仓.Points.Add(saleDataPoint);
                        }
                        #endregion

                        // 品种盈亏
                        #region
                        series品种盈亏.Points.Clear();
                        for (int i = 0; i < commodityProfits.Count(); i++)
                        {
                            DataPoint dataPoint = new DataPoint(i + 1, commodityProfits[i].Profit.ToString("0.00"));
                            var comm = statement.Commodity.ToList().FirstOrDefault(m => m.Code.ToLower().Equals(commodityProfits[i].Commodity.ToLower()));
                            string label = commodityProfits[i].Commodity.ToLower();

                            if (comm != null)
                            {
                                label = comm.Name;
                            }
                            //try
                            //{
                            //    label = _Session.Commoditys[label];
                            //}
                            //catch (Exception)
                            //{
                            //}

                            dataPoint.Label = label;
                            //dataPoint.AxisLabel = commodityProfits[i].Commodity;
                            dataPoint.ToolTip = string.Format("{0}：盈利：{1:n}   手续费：{2:n}", commodityProfits[i].Commodity, commodityProfits[i].Profit, commodityProfits[i].Commission);
                            dataPoint.BackGradientStyle = GradientStyle.None;
                            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                            dataPoint.CustomProperties = "PieLineColor=Black";
                            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            if (commodityProfits[i].Profit >= 0)
                            {
                                dataPoint.Color = System.Drawing.Color.Red;
                            }
                            else
                            {
                                dataPoint.Color = System.Drawing.Color.DarkCyan;
                            }
                            series品种盈亏.Points.Add(dataPoint);
                        }
                        #endregion

                        // 每周盈亏
                        #region
                        series每周盈亏.Points.Clear();
                        for (int i = 0; i < weekProfit.Count(); i++)
                        {
                            DataPoint dataPoint = new DataPoint(i + 1, (weekProfit[i].Close - weekProfit[i].Open).ToString("0.00"));
                            //dataPoint.Label ="";
                            dataPoint.ToolTip = string.Format("{0}：{1:n}", weekProfit[i].Date.ToShortDateString(), weekProfit[i].Close - weekProfit[i].Open);
                            dataPoint.BackGradientStyle = GradientStyle.None;
                            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                            dataPoint.CustomProperties = "PieLineColor=Black";
                            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            if (weekProfit[i].Close - weekProfit[i].Open >= 0)
                            {
                                dataPoint.Color = System.Drawing.Color.Red;
                            }
                            else
                            {
                                dataPoint.Color = System.Drawing.Color.DarkCyan;
                            }
                            series每周盈亏.Points.Add(dataPoint);
                        }
                        #endregion

                        // 每月盈亏
                        #region
                        series每月盈亏.Points.Clear();
                        for (int i = 0; i < monthProfit.Count(); i++)
                        {
                            DataPoint dataPoint = new DataPoint(i + 1, (monthProfit[i].Close - monthProfit[i].Open).ToString("0.00"));
                            //dataPoint.Label ="";
                            dataPoint.ToolTip = string.Format("{0}：{1:n}", monthProfit[i].Date.ToShortDateString(), monthProfit[i].Close - monthProfit[i].Open);
                            dataPoint.BackGradientStyle = GradientStyle.None;
                            dataPoint.BackHatchStyle = ChartHatchStyle.None;
                            dataPoint.BorderColor = System.Drawing.Color.Transparent;
                            dataPoint.CustomProperties = "PieLineColor=Black";
                            dataPoint.Font = new System.Drawing.Font("微软雅黑", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                            if (monthProfit[i].Close - monthProfit[i].Open >= 0)
                            {
                                dataPoint.Color = System.Drawing.Color.Red;
                            }
                            else
                            {
                                dataPoint.Color = System.Drawing.Color.DarkCyan;
                            }
                            series每月盈亏.Points.Add(dataPoint);
                        }
                        #endregion
                    }));

                    Dispatcher.Invoke(new FormControlInvoker(() =>
                    {
                        _loading.Visibility = System.Windows.Visibility.Hidden;
                    }));
                }
            }
        }

        /// <summary>
        /// 截取合约中品种代码
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private string Item2Commodity(string item)
        {
            char[] nums = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            return item.Substring(0, item.IndexOfAny(nums));
        }

        private void To3D()
        {
            //this._chart平仓单统计.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart成交偏好.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart持仓偏好.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart多空比例.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart多空盈亏.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart多空盈亏.ChartAreas[1].Area3DStyle.Enable3D = true;
            _chart多空盈亏.ChartAreas[2].Area3DStyle.Enable3D = true;
            _chart多空盈亏.ChartAreas[3].Area3DStyle.Enable3D = true;
            _chart每日持仓.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart每月盈亏.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart每周盈亏.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart品种盈亏.ChartAreas[0].Area3DStyle.Enable3D = true;
            _chart盈亏比例.ChartAreas[0].Area3DStyle.Enable3D = true;
        }

        private void To2D()
        {
            //this._chart平仓单统计.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart成交偏好.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart持仓偏好.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart多空比例.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart多空盈亏.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart多空盈亏.ChartAreas[1].Area3DStyle.Enable3D = false;
            _chart多空盈亏.ChartAreas[2].Area3DStyle.Enable3D = false;
            _chart多空盈亏.ChartAreas[3].Area3DStyle.Enable3D = false;
            _chart每日持仓.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart每月盈亏.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart每周盈亏.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart品种盈亏.ChartAreas[0].Area3DStyle.Enable3D = false;
            _chart盈亏比例.ChartAreas[0].Area3DStyle.Enable3D = false;
        }

        //private void _button平面图_Click(object sender, RoutedEventArgs e)
        //{
        //    this._button立体图.IsEnabled = true;
        //    this._button平面图.IsEnabled = false;
        //    this.To2D();
        //    FAHelper.SetParameter(ParameterName.Statistics风格.ToString(), this._button平面图.Content.ToString());
        //}

        //private void _button立体图_Click(object sender, RoutedEventArgs e)
        //{
        //    this._button立体图.IsEnabled = false;
        //    this._button平面图.IsEnabled = true;
        //    this.To3D();
        //    FAHelper.SetParameter(ParameterName.Statistics风格.ToString(), this._button立体图.Content.ToString());
        //}

        private void _buttonDayStatistics_Click(object sender, RoutedEventArgs e)
        {
            AllButtonEnabled();
            _buttonDayStatistics.IsEnabled = false;
            _buttonDayStatistics._Refresh();
            //
            _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();

            //_Helper.SetParameter(ParameterName.StatisticsQueryCycle.ToString(), "day");
        }

        private void _buttonWeekStatistics_Click(object sender, RoutedEventArgs e)
        {
            AllButtonEnabled();
            _buttonWeekStatistics.IsEnabled = false;
            _buttonWeekStatistics._Refresh();
            //
            _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate.AddDays(DayOfWeek.Monday - _Session.LatestFundStatusDate.DayOfWeek);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();

            //_Helper.SetParameter(ParameterName.StatisticsQueryCycle.ToString(), "week");
        }

        private void _buttonMonthStatistics_Click(object sender, RoutedEventArgs e)
        {
            AllButtonEnabled();
            _buttonMonthStatistics.IsEnabled = false;
            _buttonMonthStatistics._Refresh();
            //
            _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, _Session.LatestFundStatusDate.Month, 1);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();

            //_Helper.SetParameter(ParameterName.StatisticsQueryCycle.ToString(), "month");
        }

        private void _buttonYearStatistics_Click(object sender, RoutedEventArgs e)
        {
            AllButtonEnabled();
            _buttonYearStatistics.IsEnabled = false;
            _buttonYearStatistics._Refresh();
            //
            _dateTimePicker开始.SelectedDate = new DateTime(_Session.LatestFundStatusDate.Year, 1, 1);
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();

            //_Helper.SetParameter(ParameterName.StatisticsQueryCycle.ToString(), "year");
        }

        private void _buttonAllStatistics_Click(object sender, RoutedEventArgs e)
        {
            AllButtonEnabled();
            _buttonAllStatistics.IsEnabled = false;
            _buttonAllStatistics._Refresh();
            //
            //
            var fds = new StatementContext().FundStatus.OrderBy(fs => fs.Date).ToList().FirstOrDefault();
            if (fds != null)
                _dateTimePicker开始.SelectedDate = fds.Date;
            else
                _dateTimePicker开始.SelectedDate = _Session.LatestFundStatusDate;
            _dateTimePicker结束.SelectedDate = _Session.LatestFundStatusDate;
            //
            Thread thread = new Thread(new ThreadStart(RefreshStatistics));
            thread.Start();

            //_Helper.SetParameter(ParameterName.StatisticsQueryCycle.ToString(), "all");
        }
    }
}
