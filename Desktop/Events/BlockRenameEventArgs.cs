using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Events
{
    public class BlockRenameEventArgs : EventArgs
    {
        public BlockRenameEventArgs(string oldName, string newName)
        {
            OldName = oldName;
            NewName = newName;
        }

        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public delegate void BlockRenameEventArgsEventHandler(object sender, BlockRenameEventArgs e);
}
