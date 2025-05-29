using Desktop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
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
using static System.Net.Mime.MediaTypeNames;

namespace Desktop.Views
{
    /// <summary>
    /// DesktopIconView.xaml 的交互逻辑
    /// </summary>
    public partial class DesktopIconView : UserControl, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        public static readonly DependencyProperty IsSelectedProperty =
          DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(DesktopIconView),
              new FrameworkPropertyMetadata(false, OnIsSelectedChanged));


        private ImageSource _iconSource;

        public ImageSource IconSource
        {
            get { return _iconSource; }
            set
            {
                _iconSource = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IconSource)));
            }
        }


        private string _shortName;

        public string ShortName
        {
            get { return _shortName; }
            set
            {
                _shortName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShortName)));
            }
        }


        private string _fullName;

        public string FullName
        {
            get { return _fullName; }
            set
            {
                _fullName = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullName)));
            }
        }

        public bool IsSelected
        {
            get => (bool)GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }



        private Visibility _shortNameVisibility = Visibility.Visible;

        public Visibility ShortNameVisibility
        {
            get { return _shortNameVisibility; }
            set { _shortNameVisibility = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShortNameVisibility))); }
        }

        private Visibility _fullNameVisibility = Visibility.Collapsed;

        public Visibility FullNameVisibility
        {
            get { return _fullNameVisibility; }
            set { _fullNameVisibility = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(FullNameVisibility))); }
        }



        public DesktopIconView()
        {
            InitializeComponent();
            this.Loaded += DesktopIconView_Loaded;
            //this.Dispatcher.BeginInvoke(() =>
            //{

            //    if (this.ActualHeight < 70)
            //    {
            //        double padding = (70 - this.ActualHeight) / 2;
            //        this.Padding = new Thickness(0, padding, 0, padding);
            //    }
            //}, System.Windows.Threading.DispatcherPriority.Loaded);

        }

        private void DesktopIconView_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is DesktopFileData desktopFileData)
            {
                this.IconSource = desktopFileData.Icon;
                this.FullName = desktopFileData.Name;
                ToShortName();
            }
        }


        //显示的名称转换
        public void ToShortName()
        {
            //var dpi = VisualTreeHelper.GetDpi(this);
            //var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
            //double maxWidth = ActualWidth - textShortName.Margin.Left - textShortName.Margin.Right;
            //double lineHeight = FontSize * 1.2;
            ////能完全单行显示
            //var textSingle = CreateFormattedText(this.FullName, typeface, dpi.PixelsPerDip);
            //if (textSingle.Width <= maxWidth)
            //{
            //    ShortName = this.FullName;
            //    return;
            //}


            //int breakIndex = FindBreakIndex(this.FullName, typeface, maxWidth, dpi.PixelsPerDip);
            //string firstLine = this.FullName.Substring(0, breakIndex).TrimEnd();
            //string secondLine = this.FullName.Substring(breakIndex).TrimStart();
            //var ftFirst = CreateFormattedText(firstLine, typeface, dpi.PixelsPerDip);
            //var ftSecond = CreateFormattedText(secondLine, typeface, dpi.PixelsPerDip);
            ////如果分行后的第一行宽度小于最大宽度的一半  则在一行显示并追加...
            //if (ftFirst.Width < maxWidth * 0.5)
            //{
            //    textShortName.Margin = new Thickness(2, textShortName.Margin.Top, 2, textShortName.Margin.Bottom);
            //    //Margin = new Thickness(2, Margin.Top, 2, Margin.Bottom);
            //    maxWidth = maxWidth - textShortName.Margin.Left - textShortName.Margin.Right;
            //    string singleLineEllipsed = TrimWithEllipsis(this.FullName, typeface, maxWidth, dpi.PixelsPerDip);
            //    var ftSingle = CreateFormattedText(singleLineEllipsed, typeface, dpi.PixelsPerDip);
            //    ShortName = ftSingle.Text;
            //    return;
            //}


            //if (ftFirst.Width <= ActualWidth && ftSecond.Width <= ActualWidth)
            //{
            //    // 两行完整显示
            //    ShortName = $"{ftFirst.Text}\r\n{ftSecond.Text}";
            //    FullName = ShortName;
            //    return;
            //}

            //// Step 3: 第二行加省略号
            //string secondLineEllipsed = TrimWithEllipsis(secondLine, typeface, maxWidth, dpi.PixelsPerDip);
            //var ftSecondEllipsed = CreateFormattedText(secondLineEllipsed, typeface, dpi.PixelsPerDip);

            //ShortName = $"{ftFirst.Text}\r\n{ftSecondEllipsed.Text}";
        }

        private int FindBreakIndex(string text, Typeface typeface, double maxWidth, double dpi)
        {
            double width = 0;
            int lastBreak = -1;

            for (int i = 0; i < text.Length; i++)
            {
                var ft = CreateFormattedText(text[i].ToString(), typeface, dpi);
                width += ft.Width;

                if (width > maxWidth)
                {
                    return lastBreak > 0 ? lastBreak : i;
                }

                if (IsBreakChar(text[i]))
                {
                    lastBreak = i + 1;
                }
            }

            return text.Length;
        }

        private bool IsBreakChar(char c)
        {
            // 常用断行符号（Windows 桌面图标默认断行位置）
            return char.IsWhiteSpace(c) || c == '-' || c == '_' || c == '.' || char.IsPunctuation(c);
        }

        // 加省略号裁剪文本，保证宽度不超
        private string TrimWithEllipsis(string text, Typeface typeface, double maxWidth, double dpi)
        {
            const string ellipsis = "...";

            for (int len = text.Length; len >= 0; len--)
            {
                string candidate = text.Substring(0, len) + ellipsis;
                var ft = CreateFormattedText(candidate, typeface, dpi);
                if (ft.Width <= maxWidth)
                    return candidate;
            }

            return ellipsis;
        }

        private FormattedText CreateFormattedText(string text, Typeface typeface, double dpi)
        {
            return new FormattedText(text, System.Globalization.CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, typeface, this.FontSize, this.Foreground, dpi);
        }

        public static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DesktopIconView control && e.NewValue is bool isSelected)
            {
                if (isSelected)
                {
                    control.ShortNameVisibility = Visibility.Collapsed;
                    control.FullNameVisibility = Visibility.Visible;
                }
                else
                {
                    control.ShortNameVisibility = Visibility.Visible;
                    control.FullNameVisibility = Visibility.Collapsed;
                }
            }
        }

    }
}
