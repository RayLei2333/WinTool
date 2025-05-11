using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Desktop.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class IconInfo
    {
        public bool IsFloder { get; set; }

        /// <summary>
        /// 如果IsFloder = true，则保存文件夹路径
        /// 如果IsFloder = false，则保存文件后缀
        /// </summary>
        public string FileSuffixOrFoldderPath { get; set; }


        public ImageSource Icon { get; set; }

        public IconInfo() { }


        public IconInfo(bool isFolder,string fileSuffixOrFolderPath, ImageSource icon)
        {
            IsFloder = isFolder;
            FileSuffixOrFoldderPath = fileSuffixOrFolderPath;
            Icon = icon;
        }
    }
}
