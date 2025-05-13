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
    /// Test2Window.xaml 的交互逻辑
    /// </summary>
    public partial class Test2Window : Window
    {
        private ShellContextMenu _ctxMnu = new();

        public Test2Window()
        {
            InitializeComponent();
        }

        private void fileRightMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            string testFilePath = @"C:\Users\23162\Desktop\2025-05-12 - 副本.txt";
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
            return new System.Drawing.Point(0, 0);
           // return System.Windows.Forms.Cursor.Position;
        }
    }
}
