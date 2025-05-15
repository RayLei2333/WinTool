using System.Runtime.InteropServices;
using Win32Support.Enums;

namespace Win32Support.Interfaces
{
    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214E6-0000-0000-C000-000000000046")]
    public interface IShellFolder
    {
        // Translates a file object's or folder's display name into an item identifier list.
        // Return value: error code, if any
        [PreserveSig]
        int ParseDisplayName(
            nint hwnd,
            nint pbc,
            [MarshalAs(UnmanagedType.LPWStr)]
                string pszDisplayName,
            ref uint pchEaten,
            out nint ppidl,
            ref SFGAO pdwAttributes);

        // Allows a client to determine the contents of a folder by creating an item
        // identifier enumeration object and returning its IEnumIDList interface.
        // Return value: error code, if any
        //[PreserveSig]
        //int EnumObjects(
        //    nint hwnd,
        //    SHCONTF grfFlags,
        //    out nint enumIDList);

        [PreserveSig]
        void EnumObjects(IntPtr hwnd, SHCONTF grfFlags, out IEnumIDList ppenumIDList);

        // Retrieves an IShellFolder object for a subfolder.
        // Return value: error code, if any
        [PreserveSig]
        int BindToObject(
            nint pidl,
            nint pbc,
            ref Guid riid,
            out nint ppv);

        // Requests a pointer to an object's storage interface. 
        // Return value: error code, if any
        [PreserveSig]
        int BindToStorage(
            nint pidl,
            nint pbc,
            ref Guid riid,
            out nint ppv);

        // Determines the relative order of two file objects or folders, given their
        // item identifier lists. Return value: If this method is successful, the
        // CODE field of the HRESULT contains one of the following values (the code
        // can be retrived using the helper function GetHResultCode): Negative A
        // negative return value indicates that the first item should precede
        // the second (pidl1 < pidl2). 

        // Positive A positive return value indicates that the first item should
        // follow the second (pidl1 > pidl2).  Zero A return value of zero
        // indicates that the two items are the same (pidl1 = pidl2). 
        [PreserveSig]
        int CompareIDs(
            nint lParam,
            nint pidl1,
            nint pidl2);

        // Requests an object that can be used to obtain information from or interact
        // with a folder object.
        // Return value: error code, if any
        [PreserveSig]
        int CreateViewObject(
            nint hwndOwner,
            Guid riid,
            out nint ppv);

        // Retrieves the attributes of one or more file objects or subfolders. 
        // Return value: error code, if any
        [PreserveSig]
        int GetAttributesOf(
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
                nint[] apidl,
            ref SFGAO rgfInOut);

        // Retrieves an OLE interface that can be used to carry out actions on the
        // specified file objects or folders.
        // Return value: error code, if any
        [PreserveSig]
        int GetUIObjectOf(
            nint hwndOwner,
            uint cidl,
            [MarshalAs(UnmanagedType.LPArray)]
                nint[] apidl,
            ref Guid riid,
            nint rgfReserved,
            out nint ppv);

        // Retrieves the display name for the specified file object or subfolder. 
        // Return value: error code, if any
        //[PreserveSig()]
        int GetDisplayNameOf(
            nint pidl,
            SHGNO uFlags,
            nint lpName);

        //[PreserveSig]
        void GetDisplayNameOf(IntPtr pidl, SHGNO uFlags, out STRRET pName);

        // Sets the display name of a file object or subfolder, changing the item
        // identifier in the process.
        // Return value: error code, if any
        [PreserveSig]
        int SetNameOf(
            nint hwnd,
            nint pidl,
            [MarshalAs(UnmanagedType.LPWStr)]
                string pszName,
            SHGNO uFlags,
            out nint ppidlOut);
    }
}
