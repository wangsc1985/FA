using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FuturesAssistant.Windows
{
    /// <summary>
    /// AverageParameterWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SetTargetDialog : DialogBase
    {
        public double MonthTargetValue { get; set; }
        public double QuarterTargetValue { get; set; }
        public double YearTargetValue { get; set; }
        public System.Drawing.Color MonthTargetColor { get; set; }
        public System.Drawing.Color QuarterTargetColor { get; set; }
        public System.Drawing.Color YearTargetColor { get; set; }
#if false
        public bool MonthTargetVisibility { get; set; }
        public bool QuarterTargetVisibility { get; set; }
        public bool YearTargetVisibility { get; set; }
#endif
        public SetTargetDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _numericUpDownMonthTarget.Text = MonthTargetValue.ToString();
            _numericUpDownQuarterTarget.Text = QuarterTargetValue.ToString();
            _numericUpDownYearTarget.Text = YearTargetValue.ToString();
            _labelMonthTargetColor.Background = new SolidColorBrush(Color.FromArgb(MonthTargetColor.A, MonthTargetColor.R, MonthTargetColor.G, MonthTargetColor.B));
            _labelQuarterTargetColor.Background = new SolidColorBrush(Color.FromArgb(QuarterTargetColor.A, QuarterTargetColor.R, QuarterTargetColor.G, QuarterTargetColor.B));
            _labelYearTargetColor.Background = new SolidColorBrush(Color.FromArgb(YearTargetColor.A, YearTargetColor.R, YearTargetColor.G, YearTargetColor.B));

#if false
            this._checkBoxMonthTarget.IsChecked = MonthTargetVisibility;
            this._checkBoxQuarterTarget.IsChecked = QuarterTargetVisibility;
            this._checkBoxYearTarget.IsChecked = YearTargetVisibility;
#endif
        }

        private void _button退出_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _button确认_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void _window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_numericUpDownMonthTarget.IsFocused)
            {
                _numericUpDownMonthTarget_MouseWheel(sender, e);
            }
            else if (_numericUpDownQuarterTarget.IsFocused)
            {
                _numericUpDownQuarterTarget_MouseWheel(sender, e);
            }
            else if (_numericUpDownYearTarget.IsFocused)
            {
                _numericUpDownYearTarget_MouseWheel(sender, e);
            }
        }

        private void _numericUpDownQuarterTarget_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownQuarterTarget.Text = Convert.ToInt32(_numericUpDownQuarterTarget.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownQuarterTarget.Text = Convert.ToInt32(_numericUpDownQuarterTarget.Text) - 1 + "";
            }
        }

        private void _numericUpDownYearTarget_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownYearTarget.Text = Convert.ToInt32(_numericUpDownYearTarget.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownYearTarget.Text = Convert.ToInt32(_numericUpDownYearTarget.Text) - 1 + "";
            }
        }

        private void _numericUpDownMonthTarget_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownMonthTarget.Text = Convert.ToInt32(_numericUpDownMonthTarget.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownMonthTarget.Text = Convert.ToInt32(_numericUpDownMonthTarget.Text) - 1 + "";
            }
        }

        private void _numericUpDownMonthTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDouble(_numericUpDownMonthTarget.Text);
                MonthTargetValue = Convert.ToDouble(_numericUpDownMonthTarget.Text);
            }
            catch (Exception)
            {
                _numericUpDownMonthTarget.Text = MonthTargetValue.ToString();
            }
        }


        private void _numericUpDownQuarterTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDouble(_numericUpDownQuarterTarget.Text);
                QuarterTargetValue = Convert.ToDouble(_numericUpDownQuarterTarget.Text);
            }
            catch (Exception)
            {
                _numericUpDownQuarterTarget.Text = QuarterTargetValue.ToString();
            }
        }

        private void _numericUpDownYearTarget_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToDouble(_numericUpDownYearTarget.Text);
                YearTargetValue = Convert.ToDouble(_numericUpDownYearTarget.Text);
            }
            catch (Exception)
            {
                _numericUpDownYearTarget.Text = YearTargetValue.ToString();
            }
        }
        
#if false
        private void _checkBoxMonthTarget_Checked(object sender, RoutedEventArgs e)
        {
            this.MonthTargetVisibility = true;
        }

        private void _checkBoxMonthTarget_Unchecked(object sender, RoutedEventArgs e)
        {
            this.MonthTargetVisibility = false;
        }

        private void _checkBoxQuarterTarget_Checked(object sender, RoutedEventArgs e)
        {
            this.QuarterTargetVisibility = true;
        }

        private void _checkBoxQuarterTarget_Unchecked(object sender, RoutedEventArgs e)
        {
            this.QuarterTargetVisibility = false;
        }

        private void _checkBoxYearTarget_Checked(object sender, RoutedEventArgs e)
        {
            this.YearTargetVisibility = true;
        }

        private void _checkBoxYearTarget_Unchecked(object sender, RoutedEventArgs e)
        {
            this.YearTargetVisibility = false;
        }
#endif
        private void _labelMonthTargetColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelMonthTargetColor.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                MonthTargetColor = cd.Color;
            }
        }

        private void _labelQuarterTargetColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelQuarterTargetColor.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                QuarterTargetColor = cd.Color;
            }
        }

        private void _labelYearTargetColor_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelYearTargetColor.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                YearTargetColor = cd.Color;
            }
        }
    }
}
