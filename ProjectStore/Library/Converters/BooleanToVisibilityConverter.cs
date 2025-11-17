using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ProjectStore.Converters;

/// <summary>
/// Конвертер для преобразования bool в Visibility и обратно
/// Демонстрирует принцип инкапсуляции логики преобразования данных
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Если true, инвертирует логику преобразования
    /// </summary>
    public bool IsInverted { get; set; }

    /// <summary>
    /// Если true, скрывает элемент вместо его коллапсирования
    /// </summary>
    public bool UseHidden { get; set; }

    /// <summary>
    /// Преобразует bool в Visibility
    /// </summary>
    /// <param name="value">bool значение</param>
    /// <param name="targetType">Целевой тип</param>
    /// <param name="parameter">Дополнительный параметр</param>
    /// <param name="culture">Культура</param>
    /// <returns>Visibility значение</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            // Инвертируем логику если нужно
            if (IsInverted)
            {
                boolValue = !boolValue;
            }

            // Возвращаем соответствующий Visibility
            return boolValue ? Visibility.Visible :
                (UseHidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        // Если значение не bool, возвращаем Collapsed
        return UseHidden ? Visibility.Hidden : Visibility.Collapsed;
    }

    /// <summary>
    /// Преобразует Visibility обратно в bool
    /// </summary>
    /// <param name="value">Visibility значение</param>
    /// <param name="targetType">Целевой тип</param>
    /// <param name="parameter">Дополнительный параметр</param>
    /// <param name="culture">Культура</param>
    /// <returns>bool значение</returns>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility)
        {
            bool result = visibility == Visibility.Visible;

            // Инвертируем логику если нужно
            if (IsInverted)
            {
                result = !result;
            }

            return result;
        }

        return false;
    }
}