using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Desktop
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        #region Event
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Porperty
        private System.Windows.Media.Brush _backgroundImage;

        public System.Windows.Media.Brush BackgroundImage
        {
            get { return _backgroundImage; }
            set
            {
                _backgroundImage = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BackgroundImage"));
            }
        }

        #endregion


        public MainWindowViewModel()
        {
            string wallPaperPath = DesktopWindow.GetDesktopWallpaperPath();
            if (string.IsNullOrEmpty(wallPaperPath))
                BackgroundImage = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            else
                BackgroundImage = new ImageBrush()
                {
                    ImageSource = new BitmapImage(new Uri(wallPaperPath))
                };
        }
    }
}
