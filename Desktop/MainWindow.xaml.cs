using Desktop.Events;
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
            //SystemParameters.PrimaryScreenHeight 包含任务栏高度 
            //SystemParameters.PrimaryScreenWidth  
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;
            this.DataContext = ViewModel;
            //init block
            foreach (var item in ViewModel.Blocks)
            {
                var vm = new BlockItemViewModel(item);
                vm.JustSaveEvent += ViewModel.JustSaveEvent;
                BlockItem block = new BlockItem(vm);

                wrapper.Children.Add(block);
            }


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