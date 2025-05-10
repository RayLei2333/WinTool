using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Events
{
    public class JustSaveEventArgs : EventArgs
    {
    }

    public delegate void JustSaveEventHandler(object sender, JustSaveEventArgs e);
}
