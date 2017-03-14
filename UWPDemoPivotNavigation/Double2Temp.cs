using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWPDemoPivotNavigation
{
    /// <summary>
    /// Convert double value to string
    /// </summary>
    class Double2Temp : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            var d = (double)value;
            return string.Format("{0} °C", Math.Round(d, 2));
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
