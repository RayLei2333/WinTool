using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;
using Win32Support.Enums;
using Win32Support.Interfaces;
using Win32Support.Structs;

namespace Win32Support
{
    public class ShellContextMenu : HwndHost
    {
        #region Local variabled
        private IContextMenu _oContextMenu;
        private IContextMenu2 _oContextMenu2;
        private IContextMenu3 _oContextMenu3;
        private IShellFolder _oDesktopFolder;
        private IShellFolder _oParentFolder;
        private nint[] _arrPIDLs;
        private string _strParentFolder;
        private Dictionary<uint, string> _cmdMap = new();
        private HwndSource _messageWindow;
        #endregion

        #region Variables and Constants

        private const int MAX_PATH = 260;
        private const uint CMD_FIRST = 1;
        private const uint CMD_LAST = 30000;

        private const int S_OK = 0;
        private const int S_FALSE = 1;

        private static int cbMenuItemInfo = Marshal.SizeOf(typeof(MENUITEMINFO));
        private static int cbInvokeCommand = Marshal.SizeOf(typeof(CMINVOKECOMMANDINFOEX));

        private const uint WM_CONTEXTMENU = 0x007B;
        #endregion

        #region Shell GUIDs

        private static Guid IID_IShellFolder = new Guid("{000214E6-0000-0000-C000-000000000046}");
        private static Guid IID_IContextMenu = new Guid("{000214e4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu2 = new Guid("{000214f4-0000-0000-c000-000000000046}");
        private static Guid IID_IContextMenu3 = new Guid("{bcfce0a0-ec17-11d0-8d10-00a0c90f2719}");

        #endregion

        #region Constructor

        /// <summary>Default constructor</summary>
        public ShellContextMenu()
        {
            HwndSourceParameters parameters = new("HiddenShellWindow")
            {
                Width = 0,
                Height = 0,
                PositionX = 0,
                PositionY = 0,
                WindowStyle = unchecked((int)0x80000000), // WS_DISABLED
                ParentWindow = nint.Zero,
                UsesPerPixelOpacity = false
            };

            _messageWindow = new HwndSource(parameters);
            _messageWindow.AddHook(WndProc);
        }
        #endregion

        #region Destructor
        /// <summary>Ensure all resources get released</summary>
        ~ShellContextMenu()
        {
            ReleaseAll();
        }
        #endregion

        #region GetContextMenuInterfaces()
        /// <summary>Gets the interfaces to the context menu</summary>
        /// <param name="oParentFolder">Parent folder</param>
        /// <param name="arrPIDLs">PIDLs</param>
        /// <returns>true if it got the interfaces, otherwise false</returns>
        private bool GetContextMenuInterfaces(IShellFolder oParentFolder, nint[] arrPIDLs, out nint ctxMenuPtr)
        {
            int nResult = oParentFolder.GetUIObjectOf(
                nint.Zero,
                (uint)arrPIDLs.Length,
                arrPIDLs,
                ref IID_IContextMenu,
                nint.Zero,
                out ctxMenuPtr);

            if (S_OK == nResult)
            {
                _oContextMenu = (IContextMenu)Marshal.GetTypedObjectForIUnknown(ctxMenuPtr, typeof(IContextMenu));

                return true;
            }
            else
            {
                ctxMenuPtr = nint.Zero;
                _oContextMenu = null;
                return false;
            }
        }
        #endregion

        #region Override
        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            return new HandleRef(this, nint.Zero);
        }
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
        }

        protected override nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            #region IContextMenu

            if (_oContextMenu != null &&
                msg == (int)WM.MENUSELECT &&
                ((int)ShellHelper.HiWord(wParam) & (int)MFT.SEPARATOR) == 0 &&
                ((int)ShellHelper.HiWord(wParam) & (int)MFT.POPUP) == 0)
            {
                string info = string.Empty;

                if (ShellHelper.LoWord(wParam) == (int)CMD_CUSTOM.ExpandCollapse)
                    info = "Expands or collapses the current selected item";
                else
                {
                    info = "";
                }
            }

            #endregion

            #region IContextMenu2

            if (_oContextMenu2 != null &&
                (msg == (int)WM.INITMENUPOPUP ||
                 msg == (int)WM.MEASUREITEM ||
                 msg == (int)WM.DRAWITEM))
            {
                if (_oContextMenu2.HandleMenuMsg((uint)msg, wParam, lParam) == S_OK)
                    return nint.Zero;
            }

            #endregion

            #region IContextMenu3

