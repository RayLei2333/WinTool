using Desktop.Commands;
using Desktop.Events;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using System.Windows.Controls;

namespace Desktop.Views
{
    public class BlockItemViewModel : BaseViewModel
    {
        #region Local variabled
        private bool _isDragging = false;
        private Point _clickPosition;
        private Thickness _originalMargin;
        #endregion

        #region Poperties
        public BlockData Data { get; set; }

        private Thickness _blockItemMargin;
        /// <summary>
        /// Block Margin
        /// </summary>
        public Thickness BlockItemMargin
        {
            get { return _blockItemMargin; }
            set
            {
                _blockItemMargin = value;
                OnPropertyChanged("BlockItemMargin");
            }
        }

        private bool _isEditName;
        /// <summary>
        /// 是否在编辑名称状态
        /// </summary>
        public bool IsEditName
        {
            get { return _isEditName; }
            set
            {
                _isEditName = value;
                OnPropertyChanged("IsEditName");
            }
        }


        public double MaxHeight
        {
            get { return SystemParameters.WorkArea.Height; }

        }


        /// <summary>
        /// 是否锁定状态
        /// </summary>
        public bool Lock
        {
            get { return Data.Lock; }
            set
            {
                Data.Lock = value;
                OnPropertyChanged("Lock");
            }
        }

        /// <summary>
        /// Block名称
        /// </summary>
        public string Name
        {
            get { return Data.Name; }
            set
            {
                Data.Name = value;
                OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// 视图类型
        /// </summary>
        public int ViewType
        {
            get { return (int)Data.ViewType; }
            set
            {
                Data.ViewType = (ViewType)value;
                OnPropertyChanged("ViewType");
            }
        }
        #endregion

        #region Command
        /// <summary>
        /// 标题栏鼠标按下命令
        /// </summary>
        public ICommand TitleMouseDownCommand { get; set; }

        /// <summary>
        /// 标题栏鼠标移动命令
        /// </summary>
        public ICommand TitleMouseMoveCommand { get; set; }

        /// <summary>
        /// 标题栏鼠标抬起命令
        /// </summary>
        public ICommand TitleMouseUpCommand { get; set; }

        /// <summary>
        /// 标题双击命令
        /// </summary>
        public ICommand TitleDoubleClickCommand { get; set; }

        public ICommand TitleKeyUpCommand { get; set; }

        /// <summary>
        /// 锁定按钮点击命令
        /// </summary>
        public ICommand LockButtonClickCommand { get; set; }

        /// <summary>
        /// 视图类型点击命令
        /// </summary>
        public ICommand ViewTypeClickCommand { get; set; }

        /// <summary>
        /// 视图类型菜单项点击命令
        /// </summary>
        public ICommand ViewTypeItemClickCommand { get; set; }
        #endregion

        #region Events
        public event JustSaveEventHandler JustSaveEvent;
        #endregion

        public BlockItemViewModel(BlockData data)
        {
            Data = data;
            BlockItemMargin = new Thickness(Data.X, Data.Y, 0, 0);
            TitleDoubleClickCommand = new ReplayCommand<MouseButtonEventArgs>(TitleDoubleClick);
            TitleKeyUpCommand = new ReplayCommand<KeyEventArgs>(TitleKeyUpown);
            TitleMouseDownCommand = new ReplayCommand<MouseButtonEventArgs>(TitleMouseDown);
            TitleMouseMoveCommand = new ReplayCommand<MouseEventArgs>(TitleMouseMove);
            TitleMouseUpCommand = new ReplayCommand<MouseButtonEventArgs>(TitleMouseUp);
            LockButtonClickCommand = new ReplayCommand<object>(LockButtonClick);
            ViewTypeClickCommand = new ReplayCommand<RoutedEventArgs>(ViewTypeClick);
            ViewTypeItemClickCommand = new ReplayCommand<RoutedEventArgs>(ViewTypeItemClick);
        }

        #region Command Events
        /// <summary>
        /// 标题栏鼠标按下事件
        /// </summary>
        /// <param name="e"></param>
        private void TitleMouseDown(MouseButtonEventArgs e)
        {
            if (!Lock && e.LeftButton == MouseButtonState.Pressed)
            {
                _isDragging = true;
                _clickPosition = e.GetPosition(null); // 相对于整个窗口
                var root = ElementExtension.GetRootElement<BlockItem>(e.Source as UIElement);
                _originalMargin = root.Margin;
            }
        }

        /// <summary>
        /// 标题栏鼠标移动事件
        /// </summary>
        /// <param name="e"></param>
        private void TitleMouseMove(MouseEventArgs e)
        {
            // 判断是否按下了鼠标左键
            if (!Lock && _isDragging)
            {
                System.Windows.Point currentPosition = e.GetPosition(null);
                Vector offset = currentPosition - _clickPosition;
                Data.X = (int)(_originalMargin.Left + offset.X);
                Data.Y = (int)(_originalMargin.Top + offset.Y);
                BlockItemMargin = new Thickness(Data.X, Data.Y, 0, 0);
            }
        }

        /// <summary>
        /// 标题栏鼠标抬起事件
        /// </summary>
        /// <param name="e"></param>
        private void TitleMouseUp(MouseButtonEventArgs e)
        {
            if (Lock)
                return;
            _isDragging = false;
            JustSaveEvent?.Invoke(this, new JustSaveEventArgs());
        }

        /// <summary>
        /// 顶部标题双击事件
        /// </summary>
        /// <param name="sender"></param>
        private void TitleDoubleClick(MouseButtonEventArgs args)
        {
            IsEditName = true;
        }

        private void TitleKeyUpown(KeyEventArgs args)
        {
            if (args.Key == Key.Enter)
            {
                IsEditName = false;
                JustSaveEvent?.Invoke(this, new JustSaveEventArgs());
            }
        }

        /// <summary>
        /// 锁定按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        private void LockButtonClick(object sender)
        {
            Lock = !Data.Lock;
            JustSaveEvent?.Invoke(this, new JustSaveEventArgs());
        }

        /// <summary>
        /// 视图类型点击事件
        /// </summary>
        private void ViewTypeClick(RoutedEventArgs e)
        {
            var button = e.Source as System.Windows.Controls.Button;
            if (button == null || button.ContextMenu == null)
                return;
            var contextMenu = button.ContextMenu;
            contextMenu.IsOpen = !contextMenu.IsOpen;
        }

        /// <summary>
        /// 视图项目点击事件
        /// </summary>
        /// <param name="e"></param>
        private void ViewTypeItemClick(RoutedEventArgs e)
        {
            var menuItem = e.Source as MenuItem;
            if (menuItem == null || menuItem.Tag == null)
                return;
            //ViewType = Convert.ToInt32(menuItem.Tag);
            int viewType = Convert.ToInt32(menuItem.Tag);
            if (viewType == ViewType)
                return;
            ViewType = viewType;
            JustSaveEvent?.Invoke(this, new JustSaveEventArgs());

        }
        #endregion

        public void SetBlockSize(double width, double height)
        {
            Data.Width = width;
            Data.Height = height;
            JustSaveEvent?.Invoke(this, new JustSaveEventArgs());
        }
    }
}
