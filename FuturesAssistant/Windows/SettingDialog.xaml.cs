using System;
using System.Collections.Generic;
using System.Linq;
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
    /// <summary>
    /// SettingDialog.xaml 的交互逻辑
    /// </summary>
    public partial class SettingDialog : DialogBase
    {
        public SettingDialog(MainWindow mainWindow)
        {
            InitializeComponent();
            Width = mainWindow.Width;
            Height = mainWindow.Height;
            Top = mainWindow.Top;
            Left = mainWindow.Left;
            _settingUserControl.Initialize(mainWindow);
        }
    }
}
