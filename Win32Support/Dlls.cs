using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Win32Support.Enums;
using Win32Support.Interfaces;
using Win32Support.Structs;

namespace Win32Support
{
    public class Dlls
    {
        //public delegate int HookProc(int code, nint wParam, nint lParam);

        [DllImport("user32.dll")]
        public static extern nint FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern nint FindWindowEx(nint hwndParent, nint hwndChildAfter, string lpszClass, string lpszWindow);


        // The TrackPopupMenuEx function displays a shortcut menu at the specified location and tracks the selection of items on the shortcut menu. The shortcut menu can appear anywhere on the screen.
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern uint TrackPopupMenuEx(nint hmenu, TPM flags, int x, int y, nint hwnd, nint lptpm);

        // Retrieves the IShellFolder interface for the desktop folder, which is the root of the Shell's namespace.
        [DllImport("shell32.dll")]
        public static extern int SHGetDesktopFolder(out nint ppshf);

        // Takes a STRRET structure returned by IShellFolder::GetDisplayNameOf, converts it to a string, and places the result in a buffer. 
        [DllImport("shlwapi.dll", EntryPoint = "StrRetToBuf", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int StrRetToBuf(nint pstr, nint pidl, StringBuilder pszBuf, int cchBuf);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrRetToBuf(ref STRRET pstr, IntPtr pidl, StringBuilder pszBuf, uint cchBuf);

        // The CreatePopupMenu function creates a drop-down menu, submenu, or shortcut menu. The menu is initially empty. You can insert or append menu items by using the InsertMenuItem function. You can also use the InsertMenu function to insert menu items and the AppendMenu function to append menu items.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern nint CreatePopupMenu();

        // The DestroyMenu function destroys the specified menu and frees any memory that the menu occupies.
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool DestroyMenu(nint hMenu);

        // Determines the default menu item on the specified menu
        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int GetMenuDefaultItem(nint hMenu, bool fByPos, uint gmdiFlags);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(nint hWnd, uint Msg, nint wParam, nint lParam);



        // ************************************************************************
        // Win32: SetWindowsHookEx()
        [DllImport("user32.dll")]
        public static extern nint SetWindowsHookEx(HookType code, LocalWindowsHook.HookProc func, nint hInstance, int threadID);
        // ************************************************************************

        // ************************************************************************
        // Win32: UnhookWindowsHookEx()
        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(nint hhook);
        // ************************************************************************

        // ************************************************************************
        // Win32: CallNextHookEx()
        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(nint hhook,
            int code, nint wParam, nint lParam);
        // ************************************************************************

        [DllImport("shell32.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        public static extern void SHCreateItemFromParsingName(
           [In][MarshalAs(UnmanagedType.LPWStr)] string pszPath,
           IntPtr pbc,
           [In] ref Guid riid,
           [Out][MarshalAs(UnmanagedType.Interface)] out IShellItem shellItem);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);


        [DllImport("user32.dll", EntryPoint = "GetDesktopWindow", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint GetDesktopWindow();

        [DllImport("user32.dll", EntryPoint = "SetParent")]
        public static extern int SetParent(nint hWndChild, nint hWndNewParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, StringBuilder lpvParam, int fuWinIni);

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("user32.dll", SetLastError = true)]
        //public static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, ref LVITEM lParam);

        [DllImport("user32.dll")]
        public static extern uint GetDpiForWindow(IntPtr hwnd);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);

        
    }
}
