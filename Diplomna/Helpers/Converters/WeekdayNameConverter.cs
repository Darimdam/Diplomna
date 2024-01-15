using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Diplomna.Helpers.Converters
{
    public class WeekdayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var day = value as string;

            switch (day)
            {
                case "Monday":
                    return "Понеделник";
                case "Tuesday":
                    return "Вторник";
                case "Wednesday":
                    return "Сряда";
                case "Thursday":
                    return "Четвъртък";
                case "Friday":
                    return "Петък";
                case "Saturday":
                    return "Събота";
                case "Sunday":
                    return "Неделя";
            }

            return day;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
}
