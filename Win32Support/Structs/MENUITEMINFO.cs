using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Win32Support.Enums;

namespace Win32Support.Structs
{
    // Contains information about a menu item
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct MENUITEMINFO
    {
        /// <summary>
        /// The size, in bytes, of a MENUITEMINFO structure. The caller must set this member to sizeof(MENUITEMINFO).
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cbsize"> Marshal.SizeOf(typeof(MENUITEMINFO));</param>
        public MENUITEMINFO(string text,int cbsize)
        {
            cbSize = cbsize;
            dwTypeData = text;
            cch = text.Length;
            fMask = 0;
            fType = 0;
            fState = 0;
            wID = 0;
            hSubMenu = nint.Zero;
            hbmpChecked = nint.Zero;
            hbmpUnchecked = nint.Zero;
            dwItemData = nint.Zero;
            hbmpItem = nint.Zero;
        }

        public int cbSize;
        public MIIM fMask;
        public MFT fType;
        public MFS fState;
        public uint wID;
        public nint hSubMenu;
        public nint hbmpChecked;
        public nint hbmpUnchecked;
        public nint dwItemData;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string dwTypeData;
        public int cch;
        public nint hbmpItem;
    }
}
