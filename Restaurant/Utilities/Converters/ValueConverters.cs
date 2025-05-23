using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RestaurantManager.Utilities.Converters
{
    /// <summary>
    /// Standard boolean to visibility converter
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }
            return false;
        }
    }

    /// <summary>
    /// Inverse boolean to visibility converter
    /// </summary>
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Collapsed;
            }
            return false;
        }
    }

    /// <summary>
    /// Number to visibility converter (shows element if number > 0)
    /// </summary>
    public class NumberToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter?.ToString() == "inverse";
            bool visible = false;
            
            if (value is int intValue)
            {
                visible = intValue > 0;
            }
            else if (value is decimal decimalValue)
            {
                visible = decimalValue > 0;
            }
            else if (value is double doubleValue)
            {
                visible = doubleValue > 0;
            }
            else if (value is long longValue)
            {
                visible = longValue > 0;
            }
            
            if (inverse)
            {
                visible = !visible;
            }
            
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Null to visibility converter (shows element if value is not null)
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverse = parameter?.ToString() == "inverse";
            bool visible = value != null && !string.Empty.Equals(value);
            
            if (inverse)
            {
                visible = !visible;
            }
            
            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Boolean to text converter (for showing different text based on boolean value)
    /// </summary>
    public class BooleanToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string options)
            {
                string[] parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return boolValue ? parts[0] : parts[1];
                }
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Boolean to color converter (for changing colors based on boolean value)
    /// </summary>
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && parameter is string options)
            {
                string[] parts = options.Split('|');
                if (parts.Length == 2)
                {
                    return boolValue ? parts[0] : parts[1];
                }
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Price converter (formats price with currency symbol)
    /// </summary>
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Use the Romanian culture for formatting
            CultureInfo romanianCulture = new CultureInfo("ro-RO");
            
            if (value is decimal decimalValue)
            {
                return string.Format(romanianCulture, "{0:C}", decimalValue);
            }
            else if (value is double doubleValue)
            {
                return string.Format(romanianCulture, "{0:C}", doubleValue);
            }
            else if (value is int intValue)
            {
                return string.Format(romanianCulture, "{0:C}", intValue);
            }
            else if (value is float floatValue)
            {
                return string.Format(romanianCulture, "{0:C}", floatValue);
            }
            
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                // Remove the currency symbol and any other non-numeric characters
                string numericString = stringValue.Replace(CultureInfo.CurrentCulture.NumberFormat.CurrencySymbol, "")
                    .Replace(",", ".")
                    .Trim();
                
                if (decimal.TryParse(numericString, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
                {
                    return result;
                }
            }
            
            return 0m;
        }
    }
}