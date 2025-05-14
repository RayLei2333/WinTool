using Desktop.Models;
using Desktop.Win32Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace Desktop.Manager
{
    internal class DesktopManager
    {
        private static DesktopManager _instence = new DesktopManager();

        public static DesktopManager Instence => _instence;

        //private 

        public DesktopManager()
        {
            //Init();
        }

        //获取所有桌面图标及路径匹配
        public List<DesktopIconInfo> GetAllDesktopFile()
        {
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string commonDesktop = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);
            var allFiles = Directory.GetFileSystemEntries(desktopPath).Concat(Directory.GetFileSystemEntries(commonDesktop));
            Dictionary<string, string> physicalFiles = new();
            foreach (var item in allFiles)
            {
                physicalFiles[System.IO.Path.GetFileName(item)] = item;
            }
            //foreach (var path in Directory.GetFiles(desktopPath))
            //{
            //    physicalFiles[Path.GetFileName(path)] = path;
            //}
            var icons = DesktopWindow.GetDesktopIcon3();
            var icon2 = DesktopWindow.GetDesktopIcon();
            foreach (var item in icons)
            {
                string filePath;
                physicalFiles.TryGetValue(item.Name, out filePath);
                if(string.IsNullOrEmpty(filePath))
                    physicalFiles.TryGetValue($"{item.Name}.lnk", out filePath);
                item.FilePath = filePath;
            }

            return icons;
            //var files = Directory.GetFiles(desktopPath);

            //foreach (var file in files)
            //{
            //    var icon = new DesktopIconInfo
            //    {
            //        Name = Path.GetFileNameWithoutExtension(file),
            //        //FilePath = file,
            //        IsShortcut = Path.GetExtension(file).Equals(".lnk", StringComparison.OrdinalIgnoreCase),
            //        //TargetPath = null
            //    };

            //    if (icon.IsShortcut)
            //    {
            //        //icon.FilePath
            //    }

            //    icons.Add(icon);
            //}

            //return icons;
        }
    }
}