            if (_oContextMenu3 != null &&
                msg == (int)WM.MENUCHAR)
            {
                if (_oContextMenu3.HandleMenuMsg2((uint)msg, wParam, lParam, nint.Zero) == S_OK)
                    return nint.Zero;
            }

            #endregion

            return nint.Zero;
        }

        #endregion

        #region InvokeCommand
        private void InvokeCommand(IContextMenu oContextMenu, uint nCmd, string strFolder, Point pointInvoke)
        {
            CMINVOKECOMMANDINFOEX invoke = new CMINVOKECOMMANDINFOEX();
            invoke.cbSize = cbInvokeCommand;
            invoke.lpVerb = (nint)(nCmd - CMD_FIRST);
            invoke.lpDirectory = strFolder;
            invoke.lpVerbW = (nint)(nCmd - CMD_FIRST);
            invoke.lpDirectoryW = strFolder;
            invoke.fMask = CMIC.UNICODE | CMIC.PTINVOKE |
                ((Keyboard.Modifiers & ModifierKeys.Control) != 0 ? CMIC.CONTROL_DOWN : 0) |
                ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 ? CMIC.SHIFT_DOWN : 0);
            invoke.ptInvoke = new POINT(pointInvoke.X, pointInvoke.Y);
            invoke.nShow = SW.SHOWNORMAL;

            oContextMenu.InvokeCommand(ref invoke);
        }
        #endregion

        #region ReleaseAll()
        /// <summary>
        /// Release all allocated interfaces, PIDLs 
        /// </summary>
        public void ReleaseAll()
        {
            if (null != _oContextMenu)
            {
                Marshal.ReleaseComObject(_oContextMenu);
                _oContextMenu = null;
            }
            if (null != _oContextMenu2)
            {
                Marshal.ReleaseComObject(_oContextMenu2);
                _oContextMenu2 = null;
            }
            if (null != _oContextMenu3)
            {
                Marshal.ReleaseComObject(_oContextMenu3);
                _oContextMenu3 = null;
            }
            if (null != _oDesktopFolder)
            {
                Marshal.ReleaseComObject(_oDesktopFolder);
                _oDesktopFolder = null;
            }
            if (null != _oParentFolder)
            {
                Marshal.ReleaseComObject(_oParentFolder);
                _oParentFolder = null;
            }
            if (null != _arrPIDLs)
            {
                FreePIDLs(_arrPIDLs);
                _arrPIDLs = null;
            }
        }
        #endregion

        #region GetDesktopFolder()
        /// <summary>
        /// Gets the desktop folder
        /// </summary>
        /// <returns>IShellFolder for desktop folder</returns>
        public IShellFolder GetDesktopFolder()
        {
            nint pUnkownDesktopFolder = nint.Zero;

            if (null == _oDesktopFolder)
            {
                // Get desktop IShellFolder
                int nResult = Dlls.SHGetDesktopFolder(out pUnkownDesktopFolder);
                if (S_OK != nResult)
                {
                    throw new ShellContextMenuException("Failed to get the desktop shell folder");
                }
                _oDesktopFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnkownDesktopFolder, typeof(IShellFolder));
            }

            return _oDesktopFolder;
        }
        #endregion

        #region GetParentFolder()
        /// <summary>
        /// Gets the parent folder
        /// </summary>
        /// <param name="folderName">Folder path</param>
        /// <returns>IShellFolder for the folder (relative from the desktop)</returns>
        private IShellFolder GetParentFolder(string folderName)
        {
            if (null == _oParentFolder)
            {
                IShellFolder oDesktopFolder = GetDesktopFolder();
                if (null == oDesktopFolder)
                {
                    return null;
                }

                // Get the PIDL for the folder file is in
                nint pPIDL = nint.Zero;
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                int nResult = oDesktopFolder.ParseDisplayName(nint.Zero, nint.Zero, folderName, ref pchEaten, out pPIDL, ref pdwAttributes);
                if (S_OK != nResult)
                {
                    return null;
                }

                nint pStrRet = Marshal.AllocCoTaskMem(MAX_PATH * 2 + 4);
                Marshal.WriteInt32(pStrRet, 0, 0);
                nResult = _oDesktopFolder.GetDisplayNameOf(pPIDL, SHGNO.FORPARSING, pStrRet);
                StringBuilder strFolder = new StringBuilder(MAX_PATH);
                Dlls.StrRetToBuf(pStrRet, pPIDL, strFolder, MAX_PATH);
                Marshal.FreeCoTaskMem(pStrRet);
                pStrRet = nint.Zero;
                _strParentFolder = strFolder.ToString();

                // Get the IShellFolder for folder
                nint pUnknownParentFolder = nint.Zero;
                nResult = oDesktopFolder.BindToObject(pPIDL, nint.Zero, ref IID_IShellFolder, out pUnknownParentFolder);
                // Free the PIDL first
                Marshal.FreeCoTaskMem(pPIDL);
                if (S_OK != nResult)
                {
                    return null;
                }
                _oParentFolder = (IShellFolder)Marshal.GetTypedObjectForIUnknown(pUnknownParentFolder, typeof(IShellFolder));
            }

            return _oParentFolder;
        }
        #endregion

        #region GetPIDLs()
        /// <summary>
        /// Get the PIDLs
        /// </summary>
        /// <param name="arrFI">Array of FileInfo</param>
        /// <returns>Array of PIDLs</returns>
        protected nint[] GetPIDLs(FileInfo[] arrFI)
        {
            if (null == arrFI || 0 == arrFI.Length)
            {
                return null;
            }

            IShellFolder oParentFolder = GetParentFolder(arrFI[0].DirectoryName);
            if (null == oParentFolder)
            {
                return null;
            }

            nint[] arrPIDLs = new nint[arrFI.Length];
            int n = 0;
            foreach (FileInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                nint pPIDL = nint.Zero;
                int nResult = oParentFolder.ParseDisplayName(nint.Zero, nint.Zero, fi.Name, ref pchEaten, out pPIDL, ref pdwAttributes);
                if (S_OK != nResult)
                {
                    FreePIDLs(arrPIDLs);
                    return null;
                }
                arrPIDLs[n] = pPIDL;
                n++;
            }

            return arrPIDLs;
        }

        /// <summary>
        /// Get the PIDLs
        /// </summary>
        /// <param name="arrFI">Array of DirectoryInfo</param>
        /// <returns>Array of PIDLs</returns>
        protected nint[] GetPIDLs(DirectoryInfo[] arrFI)
        {
            if (null == arrFI || 0 == arrFI.Length)
            {
                return null;
            }

            IShellFolder oParentFolder = GetParentFolder(arrFI[0].Parent.FullName);
            if (null == oParentFolder)
            {
                return null;
            }

            nint[] arrPIDLs = new nint[arrFI.Length];
            int n = 0;
            foreach (DirectoryInfo fi in arrFI)
            {
                // Get the file relative to folder
                uint pchEaten = 0;
                SFGAO pdwAttributes = 0;
                nint pPIDL = nint.Zero;
                int nResult = oParentFolder.ParseDisplayName(nint.Zero, nint.Zero, fi.Name, ref pchEaten, out pPIDL, ref pdwAttributes);
                if (S_OK != nResult)
                {
                    FreePIDLs(arrPIDLs);
                    return null;
                }
                arrPIDLs[n] = pPIDL;
                n++;
            }

            return arrPIDLs;
        }
        #endregion

        #region FreePIDLs()
        /// <summary>
        /// Free the PIDLs
        /// </summary>
        /// <param name="arrPIDLs">Array of PIDLs (IntPtr)</param>
        protected void FreePIDLs(nint[] arrPIDLs)
        {
            if (null != arrPIDLs)
            {
                for (int n = 0; n < arrPIDLs.Length; n++)
                {
                    if (arrPIDLs[n] != nint.Zero)
                    {
                        Marshal.FreeCoTaskMem(arrPIDLs[n]);
                        arrPIDLs[n] = nint.Zero;
                    }
                }
            }
        }
        #endregion

        #region InvokeContextMenuDefault
        private void InvokeContextMenuDefault(FileInfo[] arrFI)
        {
            // Release all resources first.
            ReleaseAll();

            nint pMenu = nint.Zero,
                iContextMenuPtr = nint.Zero;

            try
            {
                _arrPIDLs = GetPIDLs(arrFI);
                if (null == _arrPIDLs)
                {
                    ReleaseAll();
                    return;
                }

                if (false == GetContextMenuInterfaces(_oParentFolder, _arrPIDLs, out iContextMenuPtr))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = Dlls.CreatePopupMenu();

                int nResult = _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    CMF.DEFAULTONLY |
                    ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 ? CMF.EXTENDEDVERBS : 0));

                uint nDefaultCmd = (uint)Dlls.GetMenuDefaultItem(pMenu, false, 0);
                if (nDefaultCmd >= CMD_FIRST)
                {
                    var point = Mouse.GetPosition(null);
                    InvokeCommand(_oContextMenu, nDefaultCmd, arrFI[0].DirectoryName, new Point((int)point.X, (int)point.Y));
                }

                Dlls.DestroyMenu(pMenu);
                pMenu = nint.Zero;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (pMenu != nint.Zero)
                {
                    Dlls.DestroyMenu(pMenu);
                }
                ReleaseAll();
            }
        }
        #endregion

        #region ShowContextMenu()

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="files">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(FileInfo[] files, Point pointScreen)
        {
            // Release all resources first.
            ReleaseAll();
            _arrPIDLs = GetPIDLs(files);
            ShowContextMenu(pointScreen);
        }

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="dirs">DirectoryInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowContextMenu(DirectoryInfo[] dirs, Point pointScreen)
        {
            // Release all resources first.
            ReleaseAll();
            _arrPIDLs = GetPIDLs(dirs);
            ShowContextMenu(pointScreen);
        }

        /// <summary>
        /// Shows the context menu
        /// </summary>
        /// <param name="arrFI">FileInfos (should all be in same directory)</param>
        /// <param name="pointScreen">Where to show the menu</param>
        private void ShowContextMenu(Point pointScreen)
        {
            nint pMenu = nint.Zero,
                iContextMenuPtr = nint.Zero,
                iContextMenuPtr2 = nint.Zero,
                iContextMenuPtr3 = nint.Zero;

            try
            {
                if (null == _arrPIDLs)
                {
                    ReleaseAll();
                    return;
                }

                if (false == GetContextMenuInterfaces(_oParentFolder, _arrPIDLs, out iContextMenuPtr))
                {
                    ReleaseAll();
                    return;
                }

                pMenu = Dlls.CreatePopupMenu();

                int nResult = _oContextMenu.QueryContextMenu(
                    pMenu,
                    0,
                    CMD_FIRST,
                    CMD_LAST,
                    CMF.EXPLORE |
                    CMF.NORMAL |
                     //CMF.DEFAULTONLY |
                     CMF.CANRENAME |
                    ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 ? CMF.EXTENDEDVERBS : 0));

                // 清空旧的命令映射
                _cmdMap.Clear();

                // 记录命令 ID -> verb 字符串
                for (uint i = CMD_FIRST; i < CMD_FIRST + (uint)nResult; i++)
                {
                    byte[] buffer = new byte[512];
                    int hr = _oContextMenu.GetCommandString(i - CMD_FIRST, GCS.VERBA, 0, buffer, buffer.Length);
                    if (hr == S_OK)
                    {
                        string verb = Encoding.ASCII.GetString(buffer).TrimEnd('\0');
                        if (!string.IsNullOrEmpty(verb))
                        {
                            _cmdMap[i] = verb;
                        }
                    }
                }

                Marshal.QueryInterface(iContextMenuPtr, ref IID_IContextMenu2, out iContextMenuPtr2);
                Marshal.QueryInterface(iContextMenuPtr, ref IID_IContextMenu3, out iContextMenuPtr3);

                _oContextMenu2 = (IContextMenu2)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr2, typeof(IContextMenu2));
                _oContextMenu3 = (IContextMenu3)Marshal.GetTypedObjectForIUnknown(iContextMenuPtr3, typeof(IContextMenu3));

                uint nSelected = Dlls.TrackPopupMenuEx(
                    pMenu,
                    TPM.RETURNCMD,
                    pointScreen.X,
                    pointScreen.Y,
                    _messageWindow.Handle,
                    nint.Zero);

                if (nSelected != 0)
                {
                    if (_cmdMap.TryGetValue(nSelected, out var verb) && verb.Equals("rename", StringComparison.OrdinalIgnoreCase))
                    {
                        // ⚠️拦截 rename 动作
                        System.Windows.MessageBox.Show("你点击了『重命名』，此处执行自定义逻辑。");
                    }
                    else
                    {
                        InvokeCommand(_oContextMenu, nSelected, _strParentFolder, pointScreen);
                    }
                }

                Dlls.DestroyMenu(pMenu);
                pMenu = nint.Zero;
            }
            catch
            {
                throw;
            }
            finally
            {
                //hook.Uninstall();
                if (pMenu != nint.Zero)
                {
                    Dlls.DestroyMenu(pMenu);
                }

                if (iContextMenuPtr != nint.Zero)
                    Marshal.Release(iContextMenuPtr);

                if (iContextMenuPtr2 != nint.Zero)
                    Marshal.Release(iContextMenuPtr2);

                if (iContextMenuPtr3 != nint.Zero)
                    Marshal.Release(iContextMenuPtr3);

                ReleaseAll();
            }
        }

        /// <summary>
        /// show desktop context menu
        /// </summary>
        /// <param name="pointScreen">Where to show the menu</param>
        public void ShowDesktopContextMenu(Point pointScreen)
        {
            nint shellViewHwnd = DesktopWindow.FindDesktopWindow();

            if (shellViewHwnd != nint.Zero)
            {
                // 组合 x 和 y 坐标到 lParam
                nint lParam = pointScreen.Y << 16 | pointScreen.X;// pointScreen.Y << 16 | pointScreen.X;

                Dlls.PostMessage(shellViewHwnd, WM_CONTEXTMENU, nint.Zero, lParam);
            }
        }

        #endregion

        // The cmd for a custom added menu item
        private enum CMD_CUSTOM
        {
            ExpandCollapse = (int)CMD_LAST + 1
        }

    }

    #region ShellContextMenuException
    public class ShellContextMenuException : Exception
    {
        /// <summary>Default contructor</summary>
        public ShellContextMenuException()
        {
        }

        /// <summary>Constructor with message</summary>
        /// <param name="message">Message</param>
        public ShellContextMenuException(string message)
            : base(message)
        {
        }
    }
    #endregion

    #region Class HookEventArgs
    public class HookEventArgs : EventArgs
    {
        public int HookCode;	// Hook code
        public nint wParam;	// WPARAM argument
        public nint lParam;	// LPARAM argument
    }
    #endregion

    #region Class LocalWindowsHook
    public class LocalWindowsHook
    {
        // ************************************************************************
        // Filter function delegate
        public delegate int HookProc(int code, nint wParam, nint lParam);
        // ************************************************************************

        // ************************************************************************
        // Internal properties
        protected nint m_hhook = nint.Zero;
        protected HookProc m_filterFunc = null;
        protected HookType m_hookType;
        // ************************************************************************

        // ************************************************************************
        // Event delegate
        public delegate void HookEventHandler(object sender, HookEventArgs e);
        // ************************************************************************

        // ************************************************************************
        // Event: HookInvoked 
        public event HookEventHandler HookInvoked;
        protected void OnHookInvoked(HookEventArgs e)
        {
            if (HookInvoked != null)
                HookInvoked(this, e);
        }
        // ************************************************************************

        // ************************************************************************
        // Class constructor(s)
        public LocalWindowsHook(HookType hook)
        {
            m_hookType = hook;
            m_filterFunc = new HookProc(CoreHookProc);
        }
        public LocalWindowsHook(HookType hook, HookProc func)
        {
            m_hookType = hook;
            m_filterFunc = func;
        }
        // ************************************************************************

        // ************************************************************************
        // Default filter function
        protected int CoreHookProc(int code, nint wParam, nint lParam)
        {
            if (code < 0)
                return Dlls.CallNextHookEx(m_hhook, code, wParam, lParam);

            // Let clients determine what to do
            HookEventArgs e = new HookEventArgs();
            e.HookCode = code;
            e.wParam = wParam;
            e.lParam = lParam;
            OnHookInvoked(e);

            // Yield to the next hook in the chain
            return Dlls.CallNextHookEx(m_hhook, code, wParam, lParam);
        }
        // ************************************************************************

        // ************************************************************************
        // Install the hook
        public void Install()
        {
            m_hhook = Dlls.SetWindowsHookEx(
                m_hookType,
                m_filterFunc,
                nint.Zero,
                Thread.CurrentThread.ManagedThreadId);
        }
        // ************************************************************************

        // ************************************************************************
        // Uninstall the hook
        public void Uninstall()
        {
            Dlls.UnhookWindowsHookEx(m_hhook);
        }
        // ************************************************************************

    }
    #endregion

    #region ShellHelper

    internal static class ShellHelper
    {
        #region Low/High Word

        /// <summary>
        /// Retrieves the High Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the High Word</returns>
        public static uint HiWord(nint ptr)
        {
            uint param32 = (uint)(ptr.ToInt64() & 0xffffffffL);
            if ((param32 & 0x80000000) == 0x80000000)
                return param32 >> 16;
            else
                return param32 >> 16 & 0xffff;
        }

        /// <summary>
        /// Retrieves the Low Word of a WParam of a WindowMessage
        /// </summary>
        /// <param name="ptr">The pointer to the WParam</param>
        /// <returns>The unsigned integer for the Low Word</returns>
        public static uint LoWord(nint ptr)
        {
            uint param32 = (uint)(ptr.ToInt64() & 0xffffffffL);
            return param32 & 0xffff;
        }

        #endregion
    }

    #endregion
}
