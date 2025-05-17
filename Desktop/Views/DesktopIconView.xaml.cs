using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Desktop.Views
{
    /// <summary>
    /// DesktopIconView.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopIconView : UserControl
    {
        public DesktopIconView()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty IconNameProperty =
            DependencyProperty.Register(nameof(IconName), typeof(string), typeof(DesktopIconView),
                new PropertyMetadata(null));

        public static readonly DependencyProperty IconSourceProperty =
            DependencyProperty.Register(nameof(IconSource), typeof(ImageSource), typeof(DesktopIconView),
                new PropertyMetadata(null));

        //public static readonly DependencyProperty TextMaxWidthProperty =
        //    DependencyProperty.Register(nameof(TextMaxWidth), typeof(double), typeof(DesktopIconView),
        //        new PropertyMetadata(76.0));

        public string IconName
        {
            get => (string)GetValue(IconNameProperty);
            set => SetValue(IconNameProperty, value);
        }

        public ImageSource IconSource
        {
            get => (ImageSource)GetValue(IconSourceProperty);
            set => SetValue(IconSourceProperty, value);
        }

        //public double TextMaxWidth
        //{
        //    get => (double)GetValue(TextMaxWidthProperty);
        //    set => SetValue(TextMaxWidthProperty, value);
        //}


        //private static void OnIconNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is DesktopIconView control && e.NewValue is string newText)
        //    {
        //        control.IconText.Text = newText;
        //        control.UpdateWrapping(newText);
        //    }
        //}

        //private void UpdateWrapping(string text)
        //{
        //    var formattedText = new FormattedText(
        //        text,
        //        CultureInfo.CurrentCulture,
        //        FlowDirection.LeftToRight,
        //        new Typeface(IconText.FontFamily, IconText.FontStyle, IconText.FontWeight, IconText.FontStretch),
        //        IconText.FontSize,
        //        Brushes.Black,
        //        VisualTreeHelper.GetDpi(this).PixelsPerDip);


        //    if (formattedText.Width <= TextMaxWidth * 1.2 || formattedText.Width > TextMaxWidth * 1.7)
        //    {
        //        // 一行可显示
        //        IconText.TextWrapping = TextWrapping.NoWrap;
        //        IconText.MaxHeight = 16; // 单行高度
        //    }
        //    else
        //    {
        //        // 超过一行，使用 Wrap + 两行限制
        //        IconText.TextWrapping = TextWrapping.Wrap;
        //        IconText.MaxHeight = 32; // 两行高度
        //    }
        //}
    }
}
