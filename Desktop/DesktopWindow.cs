using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Desktop
{
    internal class DesktopWindow
    {
        #region Variables and Constants
        const int SPI_GETDESKWALLPAPER = 0x0073;
        nint _shellViewPtr = nint.Zero;
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
        #endregion

        #region FindDescktopWindow()
        /// <summary>
        /// 查找桌面窗体
        /// </summary>
        /// <returns></returns>
        private static nint FindDescktopWindow()
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
        /// 将窗体置于底层，但为桌面上层
        /// </summary>
        /// <param name="hWndChild"></param>
        public void UpdateDesktopWindow(nint hWndChild)
        {
            nint shellViewPtr = FindDescktopWindow();
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

    }
}
