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
    public class IconInfo // : FileHandler
    {
        public bool IsFolder { get; set; }

        public bool IsLnkFile { get; set; }

        public bool IsImageFile { get; set; }

        public string Suffix { get; set; }

        public string FullPath { get; set; }

        public ImageSource Icon { get; set; }

        public IconInfo() { }

        public IconInfo Clone()
        {
            IconInfo clone = new IconInfo();
            clone.IsFolder = IsFolder;
            clone.IsLnkFile = IsLnkFile;
            clone.IsImageFile = IsImageFile;
            clone.Suffix = Suffix;
            clone.FullPath = FullPath;
            clone.Icon = Icon;

            return clone;

        }

        //public IconInfo(FileHandler fileHandler)
        //{
        //    this.FullPath = fileHandler.FullPath;
        //    this.IsFolder = fileHandler.IsFolder;
        //    this.IsLnkFile = fileHandler.IsLnkFile;
        //    this.Suffix = fileHandler.Suffix;
        //}
    }
}
