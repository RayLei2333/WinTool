﻿using System.Runtime.InteropServices;
using System.Text;
using Win32Support.Enums;
using Win32Support.Structs;

namespace Win32Support.Interfaces
{
    [ComImport, Guid("bcfce0a0-ec17-11d0-8d10-00a0c90f2719")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IContextMenu3
    {
        // Adds commands to a shortcut menu
        [PreserveSig()]
        int QueryContextMenu(
            nint hmenu,
            uint iMenu,
            uint idCmdFirst,
            uint idCmdLast,
            CMF uFlags);

        // Carries out the command associated with a shortcut menu item
        [PreserveSig()]
        int InvokeCommand(
            ref CMINVOKECOMMANDINFOEX info);

        // Retrieves information about a shortcut menu command, 
        // including the help string and the language-independent, 
        // or canonical, name for the command
        [PreserveSig()]
        int GetCommandString(
            uint idcmd,
            GCS uflags,
            uint reserved,
            [MarshalAs(UnmanagedType.LPWStr)]
                StringBuilder commandstring,
            int cch);

        // Allows client objects of the IContextMenu interface to 
        // handle messages associated with owner-drawn menu items
        [PreserveSig]
        int HandleMenuMsg(
            uint uMsg,
            nint wParam,
            nint lParam);

        // Allows client objects of the IContextMenu3 interface to 
        // handle messages associated with owner-drawn menu items
        [PreserveSig]
        int HandleMenuMsg2(
            uint uMsg,
            nint wParam,
            nint lParam,
            nint plResult);
    }
}
