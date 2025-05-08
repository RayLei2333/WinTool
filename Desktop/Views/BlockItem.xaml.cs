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
        //public BlockData BlockData { get; set; }
        private bool isDragging = false;
        private System.Windows.Point clickPosition;
        private Thickness originalMargin;

        public BlockItem(BlockData blockData, MainWindowViewModel mainWindowViewModel)
        {
            InitializeComponent();

            ViewModel = new BlockItemViewModel(blockData, mainWindowViewModel);
            this.DataContext = ViewModel;
        }

        private void title_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isDragging = true;
                clickPosition = e.GetPosition(null); // 相对于整个窗口
                originalMargin = this.Margin;
                //this.CaptureMouse();
            }
        }

        private void title_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // 判断是否按下了鼠标左键
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(null);
                Vector offset = currentPosition - clickPosition;

                ViewModel.SetMargin((int)(originalMargin.Left + offset.X), (int)(originalMargin.Top + offset.Y));
            }
        }

        private void title_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // 释放鼠标捕获
            //this.ReleaseMouseCapture();
            isDragging = false;
            //ViewModel.SetMargin((int)this.Margin.Left, (int)this.Margin.Top);
            ViewModel.Save();
            //this.ReleaseMouseCapture();
        }
    }
}
