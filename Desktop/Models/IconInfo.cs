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
    public class IconInfo : FileHandler
    {
        public ImageSource Icon { get; set; }

        public IconInfo() { }


        public IconInfo(bool isFolder, string fileSuffixOrFolderPath, ImageSource icon)
        {
            IsFolder = isFolder;
            //FileSuffixOrFoldderPath = fileSuffixOrFolderPath;
            Icon = icon;
        }

        public IconInfo(FileHandler fileHandler)
        {
            this.FullPath = fileHandler.FullPath;
            this.IsFolder = fileHandler.IsFolder;
            this.IsLnkFile = fileHandler.IsLnkFile;
            this.Suffix = fileHandler.Suffix;
        }
    }
}
