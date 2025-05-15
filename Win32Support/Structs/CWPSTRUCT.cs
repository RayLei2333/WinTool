using System.Runtime.InteropServices;

namespace Win32Support.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CWPSTRUCT
    {
        public nint lparam;
        public nint wparam;
        public int message;
        public nint hwnd;
    }
}
