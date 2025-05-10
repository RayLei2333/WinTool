using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Desktop.Converters
{
    public class ValueEqualsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 判断绑定的值是否等于参数值，返回对应的布尔值
            //return value == parameter;
            return value?.ToString().Equals(parameter) ?? false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //throw new NotImplementedException();
            return value;
        }
    }
}
