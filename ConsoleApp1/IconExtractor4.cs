using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class IconExtractor4
    {
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern uint ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, uint nIcons);

        [DllImport("User32.dll", SetLastError = true)]
        private static extern bool DestroyIcon(IntPtr hIcon);

        public static Icon GetIconByFileName(string tcType, string tcFullName, bool tlIsLarge = false)
        {
            Icon ico = null;
            string fileType = tcFullName.Contains(".") ? tcFullName.Substring(tcFullName.LastIndexOf('.')).ToLower() : string.Empty;
            RegistryKey regVersion = null;
            string regFileType = null;
            string regIconString = null;
            string systemDirectory = Environment.SystemDirectory + "\\";
            IntPtr[] phiconLarge = new IntPtr[1];
            IntPtr[] phiconSmall = new IntPtr[1];
            IntPtr hIcon = IntPtr.Zero;

            if (tcType == "FILE")
            {
                if (".exe.ico".Contains(fileType))
                {
                    phiconLarge[0] = phiconSmall[0] = IntPtr.Zero;
                    uint rst1 = ExtractIconEx(tcFullName, 0, phiconLarge, phiconSmall, 1);
                    hIcon = tlIsLarge ? phiconLarge[0] : phiconSmall[0];
                    ico = hIcon == IntPtr.Zero ? null : Icon.FromHandle(hIcon).Clone() as Icon;
                    if (phiconLarge[0] != IntPtr.Zero) DestroyIcon(phiconLarge[0]);
                    if (phiconSmall[0] != IntPtr.Zero) DestroyIcon(phiconSmall[0]);
                    if (ico != null)
                    {
                        return ico;
                    }
                }

                regVersion = Registry.ClassesRoot.OpenSubKey(fileType, false);
                if (regVersion != null)
                {
                    regFileType = regVersion.GetValue("") as string;
                    regVersion.Close();
                    regVersion = Registry.ClassesRoot.OpenSubKey(regFileType + @"\DefaultIcon", false);
                    if (regVersion != null)
                    {
                        regIconString = regVersion.GetValue("") as string;
                        regVersion.Close();
                    }
                }
                if (regIconString == null)
                {
                    regIconString = systemDirectory + "shell32.dll,0";
                }
            }
            else
            {
                regIconString = systemDirectory + "shell32.dll,3";
            }

            string[] fileIcon = regIconString.Split(new char[] { ',' });
            fileIcon = fileIcon.Length == 2 ? fileIcon : new string[] { systemDirectory + "shell32.dll", "2" };

            phiconLarge[0] = phiconSmall[0] = IntPtr.Zero;
            uint rst = ExtractIconEx(fileIcon[0].Trim('"'), Int32.Parse(fileIcon[1]), phiconLarge, phiconSmall, 1);
            hIcon = tlIsLarge ? phiconLarge[0] : phiconSmall[0];
            ico = hIcon == IntPtr.Zero ? null : Icon.FromHandle(hIcon).Clone() as Icon;
            if (phiconLarge[0] != IntPtr.Zero) DestroyIcon(phiconLarge[0]);
            if (phiconSmall[0] != IntPtr.Zero) DestroyIcon(phiconSmall[0]);
            if (ico != null)
            {
                return ico;
            }

            if (tcType == "FILE")
            {
                fileIcon = new string[] { systemDirectory + "shell32.dll", "2" };
                phiconLarge = new IntPtr[1];
                phiconSmall = new IntPtr[1];
                rst = ExtractIconEx(fileIcon[0], Int32.Parse(fileIcon[1]), phiconLarge, phiconSmall, 1);
                hIcon = tlIsLarge ? phiconLarge[0] : phiconSmall[0];
                ico = hIcon == IntPtr.Zero ? null : Icon.FromHandle(hIcon).Clone() as Icon;
                if (phiconLarge[0] != IntPtr.Zero) DestroyIcon(phiconLarge[0]);
                if (phiconSmall[0] != IntPtr.Zero) DestroyIcon(phiconSmall[0]);
            }

            return ico;
        }
    }
}
