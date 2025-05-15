using System.Runtime.InteropServices;

namespace Desktop.Win32Support.Enums
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SIZE
    {
        public int cx;
        public int cy;
    }
}
