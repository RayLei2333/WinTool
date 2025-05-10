using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Desktop
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        private ShellContextMenu _ctxMnu = new();

        public TestWindow()
        {
            InitializeComponent();
        }

        private void fileRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string testFilePath = @"C:\Users\10475\Desktop\wb.json";
            FileInfo[] files = new FileInfo[1] { new FileInfo(testFilePath) };
            _ctxMnu.ShowContextMenu(files, GetMousePosition());
        }

        private void folderRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string path = @"C:\Users\10475\Desktop\PSD";
            DirectoryInfo[] arrFI = [new DirectoryInfo(path)];
            _ctxMnu.ShowContextMenu(arrFI, GetMousePosition());
        }

        private void desktopRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            _ctxMnu.ShowDesktopContextMenu(GetMousePosition());
        }

        private System.Drawing.Point GetMousePosition()
        {
            return System.Windows.Forms.Cursor.Position;
        }
    }
}
