﻿using Desktop.Win32Support.Enums;
using Desktop.Win32Support.Interfaces;
using System.Drawing;
using System.IO;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows;

namespace Desktop.Win32Support
{
    public static class IconExtractor
    {
        /// <summary>
        /// 提取指定路径的图标位图
        /// </summary>
        /// <param name="path">文件路径或虚拟路径（如 shell:::{CLSID}）</param>
        /// <param name="size">图标大小（如 16, 32, 48, 64, 128, 256）</param>
        /// <returns>Bitmap 图标对象</returns>
        public static BitmapSource GetIconBitmap(string pathOrExtension, int size)
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
                Dlls.SHCreateItemFromParsingName(path, IntPtr.Zero, ref iid, out var shellItem);
                var factory = (IShellItemImageFactory)shellItem;

                var iconSize = new SIZE { cx = size, cy = size };
                factory.GetImage(iconSize, siigbf, out var hBitmap);

                if (hBitmap != IntPtr.Zero)
                {
                    var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                      hBitmap,
                      IntPtr.Zero,
                      Int32Rect.Empty,
                      BitmapSizeOptions.FromEmptyOptions());

                    bitmapSource.Freeze(); 
                    Dlls.DeleteObject(hBitmap); // 释放 GDI 对象
                    return bitmapSource;
                }
            }
            catch
            {
                // 返回 null 表示失败
            }

            return null;
        }
    }
}
