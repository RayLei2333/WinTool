using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using static Desktop.Win32Support.ShellContextMenu;

namespace Desktop.Win32Support
{
    internal class DesktopWindow
    {
        #region Variables and Constants
        const int SPI_GETDESKWALLPAPER = 0x0073;
        nint _shellViewPtr = nint.Zero;

        const uint LVM_FIRST = 0x1000;

        const int LVM_GETITEMCOUNT = 0x1000 + 4;
        const int LVM_GETITEMPOSITION = 0x1000 + 16;
        const int LVM_GETITEMTEXT = 0x1000 + 115;

        const int LVIF_TEXT = 0x0001;

        const uint PROCESS_VM_OPERATION = 0x0008;
        const uint PROCESS_VM_READ = 0x0010;
        const uint PROCESS_VM_WRITE = 0x0020;
        const uint PROCESS_QUERY_INFORMATION = 0x0400;

        const uint MEM_COMMIT = 0x1000;
        const uint MEM_RELEASE = 0x8000;
        const uint PAGE_READWRITE = 0x04;

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

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref LVITEM lParam);

        [DllImport("user32.dll")]
        static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("shell32.dll")]
        private static extern int SHGetDesktopFolder(out nint ppshf);
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public uint mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
        }
        #endregion

        #region FindDesktopWindow
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


        public static List<DesktopIconInfo> GetDesktopIcon()
        {
            var listView = FindSysListView32();
            if (listView == IntPtr.Zero)
                return null;

            List<DesktopIconInfo> desktopIconList = new List<DesktopIconInfo>();
            _ = GetWindowThreadProcessId(listView, out int processId);
            IntPtr hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, processId);

            int count = (int)SendMessage(listView, LVM_GETITEMCOUNT, 0, IntPtr.Zero);

            IntPtr remoteBuffer = VirtualAllocEx(hProcess, IntPtr.Zero, 4096, MEM_COMMIT, PAGE_READWRITE);

            for (int i = 0; i < count; i++)
            {
                // --- 获取位置 ---
                IntPtr positionBuffer = remoteBuffer;
                SendMessage(listView, LVM_GETITEMPOSITION, (uint)i, positionBuffer);

                byte[] posData = new byte[8]; // 2x int
                ReadProcessMemory(hProcess, positionBuffer, posData, posData.Length, out _);
                int x = BitConverter.ToInt32(posData, 0);
                int y = BitConverter.ToInt32(posData, 4);

                // --- 获取标题 ---
                IntPtr textBuffer = remoteBuffer + 64; // 偏移64，避免覆盖
                LVITEM lvItem = new LVITEM
                {
                    mask = LVIF_TEXT,
                    iItem = i,
                    iSubItem = 0,
                    pszText = textBuffer,
                    cchTextMax = 260
                };

                int lvItemSize = Marshal.SizeOf(typeof(LVITEM));
                IntPtr localLvItem = Marshal.AllocHGlobal(lvItemSize);
                Marshal.StructureToPtr(lvItem, localLvItem, false);

                WriteProcessMemory(hProcess, remoteBuffer, localLvItem, lvItemSize, out _);
                SendMessage(listView, LVM_GETITEMTEXT, (uint)i, remoteBuffer);

                byte[] textData = new byte[520]; // Unicode string
                ReadProcessMemory(hProcess, textBuffer, textData, textData.Length, out _);
                string title = Encoding.Unicode.GetString(textData);//.TrimEnd('\0');
                int nullIndex = title.IndexOf('\0');
                title = nullIndex >= 0 ? title.Substring(0, nullIndex) : title;
                Marshal.FreeHGlobal(localLvItem);
                desktopIconList.Add(new DesktopIconInfo()
                {
                    Index = i,
                    Name = title,
                    X = x,
                    Y = y,
                });
            }

            VirtualFreeEx(hProcess, remoteBuffer, 0, MEM_RELEASE);
            CloseHandle(hProcess);

            return desktopIconList;
        }

        public static List<DesktopIconInfo> GetDesktopIcon3()
        {
            var result = new List<DesktopIconInfo>();
            var listView = FindSysListView32();
            int nResult = SHGetDesktopFolder(out nint pUnkownDesktopFolder);
            if (0 != nResult)
            {
                throw new ShellContextMenuException("Failed to get the desktop shell folder");
            }
            IShellFolder desktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            // 枚举桌面图标的 PIDL
            desktopFolder.EnumObjects(IntPtr.Zero, SHCONTF.FOLDERS | SHCONTF.NONFOLDERS | SHCONTF.INCLUDEHIDDEN, out IEnumIDList enumIDList);
            //desktopFolder.EnumObjects(IntPtr.Zero,  SHCONTF.NONFOLDERS , out IEnumIDList enumIDList);
            int index = 0;
            IntPtr pidl;
            uint fetched;
            _ = GetWindowThreadProcessId(listView, out int processId);
            IntPtr hProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ | PROCESS_VM_WRITE | PROCESS_QUERY_INFORMATION, false, processId);
            IntPtr remoteBuffer = VirtualAllocEx(hProcess, IntPtr.Zero, 4096, MEM_COMMIT, PAGE_READWRITE);

            while (enumIDList.Next(1, out pidl, out fetched) == 0 && fetched == 1)
            {
                STRRET strret;
                desktopFolder.GetDisplayNameOf(pidl, SHGDN_NORMAL, out strret);
                //文件名
                string name = GetDisplayName(ref strret, pidl);

                desktopFolder.GetDisplayNameOf(pidl, SHGDN_FORPARSING, out strret);
                //路径
                string parsing = GetDisplayName(ref strret, pidl);

                IntPtr positionBuffer = remoteBuffer;
                SendMessage(listView, LVM_GETITEMPOSITION, (uint)index, positionBuffer);

                byte[] posData = new byte[8]; // 2x int
                ReadProcessMemory(hProcess, positionBuffer, posData, posData.Length, out _);
                int x = BitConverter.ToInt32(posData, 0);
                int y = BitConverter.ToInt32(posData, 4);

                //POINT pt = new POINT();
                //if (!SafeGetItemPosition(listView, index, out pt))
                //{
                //    pt.X = pt.Y = -1;
                //}

                result.Add(new DesktopIconInfo
                {
                    Name = name,
                    FilePath = parsing,
                    X = x,
                    Y = y
                });

                Marshal.FreeCoTaskMem(pidl);
                index++;
            }
            return result;
        }


        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, ref POINT lParam);

        private static bool SafeGetItemPosition(IntPtr listView, int index, out POINT pt)
        {
            pt = new POINT();
            IntPtr result;
            IntPtr success = SendMessageTimeout(
                listView,
                LVM_GETITEMPOSITION,
                index,
                ref pt,
                SMTO_ABORTIFHUNG,
                1000,
                out result
            );
            return success != IntPtr.Zero;
        }


        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SendMessageTimeout(
    IntPtr hWnd,
    uint Msg,
    int wParam,
    ref POINT lParam,
    uint fuFlags,
    uint uTimeout,
    out IntPtr lpdwResult
);
        private const uint SMTO_ABORTIFHUNG = 0x0002;
        private const uint SHGDN_NORMAL = 0x0000;
        private const uint SHGDN_FORPARSING = 0x8000;

        private static string GetDisplayName(ref STRRET strret, IntPtr pidl)
        {
            StringBuilder sb = new StringBuilder(260);
            StrRetToBuf(ref strret, pidl, sb, (uint)sb.Capacity);
            return sb.ToString();
        }

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        private static extern int StrRetToBuf(ref STRRET pstr, IntPtr pidl, StringBuilder pszBuf, uint cchBuf);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT { public int X, Y; }

        [StructLayout(LayoutKind.Explicit, Size = 520)]
        public struct STRRET
        {
            [FieldOffset(0)] public uint uType;
            [FieldOffset(4)] public IntPtr pOleStr;
        }
        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214F2-0000-0000-C000-000000000046")]
        public interface IEnumIDList
        {
            [PreserveSig]
            int Next(uint celt, out IntPtr rgelt, out uint pceltFetched);
            void Skip(uint celt);
            void Reset();
            void Clone(out IEnumIDList ppenum);
        }

        [Flags]
        public enum SHCONTF
        {
            FOLDERS = 0x0020,
            NONFOLDERS = 0x0040,
            INCLUDEHIDDEN = 0x0080
        }

        [Flags]
        public enum SFGAO : uint
        {
            SFGAO_CANCOPY = 0x00000001,
            SFGAO_CANMOVE = 0x00000002,
            SFGAO_CANLINK = 0x00000004,
            SFGAO_CANRENAME = 0x00000010,
            SFGAO_CANDELETE = 0x00000020,
            SFGAO_HASPROPSHEET = 0x00000040,
        }

        [ComImport]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        [Guid("000214E6-0000-0000-C000-000000000046")]
        public interface IShellFolder
        {
            void ParseDisplayName(IntPtr hwnd, IntPtr pbc, [MarshalAs(UnmanagedType.LPWStr)] string pszDisplayName, out uint pchEaten, out IntPtr ppidl, ref uint pdwAttributes);
            void EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IEnumIDList ppenumIDList);
            void BindToObject(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
            void BindToStorage(IntPtr pidl, IntPtr pbc, [In] ref Guid riid, out IntPtr ppv);
            void CompareIDs(IntPtr lParam, IntPtr pidl1, IntPtr pidl2);
            void CreateViewObject(IntPtr hwndOwner, [In] ref Guid riid, out IntPtr ppv);
            void GetAttributesOf(uint cidl, [In] IntPtr apidl, ref SFGAO rgfInOut);
            void GetUIObjectOf(IntPtr hwndOwner, uint cidl, [In] ref IntPtr apidl, [In] ref Guid riid, IntPtr rgfReserved, out IntPtr ppv);
            void GetDisplayNameOf(IntPtr pidl, uint uFlags, out STRRET pName);
            void SetNameOf(IntPtr hwnd, IntPtr pidl, [MarshalAs(UnmanagedType.LPWStr)] string pszName, uint uFlags, out IntPtr ppidlOut);
        }
    }
}
