using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Desktop.Models
{
    public class FileData // : FileHandler
    {
        /// <summary>
        /// 是否文件夹
        /// </summary>
        public bool IsFolder { get; set; }

        /// <summary>
        /// 是否为快捷方式文件
        /// </summary>
        public bool IsLnkFile { get; set; }

        /// <summary>
        /// 是否为图片文件
        /// </summary>
        public bool IsImageFile { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// 文件目标位置，用于lnk文件的目标路径
        /// </summary>
        public string TargetPath { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Seq { get; set; }

        [JsonIgnore]
        public ImageSource Icon { get; set; }
    }
}
