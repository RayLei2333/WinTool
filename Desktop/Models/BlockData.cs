using Desktop.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Desktop
{
    public class BlockData
    {
        public string Id { get; set; }

        /// <summary>
        /// X
        /// </summary>
        public int X { get; set; }

        /// <summary>
        /// Y
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// 块宽度
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 块高度
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// Block名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否锁定状态
        /// </summary>
        public bool Lock { get; set; }

        /// <summary>
        /// 视图类型
        /// </summary>
        public ViewType ViewType { get; set; }

        /// <summary>
        /// 文件路径列表
        /// </summary>
        public List<string> FilePathList { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        [JsonIgnore]
        public List<FileData> FileList { get; set; } = new List<FileData>();
    }
}
