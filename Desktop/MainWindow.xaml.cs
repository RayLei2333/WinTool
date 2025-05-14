using Desktop.Events;
using Desktop.Manager;
using Desktop.ViewModel;
using Desktop.Views;
using Desktop.Win32Support;
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
            //SystemParameters.PrimaryScreenHeight 包含任务栏高度 
            //SystemParameters.PrimaryScreenWidth  
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            this.DataContext = ViewModel;
            DesktopManager.Instence.GetAllDesktopFile();

            #region Test Data
            List<string> tmpFilePaths = new List<string>()
            {
                //@"C:\Users\23162\Desktop\WarningLight.cs",
                //@"C:\Users\23162\Desktop\清晰度检测.cs",
                @"C:\Users\23162\Desktop\test.bmp",
                //@"C:\Users\23162\Desktop\Config-EFEM",
                //@"C:\Users\23162\Desktop\doc.lnk"
                //@"C:\Users\10475\Desktop\wb.json",
                //@"C:\Users\10475\Desktop\Visual Studio 2022.lnk",
                //@"C:\Users\10475\Desktop\Visual Studio Code.lnk"
            };
            ViewModel.Blocks[0].FilePathList = tmpFilePaths;
            BlockManager.Instence.ResetFileList();
            var fileList = ViewModel.Blocks.SelectMany(t => t.FileList).ToList();
            IconManager.Instence.SetIcon(fileList);
            #endregion

            //init block
            foreach (var item in ViewModel.Blocks)
            {
                var vm = new BlockItemViewModel(item);
                vm.JustSaveEvent += ViewModel.JustSaveEvent;
                BlockItem block = new BlockItem(vm);

                wrapper.Children.Add(block);
            }

            //获取桌面文件夹中的所有文件的图标

            //DragSizeControl dragSizeControl = new DragSizeControl();
            //wrapper.Children.Add(dragSizeControl);


            //string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //// 设置要监听的文件夹路径
            //_fileSystemWatcher.Path = desktopPath;

            //// 监听所有文件类型的更改
            //_fileSystemWatcher.Filter = "*.*";

            //// 监听文件的创建、删除、更改和重命名事件
            ////_fileSystemWatcher.Created += OnChanged;
            ////_fileSystemWatcher.Deleted += OnChanged;
            ////_fileSystemWatcher.Changed += OnChanged;
            ////_fileSystemWatcher.Renamed += OnRenamed;

            //// 启用监听
            //_fileSystemWatcher.EnableRaisingEvents = true;
            ////_fileSystemWatcherher.EnableRaisingEvents = true;

            //Closed += (sender, e) => _fileSystemWatcher.Dispose();



            //Activated += OnActivated;


        }

        private void OnActivated(object sender, EventArgs e)
        {
            _desktopWindow.UpdateDesktopWindow(new WindowInteropHelper(this).Handle);
        }

    }
}