using Desktop.Win32Support.Enums;
using Desktop.Win32Support.Interfaces;
using Desktop.Win32Support.Models;
using Desktop.Win32Support.Structs;
using System.IO;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Controls;

namespace Desktop.Win32Support
{
    public static class DesktopWindow
    {
        /// <summary>
        /// 查找桌面窗体
        /// </summary>
        /// <returns></returns>
        public static nint FindDesktopWindow()
        {
            nint descktopPtr = Dlls.GetDesktopWindow();
            nint progmanPtr = Dlls.FindWindowEx(descktopPtr, new nint(0), "Progman", null);
            do
            {
                nint shellViewPtr = Dlls.FindWindowEx(progmanPtr, new nint(0), "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                progmanPtr = Dlls.FindWindowEx(descktopPtr, progmanPtr, "WorkerW", null);
                if (progmanPtr.ToInt64() == 0)
                {
                    break;
                }
            } while (true);

            nint workerWPtr = Dlls.FindWindowEx(descktopPtr, new nint(0), "WorkerW", null);
            do
            {
                nint shellViewPtr = Dlls.FindWindowEx(workerWPtr, new nint(0), "SHELLDLL_DefView", null);
                if (shellViewPtr.ToInt64() != 0)
                {
                    return shellViewPtr;
                }
                workerWPtr = Dlls.FindWindowEx(descktopPtr, workerWPtr, "WorkerW", null);
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
                hwndFolderView = Dlls.FindWindowEx(hwndDesktop, IntPtr.Zero, "SysListView32", null);
                if (hwndFolderView != IntPtr.Zero)
                {
                    break;
                }
                hwndDesktop = Dlls.FindWindowEx(IntPtr.Zero, hwndDesktop, null, null);
            }

            return hwndFolderView;
        }

        public static float GetDPI()
        {
            // 获取桌面文件夹视图句柄
            IntPtr hwndFolderView = FindSysListView32();
            return GetDPI(hwndFolderView);
        }

        public static float GetDPI(IntPtr hwndFolderView)
        {
            // 获取 DPI 缩放
            uint dpi = Dlls.GetDpiForWindow(hwndFolderView);
            float scale = dpi / 96.0f;
            return scale;
        }

        /// <summary>
        /// 获取桌面壁纸路径
        /// </summary>
        /// <returns></returns>
        public static string GetDesktopWallpaperPath()
        {
            StringBuilder wallpaperPathSb = new StringBuilder(256); // 分配足够的空间
            Dlls.SystemParametersInfo(DesktopConstant.SPI_GETDESKWALLPAPER, wallpaperPathSb.Capacity, wallpaperPathSb, 0);
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

        /// <summary>
        /// 获取桌面图标排列方式（自动排列图标、对齐到网格）
        /// </summary>
        public static DesktopIconArrange GetDesktopIconView()
        {

            // 获取桌面文件夹视图句柄
            IntPtr hwndFolderView = FindSysListView32();


            // 获取桌面文件夹视图的扩展样式
            IntPtr extendedStyle = Dlls.SendMessage(hwndFolderView, DesktopConstant.LVM_GETEXTENDEDLISTVIEWSTYLE, IntPtr.Zero, IntPtr.Zero);
            //iconView
            IntPtr spacing = Dlls.SendMessage(hwndFolderView, DesktopConstant.LVM_GETITEMSPACING, IntPtr.Zero, IntPtr.Zero);
            int spacingInt = spacing.ToInt32();

            int x = spacingInt & 0xFFFF;
            int y = (spacingInt >> 16) & 0xFFFF;


            // 获取 DPI 缩放
            float scale = GetDPI(hwndFolderView);

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
                AutoArrange = ((extendedStyle.ToInt32() & DesktopConstant.LVS_AUTOARRANGE) == DesktopConstant.LVS_AUTOARRANGE),
                AlignedToGrid = ((extendedStyle.ToInt32() & DesktopConstant.LVS_SNAPTOGRID) == DesktopConstant.LVS_SNAPTOGRID),
                X = xReal,
                Y = yReal,
            };
        }

        /// <summary>
        /// 获取桌面显示的所有文件
        /// </summary>
        /// <returns></returns>
        public static List<DesktopFile> GetDesktopFiles()
        {
            var icons = GetDesktopShowFile();
            var allIcon = GetDesktopAllFile();
            foreach (var item in icons)
            {
                string path = allIcon.FirstOrDefault(t => string.Equals(t.Name, item.Name, StringComparison.OrdinalIgnoreCase))?.FilePath;
                item.FilePath = path;
            }
            return icons;
        }

        //获取桌面显示的所有图标
        private static List<DesktopFile> GetDesktopShowFile()
        {
            var listView = FindSysListView32();
            if (listView == IntPtr.Zero)
                return null;

            List<DesktopFile> desktopIconList = new List<DesktopFile>();
            _ = Dlls.GetWindowThreadProcessId(listView, out int processId);
            IntPtr hProcess = Dlls.OpenProcess(DesktopConstant.PROCESS_VM_OPERATION | DesktopConstant.PROCESS_VM_READ | DesktopConstant.PROCESS_VM_WRITE | DesktopConstant.PROCESS_QUERY_INFORMATION, false, processId);

            int count = (int)Dlls.SendMessage(listView, DesktopConstant.LVM_GETITEMCOUNT, 0, IntPtr.Zero);

            IntPtr remoteBuffer = Dlls.VirtualAllocEx(hProcess, IntPtr.Zero, 4096, DesktopConstant.MEM_COMMIT, DesktopConstant.PAGE_READWRITE);

            float scale = GetDPI(listView);

            for (int i = 0; i < count; i++)
            {
                // --- 获取位置 ---
                IntPtr positionBuffer = remoteBuffer;
                Dlls.SendMessage(listView, DesktopConstant.LVM_GETITEMPOSITION, i, positionBuffer);

                byte[] posData = new byte[8]; // 2x int
                Dlls.ReadProcessMemory(hProcess, positionBuffer, posData, posData.Length, out _);
                int x = BitConverter.ToInt32(posData, 0);
                int y = BitConverter.ToInt32(posData, 4);
                //计算DPI
                x = (int)(x / scale);
                y = (int)(y / scale);

                // --- 获取标题 ---
                IntPtr textBuffer = remoteBuffer + 64; // 偏移64，避免覆盖
                LVITEM lvItem = new LVITEM
                {
                    mask = DesktopConstant.LVIF_TEXT,
                    iItem = i,
                    iSubItem = 0,
                    pszText = textBuffer,
                    cchTextMax = 260
                };

                int lvItemSize = Marshal.SizeOf(typeof(LVITEM));
                IntPtr localLvItem = Marshal.AllocHGlobal(lvItemSize);
                Marshal.StructureToPtr(lvItem, localLvItem, false);

                Dlls.WriteProcessMemory(hProcess, remoteBuffer, localLvItem, lvItemSize, out _);
                Dlls.SendMessage(listView, DesktopConstant.LVM_GETITEMTEXT, i, remoteBuffer);

                byte[] textData = new byte[520]; // Unicode string
                Dlls.ReadProcessMemory(hProcess, textBuffer, textData, textData.Length, out _);
                string title = Encoding.Unicode.GetString(textData);//.TrimEnd('\0');
                int nullIndex = title.IndexOf('\0');
                title = nullIndex >= 0 ? title.Substring(0, nullIndex) : title;
                Marshal.FreeHGlobal(localLvItem);

                IntPtr remoteRect = Dlls.VirtualAllocEx(hProcess, IntPtr.Zero, (uint)Marshal.SizeOf<RECT>(), DesktopConstant.MEM_COMMIT, DesktopConstant.PAGE_READWRITE);
                if (remoteRect == IntPtr.Zero)
                    return null;

                // 写入 LVIR_BOUNDS 作为结构体前4字节
                byte[] boundsSelector = BitConverter.GetBytes(DesktopConstant.LVIR_BOUNDS);
                Dlls.WriteProcessMemory(hProcess, remoteRect, boundsSelector, (uint)boundsSelector.Length, out _);

                // 发送获取矩形请求
                Dlls.SendMessage(listView, DesktopConstant.LVM_GETITEMRECT, i, remoteRect);

                // 读取 RECT 结构体
                IntPtr localRect = Marshal.AllocHGlobal(Marshal.SizeOf<RECT>());
                Dlls.ReadProcessMemory(hProcess, remoteRect, localRect, (uint)Marshal.SizeOf<RECT>(), out _);
                RECT rect = Marshal.PtrToStructure<RECT>(localRect);

                //Console.WriteLine($"    矩形：Left={rect.Left}, Top={rect.Top}, Right={rect.Right}, Bottom={rect.Bottom}");

                Marshal.FreeHGlobal(localRect);
                Dlls.VirtualFreeEx(hProcess, remoteRect, 0, DesktopConstant.MEM_RELEASE);

                desktopIconList.Add(new DesktopFile()
                {
                    Index = i,
                    Name = title,
                    X = x,
                    Y = y,
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom,
                });
            }

            Dlls.VirtualFreeEx(hProcess, remoteBuffer, 0, DesktopConstant.MEM_RELEASE);
            Dlls.CloseHandle(hProcess);

            return desktopIconList;
        }

        //获取桌面所有图标
        private static List<DesktopFile> GetDesktopAllFile()
        {
            var result = new List<DesktopFile>();
            //var listView = FindSysListView32();
            int nResult = Dlls.SHGetDesktopFolder(out nint pUnkownDesktopFolder);
            if (0 != nResult)
            {
                return null;
            }
            IShellFolder desktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            // 枚举桌面图标的 PIDL
            desktopFolder.EnumObjects(IntPtr.Zero, SHCONTF.FOLDERS | SHCONTF.NONFOLDERS | SHCONTF.INCLUDEHIDDEN, out IEnumIDList enumIDList);
            IntPtr pidl;
            uint fetched;
            while (enumIDList.Next(1, out pidl, out fetched) == 0 && fetched == 1)
            {
                string name = GetDisplayName(Marshal.SizeOf<STRRET>(), desktopFolder, pidl, SHGNO.NORMAL);
                string parsing = GetDisplayName(Marshal.SizeOf<STRRET>(), desktopFolder, pidl, SHGNO.FORPARSING);
                result.Add(new DesktopFile
                {
                    Name = name,
                    FilePath = parsing,
                });

                Marshal.FreeCoTaskMem(pidl);
            }
            return result;
        }

        private static string GetDisplayName(int cb, IShellFolder desktopFolder, nint pidl, SHGNO shgno)
        {
            nint pStrRet = Marshal.AllocCoTaskMem(cb);
            Marshal.WriteInt32(pStrRet, 0, 0);
            desktopFolder.GetDisplayNameOf(pidl, shgno, pStrRet);
            StringBuilder strFolder = new StringBuilder(cb);
            Dlls.StrRetToBuf(pStrRet, pidl, strFolder, cb);
            Marshal.FreeCoTaskMem(pStrRet);
            pStrRet = nint.Zero;
            return strFolder.ToString();

        }

        private static IntPtr _shellViewPtr;
        /// <summary>
        /// 将窗体置于底层，但为桌面上层
        /// </summary>
        /// <param name="hWndChild"></param>
        public static void UpdateDesktopWindow(nint hWndChild)
        {
            nint shellViewPtr = FindDesktopWindow();
            if (shellViewPtr != nint.Zero && _shellViewPtr != shellViewPtr)
            {
                Dlls.SetParent(hWndChild, shellViewPtr);
                _shellViewPtr = shellViewPtr;
            }
        }

    }
}
