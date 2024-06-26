using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuturesAssistant.Controls
{
    /// <summary>
    /// MarginUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class MarginUserControl : UserControl
    {
        private delegate void FormControlInvoker();

        public MarginUserControl()
        {
            InitializeComponent();
        }


        private void CheckMargin()
        {
            //
            using (StatementContext statement = new StatementContext(typeof(FundStatus),typeof(Account)))
            {
                var latestFundStatus = statement.FundStatus.OrderByDescending(fs => fs.Date).FirstOrDefault(fs => fs.AccountId == _Session.SelectedAccountId);
                var account = statement.Accounts.FirstOrDefault(acc => acc.Id == _Session.SelectedAccountId);

                if (latestFundStatus != null)
                {
                    // 检查保证金
                    if (latestFundStatus.AdditionalMargin != 0)
                    {
                        Dispatcher.Invoke(new FormControlInvoker(() =>
                        {
                            _textBlockTitle.Text = account.FuturesCompanyName + "追加保证金通知";
                            _textBlockAccount.Text = "资金账户：" + account.AccountNumber;
                            _textBlockDate.Text = "通知时间：" + latestFundStatus.Date.ToString("yyyy年MM月dd日(ddd)");
                            _textBlockAddMargin.Text = "追加金额：" + latestFundStatus.AdditionalMargin.ToString("C");
                            _textBlockCurrentDate.Text = DateTime.Today.ToString("yyyy年MM月dd日(ddd)");
                            _textBlockTitle.Visibility = System.Windows.Visibility.Visible;
                            _textBlockAccount.Visibility = System.Windows.Visibility.Visible;
                            _textBlockDate.Visibility = System.Windows.Visibility.Visible;
                            _textBlockAddMargin.Visibility = System.Windows.Visibility.Visible;
                            _textBlockCurrentDate.Visibility = System.Windows.Visibility.Visible;
                            this._Refresh();
                        }));
                    }
                }
            }
        }
    }
}
