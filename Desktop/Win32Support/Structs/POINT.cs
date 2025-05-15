using System.Runtime.InteropServices;

namespace Desktop.Win32Support.Structs
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public POINT() { }

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X, Y;
    }

}
