using Desktop.Manager;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
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
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = new MemoryStream(stream.ToArray());
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }
    }
}
