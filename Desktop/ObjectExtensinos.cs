using Desktop.Manager;
using Desktop.Win32Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Desktop
{
    public static class ObjectExtensinos
    {
        /// <summary>
        /// 检查字符串是否.lnk
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckIsLnkFileSuffix(this string str)
        {
            return string.Equals(str, ".lnk", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 检查字符串是否为图像文件后缀
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool CheckIsImgFileSuffix(this string str)
        {

            return IconManager.ImageFormats.Contains(str);
        }

        /// <summary>
        /// 将Bitmap转换为BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>

        public static ImageSource ToImageSource(this Bitmap bitmap)
        {
            //using (var stream = new MemoryStream())
            //{
            //    bitmap.Save(stream, ImageFormat.Png);
            //    var bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
            //    bitmapImage.EndInit();
            //    return bitmapImage;
            //}

            //var ms = new MemoryStream();
            //bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //ms.Position = 0;
            //var bitmapImage = new BitmapImage();
            //bitmapImage.BeginInit();
            //bitmapImage.StreamSource = ms;
            //bitmapImage.EndInit();
            //return bitmapImage;


            //IntPtr hBitmap = bitmap.GetHbitmap(); // 包含 alpha 的 HBitmap

            //try
            //{
            //    return Imaging.CreateBitmapSourceFromHBitmap(
            //        hBitmap,
            //        IntPtr.Zero,
            //        Int32Rect.Empty,
            //        BitmapSizeOptions.FromEmptyOptions());
            //}
            //finally
            //{
            //    Dlls.DeleteObject(hBitmap); // 避免内存泄漏
            //}

            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // 强制立即加载以避免流关闭问题
                bitmapImage.StreamSource = memory;
                bitmapImage.CreateOptions = BitmapCreateOptions.PreservePixelFormat; // 保留透明通道
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // 可选：如果用于跨线程 UI

                return bitmapImage;
            }
        }
    }
}
