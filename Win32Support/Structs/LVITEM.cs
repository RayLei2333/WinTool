using System.Runtime.InteropServices;

namespace Win32Support.Structs
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct LVITEM
    {
        public uint mask;
        public int iItem;
        public int iSubItem;
        public int state;
        public int stateMask;
        public nint pszText;
        public int cchTextMax;
        public int iImage;
        public nint lParam;
    }
}
