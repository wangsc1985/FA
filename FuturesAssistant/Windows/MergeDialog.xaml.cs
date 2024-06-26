using FuturesAssistant.FAException;
using FuturesAssistant.Helpers;
using FuturesAssistant.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
    /// AddAccountWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MergeDialog : DialogBase
    {
        public MergeDialog()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _button删除.IsEnabled = false;
            _button上移.IsEnabled = false;
            _button下移.IsEnabled = false;
        }

        private void _button添加_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog ofd = new System.Windows.Forms.OpenFileDialog();
            ofd.Filter = "图片(*.png)|*.png|图片(*.jpg)|*.jpg";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var files = ofd.FileNames;
                foreach (var file in files)
                {
                    _listBoxPics.Items.Add(file);
                }
            }
        }

        private void _button删除_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var pics = _listBoxPics.SelectedItems;
                foreach (var pi in pics)
                {
                    _listBoxPics.Items.Remove(pi);
                }
            }
            catch (Exception)
            {
            }
        }

        private void _button上移_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = _listBoxPics.SelectedItem;
            var selectIndex = _listBoxPics.SelectedIndex;
            if (selectIndex != 0)
            {
                _listBoxPics.Items.Remove(selectItem);
                _listBoxPics.Items.Insert(selectIndex - 1, selectItem);
            }
        }

        private void _button下移_Click(object sender, RoutedEventArgs e)
        {
            var selectItem = _listBoxPics.SelectedItem;
            var selectIndex = _listBoxPics.SelectedIndex;
            if (selectIndex != _listBoxPics.Items.Count - 1)
            {
                _listBoxPics.Items.Remove(selectItem);
                _listBoxPics.Items.Insert(selectIndex + 1, selectItem);
            }
        }

        private void _button合并_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_listBoxPics.Items.Count < 2)
                {
                    MessageBox.Show("至少有两张图片！");
                    return;
                }
                var diaryImages = _listBoxPics.Items;
                System.Drawing.Bitmap tempImg = new System.Drawing.Bitmap(1, 10);
                foreach (var diaryImage in diaryImages)
                {
                    Stream imgStream = File.OpenRead(diaryImage.ToString());
                    System.Drawing.Image img = System.Drawing.Image.FromStream(imgStream);
                    imgStream.Close();

                    //
                    int oldWidth = tempImg.Width;
                    int oldHeight = tempImg.Height;
                    int newWidth = img.Width;
                    int newHeight = img.Height;
                    int width = oldWidth >= newWidth ? oldWidth : newWidth;
                    int height = oldHeight + newHeight + 35;

                    System.Drawing.Bitmap bm = new System.Drawing.Bitmap(width, height);
                    System.Drawing.Graphics gra = System.Drawing.Graphics.FromImage(bm);
                    gra.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                    gra.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                    gra.DrawImage(tempImg, 0, 0);
                    gra.DrawString(System.IO.Path.GetFileNameWithoutExtension(diaryImage.ToString()), new System.Drawing.Font("微软雅黑", 15), new System.Drawing.SolidBrush(System.Drawing.Color.Black), 0, oldHeight);
                    gra.DrawImage(img, 0, oldHeight + 25);
                    tempImg = bm;
                }
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();
                sfd.Filter = "图片(*.png)|*.png|图片(*.jpg)|*.jpg";
                sfd.FileName = System.IO.Path.GetFileNameWithoutExtension(_listBoxPics.Items[0].ToString());
                sfd.ShowDialog();
                tempImg.Save(sfd.FileName);
                MessageBox.Show("合并成功！");
                _listBoxPics.Items.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DockPanel_DragOver(object sender, DragEventArgs e)
        {
            string[] filename = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filename != null)
            {
                foreach (var fi in filename)
                {
                    var extension = System.IO.Path.GetExtension(fi);
                    if (extension.Equals(".png") || extension.Equals(".jpg"))
                    {
                        _listBoxPics.Items.Add(fi);
                    }
                    else
                    {
                        MessageBox.Show(string.Format("{0}拖入失败！仅支持(PNG,JPG)格式文件！", System.IO.Path.GetFileName(fi)));
                    }
                }
            }
        }

        private void _listBoxPics_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_listBoxPics.SelectedItems.Count > 0)
            {
                _button删除.IsEnabled = true;
                _button上移.IsEnabled = true;
                _button下移.IsEnabled = true;
            }
            else
            {
                _button删除.IsEnabled = false;
                _button上移.IsEnabled = false;
                _button下移.IsEnabled = false;
            }
        }

        private void _listBoxPics_Drop(object sender, DragEventArgs e)
        {

        }

    }
}
