using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Win32Support.Enums
{
    // A generalized global memory handle used for data transfer operations by the 
    // IAdviseSink, IDataObject, and IOleCache interfaces
    [StructLayout(LayoutKind.Sequential)]
    public struct STGMEDIUM
    {
        public TYMED tymed;
        public nint hBitmap;
        public nint hMetaFilePict;
        public nint hEnhMetaFile;
        public nint hGlobal;
        public nint lpszFileName;
        public nint pstm;
        public nint pstg;
        public nint pUnkForRelease;
    }
}
