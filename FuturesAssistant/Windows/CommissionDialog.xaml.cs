﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Commission.xaml 的交互逻辑
    /// </summary>
    public partial class CommissionDialog : DialogBase
    {
        public CommissionDialog()
        {
            InitializeComponent();
            _exchangeCommissionUserControl.Initialize(true);
        }
    }
}
