using Desktop.Events;
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

    }
}
