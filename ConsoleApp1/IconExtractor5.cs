﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ConsoleApp1.IconExtractor;

namespace ConsoleApp1
{
    internal class IconExtractor5
    {
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        private struct POINT
        {
            public int x;
            public int y;
        }

        // Constants that we need in the function call

        private const int SHGFI_ICON = 0x100;

        private const int SHGFI_SMALLICON = 0x1;

        private const int SHGFI_LARGEICON = 0x0;

        private const int SHIL_JUMBO = 0x4;
        private const int SHIL_EXTRALARGE = 0x2;

        // This structure will contain information about the file

        public struct SHFILEINFO
        {

            // Handle to the icon representing the file

            public IntPtr hIcon;

            // Index of the icon within the image list

            public int iIcon;

            // Various attributes of the file

            public uint dwAttributes;

            // Path to the file

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]

            public string szDisplayName;

            // File type

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]

            public string szTypeName;

        };

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern Boolean CloseHandle(IntPtr handle);

        private struct IMAGELISTDRAWPARAMS
        {
            public int cbSize;
            public IntPtr himl;
            public int i;
            public IntPtr hdcDst;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public int xBitmap;        // x offest from the upperleft of bitmap
            public int yBitmap;        // y offset from the upperleft of bitmap
            public int rgbBk;
            public int rgbFg;
            public int fStyle;
            public int dwRop;
            public int fState;
            public int Frame;
            public int crEffect;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct IMAGEINFO
        {
            public IntPtr hbmImage;
            public IntPtr hbmMask;
            public int Unused1;
            public int Unused2;
            public RECT rcImage;
        }

        #region Private ImageList COM Interop (XP)
        [ComImportAttribute()]
        [GuidAttribute("46EB5926-582E-4017-9FDF-E8998DAA0950")]
        [InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        //helpstring("Image List"),
        interface IImageList
        {
            [PreserveSig]
            int Add(
                IntPtr hbmImage,
                IntPtr hbmMask,
                ref int pi);

            [PreserveSig]
            int ReplaceIcon(
                int i,
                IntPtr hicon,
                ref int pi);

            [PreserveSig]
            int SetOverlayImage(
                int iImage,
                int iOverlay);

            [PreserveSig]
            int Replace(
                int i,
                IntPtr hbmImage,
                IntPtr hbmMask);

            [PreserveSig]
            int AddMasked(
                IntPtr hbmImage,
                int crMask,
                ref int pi);

            [PreserveSig]
            int Draw(
                ref IMAGELISTDRAWPARAMS pimldp);

            [PreserveSig]
            int Remove(
            int i);

            [PreserveSig]
            int GetIcon(
                int i,
                int flags,
                ref IntPtr picon);

            [PreserveSig]
            int GetImageInfo(
                int i,
                ref IMAGEINFO pImageInfo);

            [PreserveSig]
            int Copy(
                int iDst,
                IImageList punkSrc,
                int iSrc,
                int uFlags);

            [PreserveSig]
            int Merge(
                int i1,
                IImageList punk2,
                int i2,
                int dx,
                int dy,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int Clone(
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetImageRect(
                int i,
                ref RECT prc);

            [PreserveSig]
            int GetIconSize(
                ref int cx,
                ref int cy);

            [PreserveSig]
            int SetIconSize(
                int cx,
                int cy);

            [PreserveSig]
            int GetImageCount(
            ref int pi);

            [PreserveSig]
            int SetImageCount(
                int uNewCount);

            [PreserveSig]
            int SetBkColor(
                int clrBk,
                ref int pclr);

            [PreserveSig]
            int GetBkColor(
                ref int pclr);

            [PreserveSig]
            int BeginDrag(
                int iTrack,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int EndDrag();

            [PreserveSig]
            int DragEnter(
                IntPtr hwndLock,
                int x,
                int y);

            [PreserveSig]
            int DragLeave(
                IntPtr hwndLock);

            [PreserveSig]
            int DragMove(
                int x,
                int y);

            [PreserveSig]
            int SetDragCursorImage(
                ref IImageList punk,
                int iDrag,
                int dxHotspot,
                int dyHotspot);

            [PreserveSig]
            int DragShowNolock(
                int fShow);

            [PreserveSig]
            int GetDragImage(
                ref POINT ppt,
                ref POINT pptHotspot,
                ref Guid riid,
                ref IntPtr ppv);

            [PreserveSig]
            int GetItemFlags(
                int i,
                ref int dwFlags);

            [PreserveSig]
            int GetOverlayImage(
                int iOverlay,
                ref int piIndex);
        };
        #endregion

        ///
        /// SHGetImageList is not exported correctly in XP.  See KB316931
        /// http://support.microsoft.com/default.aspx?scid=kb;EN-US;Q316931
        /// Apparently (and hopefully) ordinal 727 isn't going to change.
        ///
        [DllImport("shell32.dll", EntryPoint = "#727")]
        private extern static int SHGetImageList(
            int iImageList,
            ref Guid riid,
            out IImageList ppv
            );

        // The signature of SHGetFileInfo (located in Shell32.dll)
        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport("Shell32.dll")]
        public static extern int SHGetFileInfo(IntPtr pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, uint uFlags);

        [DllImport("shell32.dll", SetLastError = true)]
        static extern int SHGetSpecialFolderLocation(IntPtr hwndOwner, Int32 nFolder,
                 ref IntPtr ppidl);

        [DllImport("user32")]
        public static extern int DestroyIcon(IntPtr hIcon);

        public struct pair
        {
            public System.Drawing.Icon icon { get; set; }
            public IntPtr iconHandleToDestroy { set; get; }

        }

        public static int DestroyIcon2(IntPtr hIcon)
        {
            return DestroyIcon(hIcon);
        }


        public static Icon icon_of_path_large(string FileName, bool jumbo, bool checkDisk)
        {

            SHFILEINFO shinfo = new SHFILEINFO();

            uint SHGFI_USEFILEATTRIBUTES = 0x000000010;
            uint SHGFI_SYSICONINDEX = 0x4000;

            int FILE_ATTRIBUTE_NORMAL = 0x80;

            uint flags;
            flags = SHGFI_SYSICONINDEX;

            if (!checkDisk)  // This does not seem to work. If I try it, a folder icon is always returned.
                flags |= SHGFI_USEFILEATTRIBUTES;

            //var res = SHGetFileInfo(FileName, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), flags);
            var res = SHGetFileInfo(FileName, FILE_ATTRIBUTE_NORMAL, ref shinfo, Marshal.SizeOf(shinfo), flags);
            if (res == 0)
            {
                throw (new System.IO.FileNotFoundException());
            }
            var iconIndex = shinfo.iIcon;

            // Get the System IImageList object from the Shell:
            Guid iidImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

            IImageList iml;
            int size = jumbo ? SHIL_JUMBO : SHIL_EXTRALARGE;
            var hres = SHGetImageList(size, ref iidImageList, out iml); // writes iml
            //if (hres == 0)
            //{
            //    throw (new System.Exception("Error SHGetImageList"));
            //}

            IntPtr hIcon = IntPtr.Zero;
            int ILD_TRANSPARENT = 1;
            hres = iml.GetIcon(iconIndex, ILD_TRANSPARENT, ref hIcon);
            //if (hres == 0)
            //{
            //    throw (new System.Exception("Error iml.GetIcon"));
            //}

            var myIcon = System.Drawing.Icon.FromHandle(hIcon);
            //var bs = bitmap_source_of_icon(myIcon);
            //myIcon.Dispose();
            //bs.Freeze(); // very important to avoid memory leak
            //DestroyIcon(hIcon);
            //CloseHandle(hIcon);

            return myIcon;

        }
    }
}
