using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1
{
    public class ShellContextMenu
    {
        [DllImport("shell32.dll")]
        private static extern IntPtr SHGetFileInfo(
       string pszPath,
       uint dwFileAttributes,
       out SHFILEINFO psfi,
       uint cbFileInfo,
       uint uFlags
   );

        [DllImport("user32.dll")]
        private static extern IntPtr TrackPopupMenuEx(IntPtr hMenu, uint uFlags, int x, int y, IntPtr hWnd, IntPtr lpReserved);

        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        private const uint SHGFI_PIDL = 0x00000008;
        private const uint SHGFI_USEFILEATTRIBUTES = 0x00000010;
        private const uint SHGFI_ICON = 0x000000100;
        private const uint SHGFI_LARGEICON = 0x00000000;
        private const uint SHGFI_SMALLICON = 0x00000001;
        private const uint TPM_LEFTALIGN = 0x0000;
        private const uint TPM_RIGHTBUTTON = 0x0002;

        public void ShowContextMenu(string[] filePaths, Point point)
        {
            SHFILEINFO shfi = new SHFILEINFO();
            IntPtr hMenu = IntPtr.Zero;
            try
            {
                //AppDomain.CurrentDomain.
                foreach (string filePath in filePaths)
                {
                    SHGetFileInfo(filePath, 0, out shfi, (uint)Marshal.SizeOf(shfi), SHGFI_PIDL | SHGFI_USEFILEATTRIBUTES);
                    //TrackPopupMenuEx(shfi.hIcon, TPM_LEFTALIGN | TPM_RIGHTBUTTON, (int)point.X, (int)point.Y, new WindowInteropHelper(Application.Current.MainWindow).Handle, IntPtr.Zero);
                    TrackPopupMenuEx(shfi.hIcon, TPM_LEFTALIGN | TPM_RIGHTBUTTON, (int)point.X, (int)point.Y, IntPtr.Zero, IntPtr.Zero);
                }
            }
            finally
            {
                if (hMenu != IntPtr.Zero)
                {
                    // Release the menu handle if needed
                }
            }
        }
    }
}
