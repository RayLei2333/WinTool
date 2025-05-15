using System.Runtime.InteropServices;
using Win32Support.Enums;

namespace Win32Support.Structs
{
    // Contains extended information about a shortcut menu command
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct CMINVOKECOMMANDINFOEX
    {
        public int cbSize;
        public CMIC fMask;
        public nint hwnd;
        public nint lpVerb;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpParameters;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpDirectory;
        public SW nShow;
        public int dwHotKey;
        public nint hIcon;
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpTitle;
        public nint lpVerbW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpParametersW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpDirectoryW;
        [MarshalAs(UnmanagedType.LPWStr)]
        public string lpTitleW;
        public POINT ptInvoke;
    }
}
