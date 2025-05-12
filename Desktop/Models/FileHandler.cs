using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Desktop.Models
{
    public class FileHandler
    {
        public bool IsFolder { get; set; }

        public bool IsLnkFile { get; set; }

        public string Suffix { get; set; }

        public string FullPath { get; set; }
    }
}
