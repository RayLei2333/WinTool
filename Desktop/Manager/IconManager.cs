using Desktop.Models;
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
using Desktop.Win32Support;

namespace Desktop.Manager
{
    /// <summary>
    /// 图标管理器
    /// </summary>
    public class IconManager
    {
        private static IconManager _instence = new IconManager();

        public static IconManager Instence => _instence;

        /// <summary>
        /// 所有图片格式的缓存
        /// </summary>
        public static List<string> ImageFormats { get; set; }


        /// <summary>
        /// 图标的缓存
        /// </summary>
        private Dictionary<ViewType, List<IconInfo>> _iconListDic = new Dictionary<ViewType, List<IconInfo>>()
        {
            [ViewType.List] = new List<IconInfo>(),
            [ViewType.SmallIcon] = new List<IconInfo>(),
            [ViewType.LargeIcon] = new List<IconInfo>(),
            [ViewType.Bigg] = new List<IconInfo>(),
            [ViewType.BIGGGG] = new List<IconInfo>()
        };

        //默认后缀缓存
        private List<string> _defaultFileSuffixList = new List<string>();

        //存储后缀名称，完整路径信息，避免每次重复去获取
        //新增修改就检查suffixList
        private List<IconInfo> SuffixList { get; set; } = new List<IconInfo>();

        public IconManager()
        {
            ImageFormats = new List<string>();
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in imageEncoders)
            {
                string[] filenameExtension = encoder.FilenameExtension.Replace("*", "").ToLower().Split(";");
                ImageFormats.AddRange(filenameExtension);
            }
            _defaultFileSuffixList.AddRange(ImageFormats);
        }



        public void SetIcon(List<FileData> files)
        {

            SetFileSuffix(files);
            //先获取默认图标
            foreach (var suffix in _defaultFileSuffixList)
            {
                ExtractorIcon(suffix);
            }

            //提取非默认图标，比如文件夹，lnk文件，图片
            foreach (var suffix in SuffixList)
            {
                ExtractorIcon(suffix);
            }
        }

        private void ExtractorIcon(string suffix)
        {
            foreach (var item in _iconListDic)
            {
                IconInfo iconInfo = new IconInfo()
                {
                    Suffix = suffix,
                    Icon = IconExtractor.GetIconBitmap(suffix, (int)item.Key)
                };
                item.Value.Add(iconInfo);
            }
        }

        private void ExtractorIcon(IconInfo suffix)
        {
            foreach (var item in _iconListDic)
            {
                IconInfo iconInfo = suffix.Clone();
                iconInfo.Icon = IconExtractor.GetIconBitmap(iconInfo.FullPath, (int)item.Key);
                item.Value.Add(iconInfo);
            }
        }

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

            if (pszFile.StartsWith("::"))
            {
                return FindIconByExtension(list, pszFile);
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
            //1、先从files中获取所有文件后缀，需要排除IsFolder和IsLnkFile为true的数据
            var tmpList = files.Where(t => !t.IsFolder &&
                                           !t.IsLnkFile &&
                                           !t.IsImageFile &&
                                           !_defaultFileSuffixList.Any(c => c == t.Suffix)).Select(t => t.Suffix).Distinct().ToList();
            _defaultFileSuffixList.AddRange(tmpList);

            //2、具体要获取的完整的路径文件，包含Folder和LnkFile
            foreach (var file in files)
            {
                if (file.IsFolder || file.IsLnkFile || file.IsImageFile)
                {
                    if (!SuffixList.Any(t => t.FullPath == file.FullPath))
                    {
                        SuffixList.Add(new IconInfo()
                        {
                            IsFolder = file.IsFolder,
                            FullPath = file.FullPath,
                            Suffix = file.Suffix,
                            IsLnkFile = file.IsLnkFile,
                            IsImageFile = file.IsImageFile,
                        });
                    }
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
