using System.Runtime.InteropServices;

namespace Win32Support.Enums
{
    [StructLayout(LayoutKind.Explicit, Size = 520)]
    public struct STRRET
    {
        [FieldOffset(0)] 
        public uint uType;

        [FieldOffset(4)] 
        public IntPtr pOleStr;
    }
}
