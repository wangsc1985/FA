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
    public partial class AverageParameterWindow : DialogBase
    {
        public int Average1Value { get; set; }
        public int Average2Value { get; set; }
        public int Average3Value { get; set; }
        public System.Drawing.Color Average1Color { get; set; }
        public System.Drawing.Color Average2Color { get; set; }
        public System.Drawing.Color Average3Color { get; set; }
        public bool Average1Visibility { get; set; }
        public bool Average2Visibility { get; set; }
        public bool Average3Visibility { get; set; }

        public AverageParameterWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _numericUpDownAverage1.Text = Average1Value.ToString();
            _numericUpDownAverage2.Text = Average2Value.ToString();
            _numericUpDownAverage3.Text = Average3Value.ToString();
            _labelAverage1Color.Background = new SolidColorBrush(Color.FromArgb(Average1Color.A, Average1Color.R, Average1Color.G, Average1Color.B));
            _labelAverage2Color.Background = new SolidColorBrush(Color.FromArgb(Average2Color.A, Average2Color.R, Average2Color.G, Average2Color.B));
            _labelAverage3Color.Background = new SolidColorBrush(Color.FromArgb(Average3Color.A, Average3Color.R, Average3Color.G, Average3Color.B));
            _checkBoxAverage1.IsChecked = Average1Visibility;
            _checkBoxAverage2.IsChecked = Average2Visibility;
            _checkBoxAverage3.IsChecked = Average3Visibility;
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

        private void _numericUpDownAverage1_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownAverage1.Text = Convert.ToInt32(_numericUpDownAverage1.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownAverage1.Text = Convert.ToInt32(_numericUpDownAverage1.Text) - 1 + "";
            }
        }

        private void _numericUpDownAverage2_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownAverage2.Text = Convert.ToInt32(_numericUpDownAverage2.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownAverage2.Text = Convert.ToInt32(_numericUpDownAverage2.Text) - 1 + "";
            }
        }

        private void _numericUpDownAverage3_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta == 120)
            {
                _numericUpDownAverage3.Text = Convert.ToInt32(_numericUpDownAverage3.Text) + 1 + "";
            }
            else if (e.Delta == -120)
            {
                _numericUpDownAverage3.Text = Convert.ToInt32(_numericUpDownAverage3.Text) - 1 + "";
            }
        }

        private void _numericUpDownAverage1_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToInt32(_numericUpDownAverage1.Text);
                Average1Value = Convert.ToInt32(_numericUpDownAverage1.Text);
            }
            catch (Exception)
            {
                _numericUpDownAverage1.Text = Average1Value.ToString();
            }
        }

        private void _numericUpDownAverage2_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToInt32(_numericUpDownAverage2.Text);
                Average2Value = Convert.ToInt32(_numericUpDownAverage2.Text);
            }
            catch (Exception)
            {
                _numericUpDownAverage2.Text = Average2Value.ToString();
            }
        }

        private void _numericUpDownAverage3_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Convert.ToInt32(_numericUpDownAverage3.Text);
                Average3Value = Convert.ToInt32(_numericUpDownAverage3.Text);
            }
            catch (Exception)
            {
                _numericUpDownAverage3.Text = Average3Value.ToString();
            }
        }

        private void _labelAverage3Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelAverage3Color.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                Average3Color = cd.Color;
            }
        }

        private void _labelAverage2Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelAverage2Color.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                Average2Color = cd.Color;
            }
        }

        private void _labelAverage1Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _labelAverage1Color.Background = new SolidColorBrush(Color.FromArgb(cd.Color.A, cd.Color.R, cd.Color.G, cd.Color.B));
                Average1Color = cd.Color;
            }
        }

        private void _window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_numericUpDownAverage1.IsFocused)
            {
                _numericUpDownAverage1_MouseWheel(sender, e);
            }
            else if (_numericUpDownAverage2.IsFocused)
            {
                _numericUpDownAverage2_MouseWheel(sender, e);
            }
            else if (_numericUpDownAverage3.IsFocused)
            {
                _numericUpDownAverage3_MouseWheel(sender, e);
            }
        }

        private void _checkBoxAverage2_Checked(object sender, RoutedEventArgs e)
        {
            Average2Visibility = true;
        }

        private void _checkBoxAverage1_Checked(object sender, RoutedEventArgs e)
        {
            Average1Visibility = true;
        }

        private void _checkBoxAverage1_Unchecked(object sender, RoutedEventArgs e)
        {
            Average1Visibility = false;
        }

        private void _checkBoxAverage2_Unchecked(object sender, RoutedEventArgs e)
        {
            Average2Visibility = false;
        }

        private void _checkBoxAverage3_Checked(object sender, RoutedEventArgs e)
        {
            Average3Visibility = true;
        }

        private void _checkBoxAverage3_Unchecked(object sender, RoutedEventArgs e)
        {
            Average3Visibility = false;
        }
    }
}
