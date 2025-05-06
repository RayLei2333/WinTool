using System.IO;
using System.Net.NetworkInformation;
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
        //private UIHelper _uiHelper = new();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = ViewModel;

            //SystemParameters.PrimaryScreenHeight 包含任务栏高度 
            //SystemParameters.PrimaryScreenWidth  
            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            Activated += OnActivated;
        }

        private void OnActivated(object? sender, EventArgs e)
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

            _ctxMnu.ShowDesktopContextMenu(new System.Drawing.Point(100,100));


        }
    }
}