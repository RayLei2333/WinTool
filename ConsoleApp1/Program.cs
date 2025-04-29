using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ConsoleApp1
{
    internal class Program
    {

        // Win32 API 导入
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const int WS_EX_NOACTIVATE = 0x08000000;


        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            IntPtr hwnd = Process.GetCurrentProcess().MainWindowHandle;
            int extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TOPMOST | WS_EX_NOACTIVATE);
            Console.WriteLine("窗口已设置为免疫显示桌面");
            //Console.ReadKey(); // 保持控制台窗口打开，查看效果

            // 使用示例：
            //string folderPath = @"D:\\";
            //string filePath = @"D:\\123123.jpg";
            //ShellContextMenu scm = new ShellContextMenu();
            //scm.ShowContextMenu(new string[] { filePath }, new Point(0,0));
            //e.Handled = true;



            //Icon jumboIcon = IconExtractor5.icon_of_path_large(filePath, true, true);
            //var jumboBmp = jumboIcon.ToBitmap();
            //jumboBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\jumbo_0.png");

            //Icon jumboIcon = FileIconExtractor.GetJubmoIcon(filePath);
            //var jumboBmp = jumboIcon.ToBitmap();
            //jumboBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\jumbo_0.png");

            // 获取大图标
            //Icon largeIcon = FileIconExtractor.GetLargeIcon(filePath);
            //var largeBmp = largeIcon.ToBitmap();
            //largeBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\large_0.png");
            //Bitmap bitmap64 = new Bitmap(largeBmp, new Size(64, 64));
            //bitmap64.Save($"C:\\Users\\23162\\Desktop\\testicon\\large_64.png");

            //// 获取小图标
            //Icon smallIcon = FileIconExtractor.GetSmallIcon(filePath);
            //var smallBmp = smallIcon.ToBitmap();
            //smallBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\small_0.png");
            //Console.WriteLine("图标已成功获取并保存！");


            // 使用示例：
            //Icon largeIcon = IconExtractor4.GetIconByFileName("FILE", filePath, true);  // 大图标
            //var largeBmp = largeIcon.ToBitmap();
            //largeBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\large_0.png");

            //Icon smallIcon = IconExtractor4.GetIconByFileName("FILE", "文件路径", false); // 小图标
            //var smallBmp = smallIcon.ToBitmap();
            //smallBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\small_0.png");
            //string[] files = Directory.GetFiles(folderPath);
            //foreach (string file in files)
            //{
            //    Icon[] icons = IconExtractor3.GetIcons(file);
            //    foreach (Icon icon in icons)
            //    {
            //        // 使用图标
            //    }
            //}



            //int index = 1;
            //// 使用示例：
            //string folderPath = @"D:\\";
            //string[] files = Directory.GetFiles(folderPath);
            //foreach (string file in files)
            //{
            //    Icon largeIcon = IconExtractor2.GetFileIcon(file, true);  // 大图标
            //    Icon smallIcon = IconExtractor2.GetFileIcon(file, false); // 小图标
            //                                                              // 使用图标
            //    var largeBmp = largeIcon.ToBitmap();
            //    var smallBmp = smallIcon.ToBitmap();
            //    largeBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\large_{index}.png");
            //    smallBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\small_{index}.png");
            //    index++;
            //}

            //// 使用示例：
            //string folderPath = @"D:\\";
            //string[] files = Directory.GetFiles(folderPath);
            //int index = 1;
            //foreach (string file in files)
            //{
            //    if (file.Contains("imsdk_report"))
            //        continue;
            //    Icon largeIcon = IconExtractor.GetIcon(file, IconExtractor.SHIL_FLAGS.SHIL_LARGE);  // 大图标
            //                                                                                        //Icon smallIcon = IconExtractor.GetFileIcon(file, IconExtractor.IMAGELIST_SIZE_FLAG.ILD_smallsize); // 小图标
            //                                                                                        //Icon middleIcon = IconExtractor.GetFileIcon(file, IconExtractor.IMAGELIST_SIZE_FLAG.ILD_normal); // 中图标（比如Size(32,32)）
            //                                                                                        // 使用图标
            //    var largeBmp = largeIcon.ToBitmap();
            //    //var smallBmp = smallIcon.ToBitmap();
            //    //var middleBmp = middleIcon.ToBitmap();

            //    largeBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\large_{index}.png");
            //    //smallBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\small_{index}.png");
            //    //middleBmp.Save($"C:\\Users\\23162\\Desktop\\testicon\\middle_{index}.png");
            //    index++;
            //}



            //string folderPath = @"D:\\";
            //string[] files = Directory.GetFiles(folderPath);
            //int index = 1;
            //foreach (string file in files)
            //{
            //    Icon icon = Icon.ExtractAssociatedIcon(file);
            //    // 使用图标
            //    var bm =  icon.ToBitmap();
            //    bm.Save($"C:\\Users\\23162\\Desktop\\testicon\\{index}.png");
            //    index++;
            //}

        }
    }
}
