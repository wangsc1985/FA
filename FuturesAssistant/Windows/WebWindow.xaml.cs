﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// WebWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WebWindow : Window
    {
        private string html = "";
        public WebWindow( string html)
        {
            InitializeComponent();
            this.html = html;
        }

        private void _confirm_Click(object sender, RoutedEventArgs e)
        {

        }

        private void _cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this._web
        }
    }
}
