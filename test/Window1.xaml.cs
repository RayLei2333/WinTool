using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace test
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        private IntPtr _shellViewPtr = IntPtr.Zero;

        public Window1()
        {
            //IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            //int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            //SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOPMOST | WS_EX_NOACTIVATE);


            InitializeComponent();
            this.Activated += OnActivated;
            
            //ShowDesktop.AddHook(this);

        }
        private void OnActivated(object? sender, EventArgs e)
        {
            UpdateDesktopWindow();

           // UpdateWindowPosition();
           // this._timer.Start();

        }



        private void UpdateDesktopWindow()
        {
            IntPtr shellViewPtr = this.FindDescktopWindow();
            if (shellViewPtr != IntPtr.Zero && this._shellViewPtr != shellViewPtr)
            {
                SetParent(new WindowInteropHelper(this).Handle, shellViewPtr);
                this._shellViewPtr = shellViewPtr;
            }
        }

        private IntPtr FindDescktopWindow()
        {
            IntPtr descktopPtr = GetDesktopWindow();
            IntPtr progmanPtr = FindWindowEx(descktopPtr, new IntPtr(0), "Progman", null);
            do
            {
                IntPtr shellViewPtr = FindWindowEx(progmanPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                progmanPtr = FindWindowEx(descktopPtr, progmanPtr, "WorkerW", null);
                if (progmanPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);

            IntPtr workerWPtr = FindWindowEx(descktopPtr, new IntPtr(0), "WorkerW", null);
            do
            {
                IntPtr shellViewPtr = FindWindowEx(workerWPtr, new IntPtr(0), "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                workerWPtr = FindWindowEx(descktopPtr, workerWPtr, "WorkerW", null);
                if (workerWPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);
            return IntPtr.Zero;
        }
    }
}
