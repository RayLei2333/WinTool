namespace Desktop.Win32Support.Models
{
    /// <summary>
    /// 桌面图标排列方式
    /// </summary>
    public class DesktopIconArrange
    {
        /// <summary>
        /// 自动排列图标
        /// </summary>
        public bool AutoArrange { get; set; }

        /// <summary>
        /// 与网格对齐
        /// </summary>
        public bool AlignedToGrid { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        /// <summary>
        ///  0 => "图标",
        ///  1 => "详情",
        ///  2 => "小图标",
        ///  3 => "列表",
        ///  4 => "中等图标或大图标",
        /// </summary>
        //public string IconView { get; set; }
    }
}
