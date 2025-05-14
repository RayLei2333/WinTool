using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Win32Support
{
    class IconExtractor
    {
        [Flags]
        public enum SIIGBF
        {
            SIIGBF_RESIZETOFIT = 0x00,
            SIIGBF_BIGGERSIZEOK = 0x01,
            SIIGBF_MEMORYONLY = 0x02,
            SIIGBF_ICONONLY = 0x04,
            SIIGBF_THUMBNAILONLY = 0x08,
            SIIGBF_INCACHEONLY = 0x10
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            public int cx;
            public int cy;
        }

        [ComImport]
        [Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItemImageFactory
        {
            void GetImage(SIZE size, SIIGBF flags, out IntPtr phbm);
        }

        [ComImport]
        [Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IShellItem
        {
        }

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        private static extern void SHCreateItemFromParsingName(
            [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
            IntPtr pbc,
            [In] ref Guid riid,
            [Out][MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// 提取指定路径的图标位图
        /// </summary>
        /// <param name="path">文件路径或虚拟路径（如 shell:::{CLSID}）</param>
        /// <param name="size">图标大小（如 16, 32, 48, 64, 128, 256）</param>
        /// <returns>Bitmap 图标对象</returns>
        public static Bitmap GetIconBitmap(string pathOrExtension,int size)
        {
            if (string.IsNullOrWhiteSpace(pathOrExtension))
                throw new ArgumentException("路径不能为空");

            // 如果只提供了扩展名，则生成一个虚拟文件路径
            string path;
            SIIGBF siigbf;
            if (pathOrExtension.StartsWith("."))
            {
                siigbf = SIIGBF.SIIGBF_ICONONLY | SIIGBF.SIIGBF_BIGGERSIZEOK;
                // 创建一个临时文件以确保路径存在
                string tempDir = Path.Combine(Path.GetTempPath(), "ShellIconTemp");
                Directory.CreateDirectory(tempDir);
                path = Path.Combine(tempDir, "dummy" + pathOrExtension);
                if (!File.Exists(path))
                {
                    File.WriteAllText(path, string.Empty);
                }
            }
            else
            {
                path = pathOrExtension;
                siigbf = SIIGBF.SIIGBF_BIGGERSIZEOK;
            }

            var iid = typeof(IShellItemImageFactory).GUID;

            try
            {
                SHCreateItemFromParsingName(path, IntPtr.Zero, ref iid, out var shellItem);
                var factory = (IShellItemImageFactory)shellItem;

                var iconSize = new SIZE { cx = size, cy = size };
                factory.GetImage(iconSize, siigbf, out var hBitmap);

                if (hBitmap != IntPtr.Zero)
                {
                    var bmp = Image.FromHbitmap(hBitmap);
                    DeleteObject(hBitmap);
                    return bmp;
                }
            }
            catch
            {
                // 返回 null 表示失败
            }

            return null;
        }

        ///// <summary>
        ///// 提取指定路径的图标位图
        ///// </summary>
        ///// <param name="path">文件路径或虚拟路径（如 shell:::{CLSID}）</param>
        ///// <param name="size">图标大小（如 16, 32, 48, 64, 128, 256）</param>
        ///// <returns>Bitmap 图标对象</returns>
        //public static Bitmap GetIconBitmap(string path, int size = 256)
        //{
        //    var iidImageFactory = typeof(IShellItemImageFactory).GUID;

        //    try
        //    {
        //        SHCreateItemFromParsingName(path, IntPtr.Zero, ref iidImageFactory, out var shellItem);
        //        var factory = (IShellItemImageFactory)shellItem;

        //        var iconSize = new SIZE { cx = size, cy = size };
        //        factory.GetImage(iconSize, SIIGBF.SIIGBF_BIGGERSIZEOK, out var hBitmap);

        //        if (hBitmap != IntPtr.Zero)
        //        {
        //            var bmp = Image.FromHbitmap(hBitmap);
        //            DeleteObject(hBitmap);
        //            return bmp;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        // 忽略错误（路径错误、虚拟项不可获取等）
        //    }

        //    return null;
        //}


        ///// <summary>
        ///// 提取文件路径或扩展名的默认图标（非缩略图）
        ///// </summary>
        ///// <param name="pathOrExtension">完整路径或 .扩展名（如 .png）</param>
        ///// <param name="size">目标图标大小</param>
        ///// <returns>Bitmap 图标对象</returns>
        //public static Bitmap GetDefaultIcon(string pathOrExtension, int size = 256)
        //{
        //    if (string.IsNullOrWhiteSpace(pathOrExtension))
        //        throw new ArgumentException("路径不能为空");

        //    // 如果只提供了扩展名，则生成一个虚拟文件路径
        //    string path = string.Empty;
        //    if (pathOrExtension.StartsWith("."))
        //    {
        //        // 创建一个临时文件以确保路径存在
        //        string tempDir = Path.Combine(Path.GetTempPath(), "ShellIconTemp");
        //        Directory.CreateDirectory(tempDir);
        //        path = Path.Combine(tempDir, "dummy" + pathOrExtension);
        //        if (!File.Exists(path))
        //        {
        //            File.WriteAllText(path, string.Empty);
        //        }
        //    }
        //    else
        //    {
        //        path = pathOrExtension;
        //    }

        //    var iid = typeof(IShellItemImageFactory).GUID;
        //    try
        //    {
        //        SHCreateItemFromParsingName(path, IntPtr.Zero, ref iid, out var shellItem);
        //        var factory = (IShellItemImageFactory)shellItem;

        //        var iconSize = new SIZE { cx = size, cy = size };
        //        factory.GetImage(iconSize, SIIGBF.SIIGBF_ICONONLY | SIIGBF.SIIGBF_BIGGERSIZEOK, out var hBitmap);

        //        if (hBitmap != IntPtr.Zero)
        //        {
        //            var bmp = Image.FromHbitmap(hBitmap);
        //            DeleteObject(hBitmap);
        //            return bmp;
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        // 返回 null 表示失败
        //    }

        //    return null;
        //}

    }
}
