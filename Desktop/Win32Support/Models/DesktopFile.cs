namespace Desktop.Win32Support.Models
{
    public class DesktopFile
    {
        public int Index { get; set; }

        public string Name { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public string FilePath { get; set; }

        public bool IsShortcut { get; set; }
    }
}
