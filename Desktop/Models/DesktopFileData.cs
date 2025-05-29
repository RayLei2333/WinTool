using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Models
{
    public class DesktopFileData: FileData
    {
        public int X { get; set; }

        public int Y { get; set; }

        public int Left { get; set; }

        public int Top { get; set; }

        public int Right { get; set; }

        public int Bottom { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }
    }
}
