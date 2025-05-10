using Desktop.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace Desktop
{
    public static class ElementExtension
    {
        public static T GetRootElement<T>(UIElement element) where T:class
        {
            // 通过 VisualTreeHelper 查找父节点
            while (true)
            {
                DependencyObject parent = VisualTreeHelper.GetParent(element);
                if (parent == null)
                {
                    // 找到根节点
                    return null;
                }

                // 检查父节点是否为 Window
                if (parent is T)
                {
                    return parent as T;
                }

                // 继续向上查找
                element = parent as UIElement;
            }
        }
    }
}
