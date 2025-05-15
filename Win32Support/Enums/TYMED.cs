using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32Support.Enums
{
    // Indicates the type of storage medium being used in a data transfer
    [Flags]
    public enum TYMED
    {
        ENHMF = 0x40,
        FILE = 2,
        GDI = 0x10,
        HGLOBAL = 1,
        ISTORAGE = 8,
        ISTREAM = 4,
        MFPICT = 0x20,
        NULL = 0
    }
}
