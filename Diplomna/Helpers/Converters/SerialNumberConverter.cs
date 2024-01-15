using System;
using System.Windows.Data;

namespace Diplomna.Helpers.Converters
{
    public class SerialNumberConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value!= null && !string.IsNullOrEmpty(value.ToString()))
                return "(" + value.ToString() + ")";

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.Empty;
        }

        #endregion
    }
}
