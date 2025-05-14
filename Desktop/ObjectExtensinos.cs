using Desktop.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

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
    }
}
