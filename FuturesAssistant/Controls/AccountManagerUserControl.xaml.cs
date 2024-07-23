using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using FuturesAssistant.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FuturesAssistant.Controls
{
    /// <summary>
    /// AccountManagerUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class AccountManagerUserControl : UserControl
    {
        private MainWindow mainWindow;
        private delegate void FormControlInvoker();
        public AccountManagerUserControl()
        {
            InitializeComponent();
        }

        public void InitializeAccountList()
        {
            using (StatementContext context = new StatementContext())
            {
                List<AccountListModel> accountListMain = new List<AccountListModel>();
                List<AccountListModel> accountListSettingJY = new List<AccountListModel>();
                List<AccountListModel> accountListSettingPZ = new List<AccountListModel>();
                //context.Accounts;
                foreach (var acc in context.Account.OrderByDescending(m => m.Type))
                {
                    var firstStock = context.Stock.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().FirstOrDefault();
                    var lastStock = context.Stock.Where(m => m.AccountId == acc.Id).OrderBy(m => m.Date).ToList().LastOrDefault();

                    // 非隐藏账户
                    if (acc.Type % 10 != 0)
                    {
                        accountListMain.Add(new AccountListModel()
                        {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            TypeForeground = (acc.Type == 1 || acc.Type == 10) ? "BLACK" : "RED"
                        });
                    }

                    // 配资账户
                    if (acc.Type == 2 || acc.Type == 20)
                    {
                        accountListSettingPZ.Add(new AccountListModel()
                        {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            Password = acc.Password._RSADecrypt(),
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            Balance = lastStock == null ? "" : lastStock.Close.ToString("n"),
                            Profit = lastStock == null ? "" : lastStock.Close < 100 ? (lastStock.Close - firstStock.Open).ToString("n") : string.Concat((lastStock.Close - firstStock.Open).ToString("n"), " (", ((lastStock.Close - firstStock.Open) / firstStock.Open).ToString("P2"), ")"),
                            ProfitForeground = lastStock == null ? "BLACK" : lastStock.Close - firstStock.Open > 0 ? "RED" : "GREEN",
                            //TypeForeground = "RED",
                            State1 = acc.Type % 10 == 0 ? "Visible" : "Collapsed",
                        });
                    }
                    else // 期货公司交易账户
                    {
                        accountListSettingJY.Add(new AccountListModel()
                        {
                            Id = acc.Id,
                            Type = acc.Type,
                            AccountNumber = acc.AccountNumber,
                            CustomerName = acc.CustomerName,
                            FuturesCompanyName = acc.FuturesCompanyName,
                            IsAllowLoad = acc.IsAllowLoad,
                            LatestDataDate = lastStock == null ? "" : lastStock.Date.ToShortDateString(),
                            Balance = lastStock == null ? "" : lastStock.Close.ToString("n"),
                            Profit = lastStock == null ? "" : lastStock.Close < 100 ? (lastStock.Close - firstStock.Open).ToString("n") : string.Concat((lastStock.Close - firstStock.Open).ToString("n"), " (", ((lastStock.Close - firstStock.Open) / firstStock.Open).ToString("P2"), ")"),
                            ProfitForeground = lastStock == null ? "BLACK" : lastStock.Close - firstStock.Open > 0 ? "RED" : "GREEN",
                            //TypeForeground = "BLACK",
                            State1 = acc.Type % 10 == 0 ? "Visible" : "Collapsed",
                            State2 = acc.IsAllowLoad ? "Hidden" : "Visible"
                        });
                    }
                }
                Dispatcher.Invoke(new FormControlInvoker(() =>
                {
                    mainWindow._combox账户列表_SelectionChanged_unlink();
                    mainWindow._combox账户列表.ItemsSource = accountListMain;
                    mainWindow._combox账户列表.SelectedItem = accountListMain.FirstOrDefault(m => m.Id == _Session.SelectedAccountId);
                    mainWindow._combox账户列表_SelectionChanged_link();
                    _listBox交易账户列表.ItemsSource = accountListSettingJY.OrderByDescending(m => m.LatestDataDate).ToList();
                    _listBox配资账户列表.ItemsSource = accountListSettingPZ.OrderByDescending(m => m.LatestDataDate).ToList();
                    if (accountListSettingJY.Count > 0)
                    {
                        _gb交易账号.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                    {
                        _gb交易账号.Visibility = System.Windows.Visibility.Collapsed;
                    }
                    //if (accountListSettingPZ.Count > 0)
                    //{
                    //    this._gb配资账户.Visibility = System.Windows.Visibility.Visible;
                    //}
                    //else
                    //{
                    //    this._gb配资账户.Visibility = System.Windows.Visibility.Collapsed;
                    //}
                }));
            }
        }

        public void Initialize(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            //
            InitializeAccountList();
        }
        private const string ALLOW_LOAD = "禁止更新";
        private const string NOT_ALLOW_LOAD = "允许更新";
        private const string ADD_COOPERATE = "修改账户";
        private const string DISUSE_ACCOUNT = "禁用账户";
        private const string RESUME_ACCOUNT = "使用账户";
        private void _button添加账户_Click(object sender, RoutedEventArgs e)
        {
            AddAccountWindow aaf = new AddAccountWindow();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(aaf)
                {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
            {
                using (StatementContext statement = new StatementContext())
                {
                    InitializeAccountList();
                }
            }
        }

        private void _button添加配资账户_Click(object sender, RoutedEventArgs e)
        {
            AddCooperateAccountDialog aaf = new AddCooperateAccountDialog();
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(aaf)
                {
                    Owner = winformWindow.Handle
                };
            }
            aaf.ShowDialog();
            //
            if (aaf.DialogResult.Value)
            {
                InitializeAccountList();
            }
        }

        private void _btn是否下载_Click(object sender, RoutedEventArgs e)
        {
        }

        private void _listBox交易账户列表_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (selectedAccountItem == null || (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10))
            {
                _menuItemJ列表隐藏.IsEnabled = false;
                _menuItemJ列表显示.IsEnabled = false;
                _menuItemJ允许下载.IsEnabled = false;
                _menuItemJ禁止下载.IsEnabled = false;
                return;
            }
            else
            {
                _menuItemJ列表隐藏.IsEnabled = true;
                _menuItemJ列表显示.IsEnabled = true;
                _menuItemJ允许下载.IsEnabled = true;
                _menuItemJ禁止下载.IsEnabled = true;
            }

            // 列表中不显示
            if (selectedAccountItem.Type % 10 == 0)
            {
                _menuItemJ列表隐藏.Visibility = System.Windows.Visibility.Collapsed;
                _menuItemJ列表显示.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _menuItemJ列表隐藏.Visibility = System.Windows.Visibility.Visible;
                _menuItemJ列表显示.Visibility = System.Windows.Visibility.Collapsed;
            }

            // 跟随下载
            if (selectedAccountItem.IsAllowLoad)
            {
                _menuItemJ允许下载.Visibility = System.Windows.Visibility.Collapsed;
                _menuItemJ禁止下载.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _menuItemJ允许下载.Visibility = System.Windows.Visibility.Visible;
                _menuItemJ禁止下载.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _listBox配资账户列表_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (selectedAccountItem == null || (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20))
            {
                _menuItemP列表隐藏.IsEnabled = false;
                _menuItemP列表显示.IsEnabled = false;
                _menuItemP修改账户.IsEnabled = false;
                _menuItemP查看数据.IsEnabled = false;
                return;
            }
            else
            {
                _menuItemP列表隐藏.IsEnabled = true;
                _menuItemP列表显示.IsEnabled = true;
                _menuItemP修改账户.IsEnabled = true;
                _menuItemP查看数据.IsEnabled = true;
            }

            // 列表中不显示
            if (selectedAccountItem.Type % 10 == 0)
            {
                _menuItemP列表隐藏.Visibility = System.Windows.Visibility.Collapsed;
                _menuItemP列表显示.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _menuItemP列表隐藏.Visibility = System.Windows.Visibility.Visible;
                _menuItemP列表显示.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void _menuItem列表显示_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext())
            {
                var account = statement.Account.ToList().FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type / 10;
                statement.SaveChanges();
                InitializeAccountList();
            }
        }

        private void _menuItem列表隐藏_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            using (StatementContext statement = new StatementContext())
            {
                var account = statement.Account.ToList().FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                if (account == null)
                    throw new ArgumentNullException("要修改的对象不存在！");

                account.Type = account.Type * 10;
                statement.SaveChanges();
                InitializeAccountList();
            }
        }

        private void _menuItem修改账户_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            EditCooperateAccountDialog acdd = new EditCooperateAccountDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(acdd)
                {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
            InitializeAccountList();
        }

        private void _menuItem允许下载_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext())
            {
                Account acc = context.Account.ToList().FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.SaveChanges();
                if (!acc.IsAllowLoad)
                {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        private void _menuItem禁止下载_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 1 && selectedAccountItem.Type != 10)
                return;
            using (StatementContext context = new StatementContext())
            {
                Account acc = context.Account.ToList().FirstOrDefault(m => m.Id == selectedAccountItem.Id);
                acc.IsAllowLoad = !acc.IsAllowLoad;
                context.SaveChanges();
                if (!acc.IsAllowLoad)
                {
                    MessageBox.Show("此账户将在超过60天未更新时，下载一次。");
                }
                InitializeAccountList();
            }
        }

        AccountListModel selectedAccountItem = null;

        private void _listBox配资账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = _listBox配资账户列表.SelectedItem;
            if (item == null)
                selectedAccountItem = null;
            else
                selectedAccountItem = item as AccountListModel;
        }

        private void _listBox交易账户列表_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = _listBox交易账户列表.SelectedItem;
            if (item == null)
                selectedAccountItem = null;
            else
                selectedAccountItem = item as AccountListModel;
        }

        private void _menuItemP查看数据_Click(object sender, RoutedEventArgs e)
        {
            if (selectedAccountItem == null)
                return;
            if (selectedAccountItem.Type != 2 && selectedAccountItem.Type != 20)
                return;
            ImportCooperateDataDialog acdd = new ImportCooperateDataDialog(selectedAccountItem.Id);
            HwndSource winformWindow = (HwndSource.FromDependencyObject(this) as HwndSource);
            if (winformWindow != null)
            {
                new WindowInteropHelper(acdd)
                {
                    Owner = winformWindow.Handle
                };
            }
            acdd.ShowDialog();
        }

        private void _menuItemP自动填充数据_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
