using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using Desktop.Win32Support;
using static Desktop.Win32Support.ImageFileThumbnail;

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
            var point = Mouse.GetPosition(null);
            return new System.Drawing.Point((int)point.X,(int)point.Y);
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

        private void OnError(object sender, ErrorEventArgs e)
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
            if (_fileSystemWatcher != null)
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

        private void thumbnailBtn_Click(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\23162\Desktop\企业微信截图_17466665218601.png";
            Guid guid = typeof(IShellItemImageFactory).GUID;
            //SHCreateItemFromParsingName(filePath, IntPtr.Zero, ref guid, out IShellItemImageFactory imageFactory);
            SHCreateItemFromParsingName(filePath, IntPtr.Zero, ref guid, out IShellItemImageFactory imageFactory);
            SIZE size;
            size.cx = 256;
            size.cy = 256;
            //异常System.Runtime.InteropServices.COMException:“0x8004B200”

            imageFactory.GetImage(size, SIIGBF.SIIGBF_RESIZETOFIT | SIIGBF.SIIGBF_THUMBNAILONLY, out IntPtr hBitmap);

            using (Bitmap bmp = System.Drawing.Image.FromHbitmap(hBitmap))
            {
                bmp.Save(@"C:\Users\23162\Desktop\test\thumbnail2.png");
                //Console.WriteLine("High-quality thumbnail saved.");
            }

            // 清理
            DeleteObject(hBitmap);
        }
        private void desktopIconSizelBtn_Click(object sender, RoutedEventArgs e)
        {
            //int iconWidth = (int)GetSystemMetrics(SM_CXICON);
            //int iconHeight = (int)GetSystemMetrics(SM_CYICON);
            //int iconWidth = 0;
            //int iconHeight = 0;

            //this.log.Text += $"桌面图标宽度: {iconWidth}, 桌面图标高度: {iconHeight}\r\n";
        }



       


        private void pailieBtn_Click(object sender, RoutedEventArgs e)
        {
            var desktopViewModel = DesktopWindow.GetDesktopIconView();
            AppendToLog($"自动排列图标: {desktopViewModel.AutoArrange}");
            AppendToLog($"与网格对齐: {desktopViewModel.AlignedToGrid}");
            AppendToLog($"图标显示大小: {desktopViewModel.X},{desktopViewModel.Y}");

            //// 获取桌面窗口句柄
            //IntPtr hwndDesktop = DesktopWindow.FindDesktopWindow();//FindWindow(null, desktopFolderClass);

            //// 获取桌面文件夹视图句柄
            //IntPtr hwndFolderView = IntPtr.Zero;
            //while (hwndDesktop != IntPtr.Zero)
            //{
            //    hwndFolderView = FindWindowEx(hwndDesktop, IntPtr.Zero, "SysListView32", null);
            //    if (hwndFolderView != IntPtr.Zero)
            //    {
            //        break;
            //    }
            //    hwndDesktop = FindWindowEx(IntPtr.Zero, hwndDesktop, null, null);
            //}

            //if (hwndFolderView == IntPtr.Zero)
            //{
            //    AppendToLog("无法获取桌面文件夹视图句柄。");
            //    return;
            //}

            //// 获取桌面文件夹视图的扩展样式
            //IntPtr extendedStyle = SendMessage(hwndFolderView, LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero);

            //// 判断自动排列图标状态
            //bool isAutoArrange = ((extendedStyle.ToInt32() & LVS_AUTOARRANGE) == LVS_AUTOARRANGE);

            //// 判断与网格对齐状态
            //bool isAlignedToGrid = ((extendedStyle.ToInt32() & LVS_SNAPTOGRID) == LVS_SNAPTOGRID);

            //AppendToLog($"自动排列图标: {isAutoArrange}");
            //AppendToLog($"与网格对齐: {isAlignedToGrid}");

            //Console.WriteLine($"自动排列图标: {isAutoArrange}");
            //Console.WriteLine($"与网格对齐: {isAlignedToGrid}");
        }


        public void AppendToLog(string msg)
        {
            this.log.Text += msg+"\r\n";
        }

        private void allImageFormatBtn_Click(object sender, RoutedEventArgs e)
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in imageEncoders)
            {
                AppendToLog("Filename Extension: " + encoder.FilenameExtension);
                AppendToLog("Format Description: " + encoder.FormatDescription);
                AppendToLog("MIME Type: " + encoder.MimeType);
                AppendToLog("-------------------------------------------------");
            }

        }
    }
}
