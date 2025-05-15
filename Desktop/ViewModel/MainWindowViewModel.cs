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
using Desktop.Models;
using Desktop.Win32Support.Models;

namespace Desktop.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Porperty
        private Brush _backgroundImage;
        public Brush BackgroundImage
        {
            get { return _backgroundImage; }
            set { _backgroundImage = value; OnPropertyChanged(nameof(BackgroundImage)); }
        }

        private ObservableCollection<DesktopFileData> _desktopFile;
        public ObservableCollection<DesktopFileData> DesktopFile
        {
            get { return _desktopFile; }
            set { _desktopFile = value; OnPropertyChanged(nameof(DesktopFile)); }
        }

        public int IconWidth { get { return DesktopManager.Instence.DesktopData.IconWidth; } }

        public int IconHeight { get { return DesktopManager.Instence.DesktopData.IconHeight; } }

        private BlockManager _blockManager = BlockManager.Instence;

        public List<BlockData> Blocks { get { return _blockManager.BlockData; } }

        #endregion


        public MainWindowViewModel()
        {
            DesktopFile = new ObservableCollection<DesktopFileData>();
            //设置壁纸
            SetBackground();
            foreach (var item in DesktopManager.Instence.DesktopData.FileData)
            {
                DesktopFile.Add(item);
            }

            //填充默认块
            if (!Blocks.Any())
            {
                _blockManager.AddBlock(100, "测试块1");
            }
        }

        private void SetBackground()
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
