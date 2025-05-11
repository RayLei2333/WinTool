using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Desktop.Models
{
    public class FileData : FileHandler
    {
        public string Name { get; set; }

        public ImageSource Icon { get; set; }
    }
}
