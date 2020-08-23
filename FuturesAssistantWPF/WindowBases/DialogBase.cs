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

namespace FuturesAssistantWPF.Windows
{
    public class DialogBase : Window
    {
        public DialogBase()
        {
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            App.Current.Resources.MergedDictionaries.Add(Application.LoadComponent(new Uri("/Styles/DialogBaseStyle.xaml", UriKind.Relative)) as ResourceDictionary);
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
