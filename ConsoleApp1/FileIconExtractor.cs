using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class FileIconExtractor
    {
        // 定义 SHFILEINFO 结构
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        // 定义常量
        private const uint SHGFI_ICON = 0x100;
        private const uint SHGFI_LARGEICON = 0x0;
        private const uint SHGFI_SMALLICON = 0x1;
        private const uint SHGFI_OPENICON = 0x2;
        private const uint SHGFI_SYSICONINDEX = 0x4000;
        private const uint SHGFI_LINKOVERLAY = 0x8000;  //带快捷方式图标
        private const uint SHGFI_SELECTED = 0x10000;    //选中后图标
        private const uint SHGFI_ATTR_SPECIFIED = 0x20000;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x10;
        private const uint SHIL_JUMBO = 0X4; //{256x256}


        // 导入 SHGetFileInfo 函数
        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
        );

        // 导入 DestroyIcon 函数
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        // 获取文件的大图标
        public static Icon GetLargeIcon(string filePath)
        {
            SHFILEINFO shfileinfo = new SHFILEINFO();
            IntPtr hImgLarge = SHGetFileInfo(
                filePath,
                0,
                ref shfileinfo,
                (uint)Marshal.SizeOf(shfileinfo),
                SHGFI_ICON | SHGFI_LARGEICON
            );
            Icon icon = (Icon)Icon.FromHandle(shfileinfo.hIcon).Clone();
            DestroyIcon(shfileinfo.hIcon);
            return icon;
        }

        // 获取文件的小图标
        public static Icon GetSmallIcon(string filePath)
        {
            SHFILEINFO shfileinfo = new SHFILEINFO();
            IntPtr hImgSmall = SHGetFileInfo(
                filePath,
                0,
                ref shfileinfo,
                (uint)Marshal.SizeOf(shfileinfo),
                SHGFI_ICON | SHGFI_SMALLICON
            );
            Icon icon = (Icon)Icon.FromHandle(shfileinfo.hIcon).Clone();
            DestroyIcon(shfileinfo.hIcon);
            return icon;
        }


        // 获取文件的小图标
        public static Icon GetJubmoIcon(string filePath)
        {
            SHFILEINFO shfileinfo = new SHFILEINFO();
            IntPtr hImgSmall = SHGetFileInfo(
                filePath,
                0,
                ref shfileinfo,
                (uint)Marshal.SizeOf(shfileinfo),
                SHGFI_ICON | SHGFI_USEFILEATTRIBUTES
            );
            Icon icon = (Icon)Icon.FromHandle(shfileinfo.hIcon).Clone();
            DestroyIcon(shfileinfo.hIcon);
            return icon;
        }
    }
}
