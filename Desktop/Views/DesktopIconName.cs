using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Desktop.Views
{
    public class DesktopIconName : TextBlock
    {

        public static readonly DependencyProperty TextContentProperty =
          DependencyProperty.Register(nameof(TextContent), typeof(string), typeof(DesktopIconName),
              new FrameworkPropertyMetadata(string.Empty, OnTextChanged));

        public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        public DesktopIconName()
        {
        }


        public static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DesktopIconName control && e.NewValue is string Text)
            {
                control.Dispatcher.BeginInvoke(() =>
                {
                    var dpi = VisualTreeHelper.GetDpi(control);
                    var typeface = new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch);
                    double maxWidth = control.ActualWidth - control.Margin.Left - control.Margin.Right;
                    double lineHeight = control.FontSize * 1.2;


                    //能完全单行显示
                    var textSingle = CreateFormattedText(Text, typeface, control.FontSize, control.Foreground, dpi.PixelsPerDip);
                    if (textSingle.Width <= maxWidth)
                    {
                        control.Text = Text;
                        return;
                    }

                    int breakIndex = FindBreakIndex(Text, typeface, control.FontSize, control.Foreground, maxWidth, dpi.PixelsPerDip);
                    string firstLine = Text.Substring(0, breakIndex).TrimEnd();
                    string secondLine = Text.Substring(breakIndex).TrimStart();
                    var ftFirst = CreateFormattedText(firstLine, typeface, control.FontSize, control.Foreground, dpi.PixelsPerDip);
                    var ftSecond = CreateFormattedText(secondLine, typeface, control.FontSize, control.Foreground, dpi.PixelsPerDip);

                    //如果分行后的第一行宽度小于最大宽度的一半  则在一行显示并追加...
                    if (ftFirst.Width < maxWidth * 0.5)
                    {
                        control.Margin = new Thickness(2, 0, 2, 0);
                        maxWidth = maxWidth - control.Margin.Left - control.Margin.Right;
                        string singleLineEllipsed = TrimWithEllipsis(Text, typeface, control.FontSize, control.Foreground, maxWidth, dpi.PixelsPerDip);
                        var ftSingle = CreateFormattedText(singleLineEllipsed, typeface, control.FontSize, control.Foreground, dpi.PixelsPerDip);
                        control.Text = ftSingle.Text;
                        return;
                    }
                    else
                    {
                        if (ftFirst.Width <= control.ActualWidth && ftSecond.Width <= control.ActualWidth)
                        {
                            // 两行完整显示
                            control.Text = ftFirst.Text;
                            control.Text += "\r\n" + ftSecond.Text;
                            return;
                        }

                        // Step 3: 第二行加省略号
                        string secondLineEllipsed = TrimWithEllipsis(secondLine, typeface, control.FontSize, control.Foreground, maxWidth, dpi.PixelsPerDip);
                        var ftSecondEllipsed = CreateFormattedText(secondLineEllipsed, typeface, control.FontSize, control.Foreground, dpi.PixelsPerDip);

                        control.Text = ftFirst.Text;
                        control.Text += "\r\n" + ftSecondEllipsed.Text;

                    }
                }, System.Windows.Threading.DispatcherPriority.Loaded);
            }
        }

        private static int FindBreakIndex(string text, Typeface typeface, double fontSize, Brush foreground, double maxWidth, double dpi)
        {
            double width = 0;
            int lastBreak = -1;

            for (int i = 0; i < text.Length; i++)
            {
                var ft = CreateFormattedText(text[i].ToString(), typeface, fontSize, foreground, dpi);
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

        private static bool IsBreakChar(char c)
        {
            // 常用断行符号（Windows 桌面图标默认断行位置）
            return char.IsWhiteSpace(c) || c == '-' || c == '_' || c == '.' || char.IsPunctuation(c);
        }


        // 加省略号裁剪文本，保证宽度不超
        private static string TrimWithEllipsis(string text, Typeface typeface, double fontSize, Brush foreground, double maxWidth, double dpi)
        {
            const string ellipsis = "...";

            for (int len = text.Length; len >= 0; len--)
            {
                string candidate = text.Substring(0, len) + ellipsis;
                var ft = CreateFormattedText(candidate, typeface, fontSize, foreground, dpi);
                if (ft.Width <= maxWidth)
                    return candidate;
            }

            return ellipsis;
        }

        private static FormattedText CreateFormattedText(string text, Typeface typeface, double emSize, Brush foreground, double dpi)
        {
            return new FormattedText(text, System.Globalization.CultureInfo.CurrentUICulture,
                FlowDirection.LeftToRight, typeface, emSize, foreground, dpi);
        }
    }
}
