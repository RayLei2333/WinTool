using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Desktop
{
    internal class Constants
    {
        public static string BasePath = AppDomain.CurrentDomain.BaseDirectory;

        public static string DataPath = Path.Combine(BasePath, "Data");

        public static string DesktopDataPath = Path.Combine(DataPath, "desktop_data.dat");

        public static string BlockDataPath = Path.Combine(DataPath, "block_data.dat");

        //public static string DesktopDataPath = ;
    }
}
