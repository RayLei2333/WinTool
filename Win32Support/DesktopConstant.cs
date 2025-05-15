using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win32Support
{
    internal class DesktopConstant
    {
        public const uint LVM_FIRST = 0x1000;

        //获取图标视图，大图标、中等图标、小图标
        public const uint LVM_GETITEMSPACING = LVM_FIRST + 51;
        //获取自动排列and网格对齐方式
        public const uint LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;

        public const int LVS_AUTOARRANGE = 0x0040;
        public const int LVS_ALIGNLEFT = 0x0800;
        public const int LVS_SNAPTOGRID = 0x0400;

        //获取列表视图项的个数
        public const int LVM_GETITEMCOUNT = 0x1000 + 4;
        //获取列表项位置
        public const int LVM_GETITEMPOSITION = 0x1000 + 16;
        //获取列表名称
        public const int LVM_GETITEMTEXT = 0x1000 + 115;

        public const int LVIF_TEXT = 0x0001;


        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;
        public const uint PAGE_READWRITE = 0x04;


        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;
        public const uint PROCESS_QUERY_INFORMATION = 0x0400;

        public const int SPI_GETDESKWALLPAPER = 0x0073;
    }
}
