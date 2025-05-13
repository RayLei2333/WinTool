using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Desktop.Events;
using Desktop.Manager;
using Desktop.Win32Support;

namespace Desktop.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Porperty
        private Brush _backgroundImage;
        public Brush BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundImage"));
            }
        }

        private BlockManager _blockManager = BlockManager.Instence;

        public List<BlockData> Blocks { get { return _blockManager.BlockData; } }

        #endregion


        public MainWindowViewModel()
        {
            //设置壁纸
            string wallPaperPath = DesktopWindow.GetDesktopWallpaperPath();
            if (string.IsNullOrEmpty(wallPaperPath))
                BackgroundImage = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            else
                BackgroundImage = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(wallPaperPath))
                };


            //填充默认块
            if (!Blocks.Any())
            {
                _blockManager.AddBlock(100, "测试块1");
            }
        }

        #region 订阅BlockItem事件

        public void JustSaveEvent(object sender, JustSaveEventArgs e)
        {
            // 调整BlockItem位置
            _blockManager.Save();
        }
        #endregion
    }
}
