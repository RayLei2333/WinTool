using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class IconExtractor3
    {
        [DllImport("shell32.dll")]
        public static extern int ExtractIconEx(string lpszFile, int nIconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        public static Icon[] GetIcons(string appPath)
        {
            int count = ExtractIconEx(appPath, -1, null, null, 0);
            IntPtr[] largeIcons = new IntPtr[count];
            IntPtr[] smallIcons = new IntPtr[count];
            ExtractIconEx(appPath, 0, largeIcons, smallIcons, count);
            Icon[] icons = new Icon[count * 2];
            for (int i = 0; i < count; i++)
            {
                icons[i * 2] = Icon.FromHandle(largeIcons[i]);
                icons[i * 2 + 1] = Icon.FromHandle(smallIcons[i]);
            }
            return icons;
        }
    }
}
