using System;
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
    public class WindowBase : Window
    {
        public Button SettingButton
        {
            get;set;
        }
        public WindowBase()
        {
            App.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/Styles/WindowBaseStyle.xaml", UriKind.Relative)) as ResourceDictionary);
            App.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/Styles/GlobalStyle.xaml", UriKind.Relative)) as ResourceDictionary);
            Style = (Style)App.Current.Resources["BaseWindowStyle"];

            Loaded += delegate
            {
                InitializeEvent();
            };
        }

        private void InitializeEvent()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["BaseWindowControlTemplate"];

            TextBlock borderTitle = (TextBlock)baseWindowTemplate.FindName("_titleTextBlock", this);
            Button closeButton = (Button)baseWindowTemplate.FindName("_closeButton", this);
            SettingButton = (Button)baseWindowTemplate.FindName("_settingButton", this);
            Button minButton = (Button)baseWindowTemplate.FindName("_minButton", this);
            Button maxButton = (Button)baseWindowTemplate.FindName("_maxButton", this);
            Button colorButton = (Button)baseWindowTemplate.FindName("_colorButton", this);
            Image maxImage = (Image)baseWindowTemplate.FindName("_maxImage", this);
            colorButton.Click += delegate
            {
                System.Windows.Forms.ColorDialog dialog = new System.Windows.Forms.ColorDialog();
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    App.Current.Resources["ThemeColor"] = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);
            };
            colorButton.Visibility = Visibility.Collapsed;
            maxButton.Click += delegate
            {
                if (WindowState == WindowState.Normal)
                {
                    WindowState = WindowState.Maximized;
                    maxImage.Source = new BitmapImage(new Uri("/Images/normal.black.ico", UriKind.Relative));
                }
                else
                {
                    WindowState = WindowState.Normal;
                    maxImage.Source = new BitmapImage(new Uri("/Images/max.black.ico", UriKind.Relative));
                }
            };
            minButton.Click += delegate
            {
                WindowState = WindowState.Minimized;
            };

            closeButton.Click += delegate
            {
                Close();
            };

            borderTitle.MouseMove += delegate(object sender, MouseEventArgs e)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            };
        }

        private bool _allowSizeToContent = false;
        /// <summary>
        /// 自定义属性，用于标记该窗体是否允许按内容适应，设此属性是为了解决最大化按钮当SizeToContent属性为WidthAndHeight时不能最大化，从而最大、最小化必须变更SizeToContent的值的问题
        /// </summary>
        public bool AllowSizeToContent
        {
            get
            {
                return _allowSizeToContent;
            }
            set
            {
                SizeToContent = (value ? SizeToContent.WidthAndHeight : SizeToContent.Manual);
                _allowSizeToContent = value;
            }
        }
    }
}
