using Desktop.Models;
using Desktop.Win32Support;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Desktop.Manager
{
    /// <summary>
    /// 图标管理器
    /// </summary>
    public class IconManager
    {
        private static IconManager _instence = new IconManager();

        public static IconManager Instence => _instence;

        private Dictionary<ViewType, List<IconInfo>> _iconListDic = new Dictionary<ViewType, List<IconInfo>>()
        {
            [ViewType.List] = new List<IconInfo>(),
            [ViewType.SmallIcon] = new List<IconInfo>(),
            [ViewType.LargeIcon] = new List<IconInfo>()
        };

        private List<FileHandler> SuffixList { get; set; } = new List<FileHandler>();


        public void SetIcon(List<FileData> files)
        {

            SetFileSuffix(files);
            foreach (var suffix in SuffixList)
            {
                string pszFile = (suffix.IsFolder || suffix.IsLnkFile) ? suffix.FullPath : suffix.Suffix;
                foreach (var item in _iconListDic)
                {
                    IconInfo iconInfo = new IconInfo(suffix);
                    switch (item.Key)
                    {
                        case ViewType.List:
                            iconInfo.Icon = GetIcon(IconExtractor.GetIcon16(pszFile, iconInfo.IsFolder));
                            break;
                        case ViewType.SmallIcon:
                            iconInfo.Icon = GetIcon(IconExtractor.GetIcon32(pszFile, iconInfo.IsFolder));
                            break;
                        case ViewType.LargeIcon:
                            iconInfo.Icon = GetIcon(IconExtractor.GetIcon48(pszFile, iconInfo.IsFolder));
                            break;
                    }
                    item.Value.Add(iconInfo);
                }
            }
        }



        //public List<IconInfo> GetIcon(ViewType viewType)
        //{
        //    return _iconListDic[viewType];
        //}




        public IconInfo GetIcon(ViewType viewType, string pszFile)
        {
            //先判断pszFile是否为路径，如果是路径判断是否为文件夹
            //如果是文件夹则根据t.FileSuffixOrFoldderPath == pszFile 取出文件夹图标
            //如果是文件，则取出后缀去匹配图标
            //如果不是文件和文件夹，那么就默认他是文件后缀
            var list = _iconListDic[viewType];
            // 处理直接传入扩展名的情况
            if (pszFile.StartsWith("."))
            {
                return pszFile.CheckIsLnkFileSuffix()
                    ? null
                    : FindIconByExtension(list, pszFile);
            }

            // 处理完整路径情况
            if (Directory.Exists(pszFile))
            {
                return FindIconByFolder(list, pszFile);
            }

            if (File.Exists(pszFile))
            {
                string extension = Path.GetExtension(pszFile)?.ToLower() ?? string.Empty;
                if (extension.CheckIsLnkFileSuffix())
                {
                    return FindIconByFile(list, pszFile);
                }
                else
                {
                    return FindIconByExtension(list, extension);
                }
            }

            // 处理既不是完整路径也不是扩展名的情况
            return null;

        }

        public IconInfo GteIcon(ViewType viewType, FileData file)
        {
            var list = _iconListDic[viewType];
            if (file.IsFolder)
                return FindIconByFolder(list, file.FullPath);

            if (file.IsLnkFile)
                return FindIconByFile(list, file.FullPath);

            return FindIconByExtension(list, file.Suffix);
        }

        private void SetFileSuffix(List<FileData> files)
        {
            foreach (var file in files)
            {
                if (file.IsFolder || file.IsLnkFile)
                {
                    if (!SuffixList.Any(t => t.FullPath == file.FullPath))
                    {
                        SuffixList.Add(new FileHandler()
                        {
                            IsFolder = file.IsFolder,
                            FullPath = file.FullPath,
                            Suffix = file.Suffix,
                            IsLnkFile = file.IsLnkFile
                        });
                    }
                }
                if (!SuffixList.Any(t => t.Suffix == file.Suffix))
                {
                    SuffixList.Add(new FileHandler()
                    {
                        Suffix = file.Suffix,
                    });
                }
            }
        }

        public static ImageSource GetIcon(Icon icon)
        {
            using (var bitmap = icon.ToBitmap())
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

        private IconInfo FindIconByExtension(ICollection<IconInfo> iconList, string extension)
        {
            return iconList.FirstOrDefault(t => string.Equals(t.Suffix, extension, StringComparison.OrdinalIgnoreCase));
        }

        private IconInfo FindIconByFolder(ICollection<IconInfo> iconList, string folderPath)
        {
            return iconList.FirstOrDefault(t => string.Equals(t.FullPath, folderPath, StringComparison.OrdinalIgnoreCase));
        }

        private IconInfo FindIconByFile(ICollection<IconInfo> iconList, string filePath)
        {
            return iconList.FirstOrDefault(t => string.Equals(t.FullPath, filePath, StringComparison.OrdinalIgnoreCase));
        }
    }
}
