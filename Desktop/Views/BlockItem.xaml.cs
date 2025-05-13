using Desktop.Events;
using Desktop.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using UserControl = System.Windows.Controls.UserControl;

namespace Desktop.Views
{
    /// <summary>
    /// BlockItem.xaml 的交互逻辑
    /// </summary>
    public partial class BlockItem : UserControl
    {
        public BlockItemViewModel ViewModel;

        public BlockItem()
        {
            InitializeComponent();
        }

        public BlockItem(BlockItemViewModel viewModel) : this()
        {
            ViewModel = viewModel;
            this.DataContext = ViewModel;
            this.MenuContextMenu.DataContext = this.DataContext;
        }

        private void TopThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newHeight = this.Height - e.VerticalChange;
            if (newHeight > MinHeight)
            {
                this.Height = newHeight;
                this.Margin = new Thickness(Margin.Left, Margin.Top + e.VerticalChange, Margin.Right, Margin.Bottom);
            }
        }

        // 下
        private void BottomThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newHeight = this.Height + e.VerticalChange;
            if (newHeight > MinHeight)
                this.Height = newHeight;

        }

        // 左
        private void LeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = this.Width - e.HorizontalChange;
            if (newWidth > MinWidth)
            {
                this.Width = newWidth;
                this.Margin = new Thickness(Margin.Left + e.HorizontalChange, Margin.Top, Margin.Right, Margin.Bottom);
            }

        }

        // 右
        private void RightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            double newWidth = this.Width + e.HorizontalChange;
            if (newWidth > MinWidth)
                this.Width = newWidth;

        }

        // 左上
        private void TopLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            TopThumb_DragDelta(sender, e);
            LeftThumb_DragDelta(sender, e);
        }

        // 右上
        private void TopRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            TopThumb_DragDelta(sender, e);
            RightThumb_DragDelta(sender, e);
        }

        // 左下
        private void BottomLeftThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            BottomThumb_DragDelta(sender, e);
            LeftThumb_DragDelta(sender, e);
        }

        // 右下
        private void BottomRightThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            BottomThumb_DragDelta(sender, e);
            RightThumb_DragDelta(sender, e);
        }

        private void Thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ViewModel.SetBlockSize(this.Width,this.Height);
            //this.log.Text += $"拖动完成：高度：{this.Height},宽度：{this.Width}\r\n";
        }

    }
}
