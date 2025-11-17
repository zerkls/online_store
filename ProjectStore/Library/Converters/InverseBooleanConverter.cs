using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectStore.Converters;

/// <summary>
/// Конвертер для инвертирования bool значений
/// Демонстрирует принцип повторного использования кода через конвертеры
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return false;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }

        return false;
    }
}