using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        private ShellContextMenu _ctxMnu = new();
        private FileSystemWatcher _fileSystemWatcher = new();

        public TestWindow()
        {
            InitializeComponent();
            _fileSystemWatcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            _fileSystemWatcher.Filter = "*.*";
            _fileSystemWatcher.Error += OnError;
            _fileSystemWatcher.Created += OnChanged;
            _fileSystemWatcher.Deleted += OnChanged;
            _fileSystemWatcher.Changed += OnChanged;
            _fileSystemWatcher.Renamed += OnRenamed;

        }

        private void fileRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string testFilePath = @"C:\Users\10475\Desktop\wb.json";
            FileInfo[] files = new FileInfo[1] { new FileInfo(testFilePath) };
            _ctxMnu.ShowContextMenu(files, GetMousePosition());
        }

        private void folderRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = @"C:\Users\10475\Desktop\PSD";
            DirectoryInfo[] arrFI = [new DirectoryInfo(path)];
            _ctxMnu.ShowContextMenu(arrFI, GetMousePosition());
        }

        private void desktopRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            _ctxMnu.ShowDesktopContextMenu(GetMousePosition());
        }

        private System.Drawing.Point GetMousePosition()
        {
            return System.Windows.Forms.Cursor.Position;
        }

        
        private void folderWatchBtn_Click(object sender, RoutedEventArgs e)
        {
          
            _fileSystemWatcher.EnableRaisingEvents = !_fileSystemWatcher.EnableRaisingEvents;
            if (_fileSystemWatcher.EnableRaisingEvents)
            {
                folderWatchBtn.Content = "停止监听";
            }
            else
            {
                folderWatchBtn.Content = "开始监听";
            }
        }

        private void OnError(object sender,ErrorEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.log.Text += e.GetException().Message + "\r\n";
                //MessageBox.Show(e.GetException().Message);
            });
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                this.log.Text += $"类型：{e.ChangeType}，名称：{e.Name},路径：{e.FullPath}" + "\r\n";
            });
            //
           //e.ChangeType
        }
        private void OnRenamed(object sender, RenamedEventArgs e)
        {
           this.Dispatcher.Invoke(() =>
            {
                this.log.Text += $"类型：{e.ChangeType}，名称：{e.Name},路径：{e.FullPath}，原名称：{e.OldName},原路径：{e.OldFullPath}" + "\r\n";
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if(_fileSystemWatcher != null)
                _fileSystemWatcher.Dispose();
        }

        private void folderIconBtn_Click(object sender, RoutedEventArgs e)
        {
           
            string testFloderPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
            var icon = IconExtractor.GetIcon48(testFloderPath, true);
            var bmp = icon.ToBitmap();
            bmp.Save("C:\\Users\\10475\\Desktop\\test\\fileicon.png");
        }

        private void fileLnkIconBtn_Click(object sender, RoutedEventArgs e)
        {
            string testFilePath = @"C:\Users\10475\Desktop\Visual Studio 2022.lnk";
            var icon = IconExtractor.GetIcon32(testFilePath, false);
            var bmp = icon.ToBitmap();
            bmp.Save("C:\\Users\\10475\\Desktop\\test\\fileicon_lnk.png");
        }
    }
}
