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
using static Desktop.Win32Support.ImageFileThumbnail;

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
        public static List<string> ImageFormats { get; set; } = new List<string>();

        static IconManager()
        {
            ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo encoder in imageEncoders)
            {
                string[] filenameExtension = encoder.FilenameExtension.Replace("*", "").ToLower().Split(";");
                ImageFormats.AddRange(filenameExtension);
            }
        }

        /// <summary>
        /// 图标的缓存
        /// </summary>
        private Dictionary<ViewType, List<IconInfo>> _iconListDic = new Dictionary<ViewType, List<IconInfo>>()
        {
            [ViewType.List] = new List<IconInfo>(),
            [ViewType.SmallIcon] = new List<IconInfo>(),
            [ViewType.LargeIcon] = new List<IconInfo>()
        };

        //默认后缀缓存
        private List<string> _defaultFileSuffixList = new List<string>();

        //存储后缀名称，完整路径信息，避免每次重复去获取
        //新增修改就检查suffixList
        private List<IconInfo> SuffixList { get; set; } = new List<IconInfo>();




        public void SetIcon(List<FileData> files)
        {

            SetFileSuffix(files);
            _defaultFileSuffixList.AddRange(ImageFormats);
            _defaultFileSuffixList = _defaultFileSuffixList.Distinct().ToList();
            //先获取默认图标
            foreach (var suffix in _defaultFileSuffixList)
            {
                ExtractorIcon(suffix);
            }

            //提取非默认图标，比如文件夹，lnk文件，图片
            foreach (var suffix in SuffixList)
            {
                //判断是否图片格式，如果是图片格式则去获取缩略图
                //如果缩略图获取失败了，再从缓存中去获取对应图标
                if (suffix.IsImageFile)
                {
                    ImageThumbnail(suffix);
                }
                else
                {
                    //获取对应图标
                    ExtractorIcon(suffix);
                }
            }
        }

        private void ExtractorIcon(string pszFile)
        {
            foreach (var item in _iconListDic)
            {
                IconInfo iconInfo = new IconInfo()
                {
                    Suffix = pszFile,
                    Icon =  ExtractorIcon(pszFile, item.Key, false)
                };

                item.Value.Add(iconInfo);
            }
        }

        private void ExtractorIcon(IconInfo suffix)
        {
            string pszFile = (suffix.IsFolder || suffix.IsLnkFile) ? suffix.FullPath : suffix.Suffix;
            foreach (var item in _iconListDic)
            {
                IconInfo iconInfo = suffix.Clone();
                ExtractorIcon(pszFile, item.Key, iconInfo.IsFolder);
                item.Value.Add(iconInfo);
            }
        }

        private ImageSource ExtractorIcon(string pszFile, ViewType viewType, bool chekcDisk)
        {
            switch (viewType)
            {
                case ViewType.List:
                    return GetIcon(IconExtractor.GetIcon16(pszFile, chekcDisk));
                case ViewType.SmallIcon:
                    return GetIcon(IconExtractor.GetIcon32(pszFile, chekcDisk));
                case ViewType.LargeIcon:
                    return GetIcon(IconExtractor.GetIcon48(pszFile, chekcDisk));
            }

            return null;
        }

        private void ImageThumbnail(IconInfo suffix)
        {
            //获取缩略图
            foreach (var item in _iconListDic)
            {
                IconInfo iconInfo = suffix.Clone();
                switch (item.Key)
                {
                    //图片列表模式则直接使用小icon
                    case ViewType.SmallIcon:
                        iconInfo.Icon = ImageThumbnail(iconInfo.FullPath, 32, 32);
                        break;
                    case ViewType.LargeIcon:
                        iconInfo.Icon = ImageThumbnail(iconInfo.FullPath, 48, 48);
                        break;
                }

                //如果获取到图片的缩略图是空的，就从缓存中去取一个
                if (iconInfo.Icon == null)
                    iconInfo.Icon = _iconListDic[item.Key].FirstOrDefault(t => t.Suffix == iconInfo.Suffix)?.Icon;
                item.Value.Add(iconInfo);

            }
        }

        private ImageSource ImageThumbnail(string filePath, int width, int height)
        {
            try
            {
                Guid guid = typeof(IShellItemImageFactory).GUID;
                SHCreateItemFromParsingName(filePath, IntPtr.Zero, ref guid, out IShellItemImageFactory imageFactory);
                SIZE size;
                size.cx = width;
                size.cy = height;
                //异常System.Runtime.InteropServices.COMException:“0x8004B200”

                imageFactory.GetImage(size, SIIGBF.SIIGBF_RESIZETOFIT | SIIGBF.SIIGBF_THUMBNAILONLY, out IntPtr hBitmap);
                using (Bitmap bmp = Image.FromHbitmap(hBitmap))
                {
                    // 清理
                    DeleteObject(hBitmap);
                    return GetIcon(bmp);
                }
            }
            catch (Exception ex)
            {
                return null;
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

        public static ImageSource GetIcon(Icon icon)
        {
            using (var bitmap = icon.ToBitmap())
            {
                return GetIcon(bitmap);
            }
        }

        public static ImageSource GetIcon(Bitmap bitmap)
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
