using FuturesAssistant.Models;
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
    /// CommodityDialog.xaml 的交互逻辑
    /// </summary>
    public partial class CommodityDialog : DialogBase
    {
        public CommodityDialog()
        {
            InitializeComponent();
            //
            _listBox品种列表.ItemsSource = new StatementContext().Commodity.ToList();
        }
        private void _btn扫描品种_Click(object sender, RoutedEventArgs e)
        {
            using (StatementContext statement = new StatementContext())
            {
                List<Commodity> commodityList = new List<Commodity>();
                var commoditys = statement.Commodity;
                var css = statement.CommoditySummarization;
                int count = 0;
                foreach (var cs in css)
                {
                    if (commoditys.FirstOrDefault(m => m.Code.Equals(cs.Commodity)) == null
                        && commodityList.FirstOrDefault(m => m.Code.Equals(cs.Commodity)) == null)
                    {
                        Commodity cd = new Commodity();
                        cd.Code = cs.Commodity;
                        cd.Name = cs.Commodity;
                        commodityList.Add(cd);
                        count++;
                    }
                }
                statement.Commodity.AddRange(commodityList);
                statement.SaveChanges();

                _listBox品种列表.ItemsSource = null;
                _listBox品种列表.ItemsSource = new StatementContext().Commodity.ToList();
                MessageBox.Show(string.Concat("扫描完成！新增", count, "个品种。"));
            }
        }

        private void _btn品种更名_Click(object sender, RoutedEventArgs e)
        {
            if (_listBox品种列表.SelectedItem == null)
            {
                MessageBox.Show("请先选择需要修改的品种。");
            }
            using (StatementContext context = new StatementContext())
            {
                Commodity comm = _listBox品种列表.SelectedItem as Commodity;
                ModifyCommodity mc = new ModifyCommodity(comm);
                mc.ShowDialog();
                _listBox品种列表.ItemsSource = null;
                _listBox品种列表.ItemsSource = new StatementContext().Commodity.ToList();
            }
        }
    }
}
