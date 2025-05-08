using Desktop.Views;
using System.IO;
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
        private ShellContextMenu _ctxMnu = new();
        private DesktopWindow _desktopWindow = new();
        private FileSystemWatcher _fileSystemWatcher = new();
        public MainWindowViewModel ViewModel = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;
            //init block
            foreach (var item in ViewModel.Blocks)
            {
                BlockItem block = new BlockItem(item, ViewModel);
                wrapper.Children.Add(block);
            }


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
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            //Activated += OnActivated;


        }

        private void OnActivated(object sender, EventArgs e)
        {
            _desktopWindow.UpdateDesktopWindow(new WindowInteropHelper(this).Handle);
        }


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
    }
}