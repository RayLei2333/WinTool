using Desktop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Desktop.Win32Support
{
    internal class DesktopWindow
    {
        #region Variables and Constants
        const int SPI_GETDESKWALLPAPER = 0x0073;
        nint _shellViewPtr = nint.Zero;

        const uint LVM_FIRST = 0x1000;
        //获取图标视图，大图标、中等图标、小图标
        const uint LVM_GETITEMSPACING = LVM_FIRST + 51;
        //获取自动排列and网格对齐方式
        const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;

        const int LVS_AUTOARRANGE = 0x0040;
        const int LVS_ALIGNLEFT = 0x0800;
        const int LVS_SNAPTOGRID = 0x0400;
        #endregion

        #region DLL Import
        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint GetDesktopWindow();

        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        public static extern nint FindWindowEx(nint hwndParent, nint hwndChildAfter, string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(nint hWndChild, nint hWndNewParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hwnd);
        #endregion

        #region FindDesktopWindow()
        /// <summary>
        /// 查找桌面窗体
        /// </summary>
        /// <returns></returns>
        public static nint FindDesktopWindow()
        {
            nint descktopPtr = GetDesktopWindow();
            nint progmanPtr = FindWindowEx(descktopPtr, new nint(0), "Progman", null);
            do
            {
                nint shellViewPtr = FindWindowEx(progmanPtr, new nint(0), "SHELLDLL_DefView", null);
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

            nint workerWPtr = FindWindowEx(descktopPtr, new nint(0), "WorkerW", null);
            do
            {
                nint shellViewPtr = FindWindowEx(workerWPtr, new nint(0), "SHELLDLL_DefView", null);
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
            return nint.Zero;
        }

        /// <summary>
        /// 查找桌面文件夹视图句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr FindSysListView32()
        {
            IntPtr hwndDesktop = FindDesktopWindow();

            // 获取桌面文件夹视图句柄
            IntPtr hwndFolderView = IntPtr.Zero;
            while (hwndDesktop != IntPtr.Zero)
            {
                hwndFolderView = FindWindowEx(hwndDesktop, IntPtr.Zero, "SysListView32", null);
                if (hwndFolderView != IntPtr.Zero)
                {
                    break;
                }
                hwndDesktop = FindWindowEx(IntPtr.Zero, hwndDesktop, null, null);
            }

            return hwndFolderView;


        }

        /// <summary>
        /// 将窗体置于底层，但为桌面上层
        /// </summary>
        /// <param name="hWndChild"></param>
        public void UpdateDesktopWindow(nint hWndChild)
        {
            nint shellViewPtr = FindDesktopWindow();
            if (shellViewPtr != nint.Zero && _shellViewPtr != shellViewPtr)
            {
                SetParent(hWndChild, shellViewPtr);
                _shellViewPtr = shellViewPtr;
            }
        }
        #endregion

        #region GetDesktopWallpaperPath()
        /// <summary>
        /// 获取桌面壁纸路径
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopWallpaperPath()
        {
            StringBuilder wallpaperPathSb = new StringBuilder(256); // 分配足够的空间
            SystemParametersInfo(SPI_GETDESKWALLPAPER, wallpaperPathSb.Capacity, wallpaperPathSb, 0);
            string wallpaperPath = wallpaperPathSb.ToString();
            if (string.IsNullOrEmpty(wallpaperPath) || !File.Exists(wallpaperPath))
            {
                string windowsThememsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Microsoft\\Windows\\Themes");
                wallpaperPath = Path.Combine(windowsThememsPath, "TranscodedWallpaper");
                if (Directory.Exists(windowsThememsPath) && File.Exists(wallpaperPath))
                    return wallpaperPath;
                return null;

            }
            if (File.Exists(wallpaperPath))
                return wallpaperPath;
            return null;
            //return wallpaperPath.ToString();
        }
        #endregion

        public static float GetDPI()
        {
            // 获取桌面文件夹视图句柄
            IntPtr hwndFolderView = FindSysListView32();
            return GetDPI(hwndFolderView);
        }

        public static float GetDPI(IntPtr hwndFolderView)
        {
            // 获取 DPI 缩放
            uint dpi = GetDpiForWindow(hwndFolderView);
            float scale = dpi / 96.0f;
            return scale;
        }

        /// <summary>
        /// 获取桌面图标排列方式（自动排列图标、对齐到网格）
        /// </summary>
        public static DesktopIconArrange GetDesktopIconView()
        {

            // 获取桌面文件夹视图句柄
            IntPtr hwndFolderView = FindSysListView32();


            // 获取桌面文件夹视图的扩展样式
            IntPtr extendedStyle = SendMessage(hwndFolderView, LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero);
            //iconView
            IntPtr spacing = SendMessage(hwndFolderView, LVM_GETITEMSPACING, IntPtr.Zero, IntPtr.Zero);
            int spacingInt = spacing.ToInt32();

            int x = spacingInt & 0xFFFF;
            int y = (spacingInt >> 16) & 0xFFFF;


            // 获取 DPI 缩放
            uint dpi = GetDpiForWindow(hwndFolderView);
            float scale = dpi / 96.0f;

            int xReal = (int)(x / scale);
            int yReal = (int)(y / scale);
            //// 用缩放后值进行判断
            //if (xReal < 85)
            //    return "小图标";
            //else if (xReal < 105)
            //    return "中等图标";
            //else if (xReal < 125)
            //    return "大图标";
            //else
            //    return "超大图标";

            return new DesktopIconArrange()
            {
                AutoArrange = ((extendedStyle.ToInt32() & LVS_AUTOARRANGE) == LVS_AUTOARRANGE),
                AlignedToGrid = ((extendedStyle.ToInt32() & LVS_SNAPTOGRID) == LVS_SNAPTOGRID),
                X = xReal,
                Y = yReal,
            };
        }

    }
}
