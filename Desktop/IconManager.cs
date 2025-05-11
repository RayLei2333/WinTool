using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Desktop
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

        private List<FileHandler> SuffixList { get; set; }

        public void SetIcon(List<string> filePaths)
        {
            SuffixList = GetFileSuffix(filePaths);
            foreach (var item in SuffixList)
            {
                //如果是图像  则去读整个图像
                //如果图像太大如何处理
                if (item.Suffix.CheckIsImgFileSuffix())
                {

                }
                else
                {
                    string pszFile = string.Empty;
                    if (item.Suffix.CheckIsLnkFileSuffix())
                        pszFile = item.FullPath;
                    else
                        pszFile = item.IsFolder ? item.FullPath : item.Suffix;

                    _iconListDic[ViewType.List].Add(new IconInfo(item.IsFolder, pszFile, GetIcon(IconExtractor.GetIcon16(pszFile, item.IsFolder))));
                    _iconListDic[ViewType.SmallIcon].Add(new IconInfo(item.IsFolder, pszFile, GetIcon(IconExtractor.GetIcon32(pszFile, item.IsFolder))));
                    _iconListDic[ViewType.LargeIcon].Add(new IconInfo(item.IsFolder, pszFile, GetIcon(IconExtractor.GetIcon48(pszFile, item.IsFolder))));
                }

              
            }
        }

        public List<IconInfo> GetIcon(ViewType viewType)
        {
            return _iconListDic[viewType];
        }

        public IconInfo GetIcon(ViewType viewType, string pszFile)
        {
            //先判断pszFile是否为路径，如果是路径判断是否为文件夹
            //如果是文件夹则根据t.FileSuffixOrFoldderPath == pszFile 取出文件夹图标
            //如果是文件，则取出后缀去匹配图标
            //如果不是文件和文件夹，那么就默认他是文件后缀
            string matchRule = pszFile;
            if (File.Exists(pszFile))
            {
                string extension = Path.GetExtension(pszFile).ToLower();
                if (!extension.CheckIsLnkFileSuffix())
                    matchRule = extension;
            }
            return _iconListDic[viewType].FirstOrDefault(t => string.Equals(t.FileSuffixOrFoldderPath, matchRule));
        }

        public List<IconInfo> GetIcon(ViewType viewType, IEnumerable<string> pszFiles)
        {
            return _iconListDic[viewType].Where(t => pszFiles.Contains(t.FileSuffixOrFoldderPath)).ToList();
        }

        private List<FileHandler> GetFileSuffix(List<string> filePaths)
        {
            List<FileHandler> list = new List<FileHandler>();
            foreach (var item in filePaths)
            {
                bool isFolder = false;
                string suffix = string.Empty;
                if (Directory.Exists(item))
                {
                    isFolder = true;
                    suffix = "folder";
                }
                else if (File.Exists(item))
                {
                    string extension = Path.GetExtension(item).ToLower();
                    if (!extension.CheckIsLnkFileSuffix() && list.Any(t => string.Equals(t.Suffix, extension)))
                        continue;
                    suffix = extension;
                }
                else
                {
                    suffix = "invalid path";
                }

                list.Add(new FileHandler()
                {
                    FullPath = item,
                    IsFolder = isFolder,
                    Suffix = suffix,
                });
            }


            return list;
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
    }
}
