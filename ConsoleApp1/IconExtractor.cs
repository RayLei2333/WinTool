using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class IconExtractor
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        public const uint SHGFI_ICON = 0x100;
        public const uint SHGFI_SYSICONINDEX = 0x4000;
        public const uint SHGFI_LARGEICON = 0x0;
        public const uint SHGFI_SMALLICON = 0x1;

        [DllImport("shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref SHFILEINFO psfi,
            uint cbFileInfo,
            uint uFlags
        );

        [DllImport("user32.dll")]
        public static extern bool DestroyIcon(IntPtr hIcon);

        public static int GetIconIndex(string fileName)
        {
            SHFILEINFO info = new SHFILEINFO();
            IntPtr iconIntPtr = SHGetFileInfo(fileName, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SYSICONINDEX | SHGFI_LARGEICON);
            if (iconIntPtr == IntPtr.Zero)
            {
                return -1;
            }
            return info.iIcon;
        }

        [ComImport, Guid("46EB5926-582E-4017-9FDF-E8998DAA0950"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IImageList
        {
            [PreserveSig]
            int Add(IntPtr hbmImage, IntPtr hbmMask, out uint pi);
            [PreserveSig]
            int ReplaceIcon(uint i, IntPtr hicon, out uint pi);
            [PreserveSig]
            int SetOverlayImage(uint iImage, uint iOverlay);
            [PreserveSig]
            int Replace(uint i, IntPtr hbmImage, IntPtr hbmMask);
            [PreserveSig]
            int AddMasked(IntPtr hbmImage, uint crMask, out uint pi);
            [PreserveSig]
            int Draw(ref IMAGELISTDRAWPARAMS pimldp);
            [PreserveSig]
            int Remove(uint i);
            [PreserveSig]
            int GetIcon(uint i, uint flags, out IntPtr picon);
            [PreserveSig]
            int GetImageInfo(uint i, out IMAGEINFO pImageInfo);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public IntPtr Unused1;
            public IntPtr Unused2;
            public Rectangle rcImage;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct IMAGELISTDRAWPARAMS
        {
            public uint cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;
            public int yBitmap;
            public COLORREF rgbBk;
            public COLORREF rgbFg;
            public uint fStyle;
            public uint dwRop;
            public uint fState;
            public uint rt;
            public RECT rc;
            public IntPtr hdcMask;
            public int xMasked;
            public int yMasked;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COLORREF
        {
            public uint colorref;
            public COLORREF(uint colorref)
            {
                this.colorref = colorref;
            }
        }

        [DllImport("shell32.dll")]
        public static extern int SHGetImageList(int iImageList, ref Guid riid, ref IImageList ppv);

        public enum SHIL_FLAGS
        {
            SHIL_SMALL = 0x0001,
            SHIL_LARGE = 0x0002,
            SHIL_EXTRALARGE = 0x0004,
            SHIL_JUMBO = 0x0005
        }

        //[DllImport("user32.dll")]
        //public static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon GetIcon(string fileName, SHIL_FLAGS sizeFlag)
        {
            SHFILEINFO info = new SHFILEINFO();
            IntPtr iconIntPtr = SHGetFileInfo(fileName, 0, ref info, (uint)Marshal.SizeOf(info), SHGFI_ICON | SHGFI_SYSICONINDEX | SHGFI_LARGEICON);
            if (iconIntPtr == IntPtr.Zero)
            {
                return null;
            }
            int iconIndex = info.iIcon;

            IImageList list = null;
            Guid theGuid = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");
            int hr = SHGetImageList((int)sizeFlag, ref theGuid, ref list);
            if (hr != 0)
            {
                return null; // SHGetImageList failed
            }

            uint flags = 0x00000001 | 0x00000020; // ILD_NORMAL | ILD_TRANSPARENT
            IntPtr hIcon = IntPtr.Zero;
            list.GetIcon((uint)iconIndex, flags, out hIcon);
            if (hIcon == IntPtr.Zero)
            {
                return null; // GetIcon failed
            }

            Icon icon = Icon.FromHandle(hIcon);
            return icon;
        }

    }
}
