using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Models
{
    internal class DesktopData
    {
        /// <summary>
        /// 自动排列图标
        /// </summary>
        public bool AutoArrange { get; set; }

        /// <summary>
        /// 与网格对齐
        /// </summary>
        public bool AlignedToGrid { get; set; }

        /// <summary>
        /// 图标宽度
        /// </summary>
        public int IconWidth { get; set; }

        /// <summary>
        /// 图标高度
        /// </summary>
        public int IconHeight { get; set; } 

        public List<DesktopFileData> FileData { get; set; }

        public DesktopData()
        {
            FileData = new List<DesktopFileData>();
        }
    }
}
