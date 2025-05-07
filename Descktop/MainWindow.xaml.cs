using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private IntPtr _shellViewPtr = IntPtr.Zero;
        private ShellContextMenu _ctxMnu = new();
        private DesktopWindow _desktopWindow = new();
        public MainWindowViewModel ViewModel = new MainWindowViewModel();
        //private FileChangeWatcher _fileChangeWatcher;
        //private UIHelper _uiHelper = new();
        private FileSystemWatcher _fileSystemWatcher = new();
        public MainWindow()
        {
            InitializeComponent();

           // this.DataContext = ViewModel;
            
            
            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //_fileChangeWatcher = new FileChangeWatcher(new WindowInteropHelper(this).Handle, desktopPath);
            //_fileChangeWatcher.FileCreated += OnFileCreated;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            // 设置要监听的文件夹路径
            _fileSystemWatcher.Path = desktopPath;

            // 监听所有文件类型的更改
            _fileSystemWatcher.Filter = "*.*";

            // 监听文件的创建、删除、更改和重命名事件
            //_fileSystemWatcher.Created += OnChanged;
            //_fileSystemWatcher.Deleted += OnChanged;
            //_fileSystemWatcher.Changed += OnChanged;
            //_fileSystemWatcher.Renamed += OnRenamed;

            // 启用监听
            _fileSystemWatcher.EnableRaisingEvents = true;
            //_fileSystemWatcherher.EnableRaisingEvents = true;

            Closed += (sender, e) => _fileSystemWatcher.Dispose();
            //SystemParameters.PrimaryScreenHeight 包含任务栏高度 
            //SystemParameters.PrimaryScreenWidth  
            //Width = SystemParameters.WorkArea.Width;
            //Height = SystemParameters.WorkArea.Height;

            //Activated += OnActivated;
        }

        private void OnActivated(object sender, EventArgs e)
        {
            _desktopWindow.UpdateDesktopWindow(new WindowInteropHelper(this).Handle);
        }

        private void Init()
        {

        }


        //监听桌面路径



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //git:ShellContextMenu
            //string path = "C:\\Users\\10475\\Desktop\\";
            ////ShellContextMenu ctxMnu = new();
            //DirectoryInfo[] arrFI = [new DirectoryInfo(path)];
            //_ctxMnu.ShowContextMenu(arrFI, new System.Drawing.Point(100, 100));

            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //FileInfo[] files = new FileInfo[1];
            //files[0] = new FileInfo(desktopPath);

            _ctxMnu.ShowDesktopContextMenu(new System.Drawing.Point(100, 100));
        }

        //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //const int WM_SHNOTIFY = 0x0401;
        //const uint SHCNE_CREATE = 0x00000004;

        //if (msg == WM_SHNOTIFY)
        //{
        //    uint eventCode = (uint)wParam.ToInt32();
        //    if (eventCode == SHCNE_CREATE)
        //    {
        //        _fileChangeWatcher.HandleFileChangeMessage(lParam);
        //        handled = true;
        //    }
        //}

        //return IntPtr.Zero;
        //   }

     

    }
}