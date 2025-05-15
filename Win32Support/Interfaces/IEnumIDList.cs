using System.Runtime.InteropServices;

namespace Win32Support.Interfaces
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F2-0000-0000-C000-000000000046")]
    public interface IEnumIDList
    {
        [PreserveSig]
        int Next(uint celt, out IntPtr rgelt, out uint pceltFetched);
        void Skip(uint celt);
        void Reset();
        void Clone(out IEnumIDList ppenum);
    }

}
