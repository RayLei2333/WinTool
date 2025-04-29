using System.Drawing;
using System.Reflection.Metadata;
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

namespace test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            //InitializeComponent();
        }

        //[DllImport("user32.dll", SetLastError = true)]
        //private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        //private const UInt32 SWP_NOSIZE = 0x0001;
        //private const UInt32 SWP_NOMOVE = 0x0002;
        //private const UInt32 SWP_NOACTIVATE = 0x0010;
        //private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        //[DllImport("user32.dll", EntryPoint = "SetParent")]
        //public static extern int SetParent(int hWndChild, int hWndNewParent);

        //[DllImport("user32.dll", EntryPoint = "FindWindow")]
        //public static extern int FindWindow(string lpClassName, string lpWindowName);

        //[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr FindWindowEx(IntPtr hP, IntPtr hC, string sC, string sW);

        //[System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        //public MainWindow()
        //{
        //    //IntPtr nWinHandle = FindWindowEx(IntPtr.Zero, IntPtr.Zero, "Progman", null);
        //    //nWinHandle = FindWindowEx(nWinHandle, IntPtr.Zero, "SHELLDLL_DefView", null);
        //    //SetParent(new WindowInteropHelper(this).Handle, nWinHandle);
        
            
        //    InitializeComponent();
        //    //SetParent(FindWindow("Progman", "Program Manager"),new WindowInteropHelper(this).Handle.ToInt32());
        //    //SendMsgToProgman();
        //    //// 设置当前窗口为 Program Manager的子窗口
        //    //Win32Func.SetParent(new WindowInteropHelper(this).Handle, programHandle);

        //    //this.SourceInitialized += MainWindow_SourceInitialized;
        //    //Check();
        //    //this.SourceInitialized += new EventHandler(OnSourceInitialized);

        //    //this.hDesktop = GetDesktopHandle(DesktopLayer.FolderView);
        //    //EmbedDesktop(this, new WindowInteropHelper(this).Handle, this.hDesktop);
        //    //isMouseDown =false;

        //}


        //private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        //{
        //    if (this.WindowState == WindowState.Minimized)
        //        this.WindowState = WindowState.Normal;
        //}

        //private void Window_StateChanged(object sender, EventArgs e)
        //{
        //    if (this.WindowState == WindowState.Minimized)
        //    {
        //        this.WindowState = WindowState.Maximized;
        //        this.Activate(); // 激活窗口，使其获得焦点
        //    }
        //}


        //private void MainWindow_SourceInitialized(object sender, EventArgs e)
        //{
        //    IntPtr hWnd = new WindowInteropHelper(this).Handle;
        //    //SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
        //}

        //private const int WM_ACTIVATE = 0x0006;
        //private const int WA_INACTIVE = 0;
        //private IntPtr HandleMessages(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        //{
        //    if (msg == 0x0112 && ((int)wParam & 0xFFF0) == 0xF020) // 检测到最小化命令
        //    {
        //        handled = true; // 取消最小化命令
        //        this.WindowState = WindowState.Normal; // 调整窗口大小
        //        this.Activate(); // 激活窗口
        //    }
        //    else if (msg == WM_ACTIVATE && wParam.ToInt32() == WA_INACTIVE)
        //    {
        //        // 窗口失去焦点时的处理逻辑
        //        //this.text.Text += $"\r\n当前状态1111：{this.WindowState.ToString()}";
        //    }
        //    return IntPtr.Zero;
        //}

        //public void Check()
        //{
        //    Task.Run(() =>
        //    {
        //        while (true)
        //        {
        //            bool result = false;
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                //this.text.Text += $"\r\n当前状态：{this.WindowState.ToString()}";
        //                //this.WindowState = WindowState.Normal;
        //                //this.Activate(); // 激活窗口
        //                //if (this.WindowState == WindowState.Minimized)
        //                //{
        //                //    result = true;
        //                //    this.WindowState = WindowState.Normal; // 调整窗口大小
        //                //    this.Activate(); // 激活窗口
        //                //}
        //            });

        //            //Application.Current.Dispatcher.Invoke(() =>
        //            //{
        //            //    this.text.Text += $"\r\n当前状态：{this.WindowState.ToString()}";
        //            //});
        //            //if (this.WindowState == WindowState.Minimized)
        //            //{
        //            //    Application.Current.Dispatcher.Invoke(() =>
        //            //    {
        //            //        this.WindowState = WindowState.Normal; // 调整窗口大小
        //            //        this.Activate(); // 激活窗口
        //            //    });
        //            //    break;
        //            //}

        //            if (result)
        //                break;
        //            Thread.Sleep(1000);
        //        }

        //    });
        //}
        //private IntPtr programHandle;

        ///// <summary>
        ///// 向桌面发送消息
        ///// </summary>
        //public void SendMsgToProgman()
        //{
        //    // 桌面窗口句柄，在外部定义，用于后面将我们自己的窗口作为子窗口放入
        //   programHandle = Win32Func.FindWindow("Progman", null);

        //    IntPtr result = IntPtr.Zero;
        //    // 向 Program Manager 窗口发送消息 0x52c 的一个消息，超时设置为2秒
        //    Win32Func.SendMessageTimeout(programHandle, 0x52c, IntPtr.Zero, IntPtr.Zero, 0, 2, result);

        //    // 遍历顶级窗口
        //    Win32Func.EnumWindows((hwnd, lParam) =>
        //    {
        //        // 找到第一个 WorkerW 窗口，此窗口中有子窗口 SHELLDLL_DefView，所以先找子窗口
        //        if (Win32Func.FindWindowEx(hwnd, IntPtr.Zero, "SHELLDLL_DefView", null) != IntPtr.Zero)
        //        {
        //            // 找到当前第一个 WorkerW 窗口的，后一个窗口，及第二个 WorkerW 窗口。
        //            IntPtr tempHwnd = Win32Func.FindWindowEx(IntPtr.Zero, hwnd, "WorkerW", null);

        //            // 隐藏第二个 WorkerW 窗口
        //            Win32Func.ShowWindow(tempHwnd, 0);
        //        }
        //        return true;
        //    }, IntPtr.Zero);
        //}

        //IntPtr hDesktop;
        //public const int GW_CHILD = 5;

        //public IntPtr GetDesktopHandle(DesktopLayer layer)
        //{ //hWnd = new HandleRef();
        //    HandleRef hWnd;
        //    IntPtr hDesktop = new IntPtr();
        //    switch (layer)
        //    {
        //        case DesktopLayer.Progman:
        //            hDesktop = Win32Support.FindWindow("Progman", null);//第一层桌面
        //            break;
        //        case DesktopLayer.SHELLDLL:
        //            hDesktop = Win32Support.FindWindow("Progman", null);//第一层桌面
        //            hWnd =new HandleRef(this, hDesktop);
        //            hDesktop = Win32Support.GetWindow(hWnd, GW_CHILD);//第2层桌面
        //            break;
        //        case DesktopLayer.FolderView:
        //            hDesktop = Win32Support.FindWindow("Progman", null);//第一层桌面
        //            hWnd =new HandleRef(this, hDesktop);
        //            hDesktop = Win32Support.GetWindow(hWnd, GW_CHILD);//第2层桌面
        //            hWnd =new HandleRef(this, hDesktop);
        //            hDesktop = Win32Support.GetWindow(hWnd, GW_CHILD);//第3层桌面
        //            break;
        //    }
        //    return hDesktop;
        //}

        //public void EmbedDesktop(Object embeddedWindow, IntPtr childWindow, IntPtr parentWindow)
        //{
        //    var window = (Window)embeddedWindow;
        //    HandleRef HWND_BOTTOM = new HandleRef(embeddedWindow, new IntPtr(1));
        //    const int SWP_FRAMECHANGED = 0x0020;//发送窗口大小改变消息
        //    Win32Support.SetParent(childWindow, parentWindow);
        //    //Win32Support.SetParent(parentWindow,childWindow);
        //     Win32Support.SetWindowPos(new HandleRef(window, childWindow), HWND_BOTTOM, 300, 300, (int)window.Width, (int)window.Height, SWP_FRAMECHANGED);
        //   // Win32Support.SetWindowPos(new HandleRef(window, parentWindow), HWND_BOTTOM, 300, 300, (int)window.Width, (int)window.Height, SWP_FRAMECHANGED);


        //}

    }

    //public enum DesktopLayer
    //{
    //    Progman =0,
    //    SHELLDLL =1,
    //    FolderView =2
    //}

}