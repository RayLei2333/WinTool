using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Events
{
    /// <summary>
    /// 调整位置事件参数
    /// </summary>
    public class BlockAdjustPositionEventArgs : EventArgs
    {
        /// <summary>
        /// 块ID
        /// </summary>
        public string BlockId { get; set; }

        /// <summary>
        /// 调整后的位置
        /// </summary>
        public Point Point { get; set; }
    }

    public delegate void AdjustBlockPositionEventHandler(object sender, BlockAdjustPositionEventArgs e);
}
