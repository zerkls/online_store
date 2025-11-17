using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ProjectStore.Models.Enums;

namespace ProjectStore.Converters;

/// <summary>
/// Конвертер для преобразования статуса заказа в цвет
/// </summary>
public class OrderStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Pending => new SolidColorBrush(Color.FromRgb(255, 193, 7)),     // Желтый
                OrderStatus.Processing => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Синий
                OrderStatus.Completed => new SolidColorBrush(Color.FromRgb(76, 175, 80)),   // Зеленый
                OrderStatus.Cancelled => new SolidColorBrush(Color.FromRgb(244, 67, 54)),   // Красный
                OrderStatus.Shipped => new SolidColorBrush(Color.FromRgb(156, 39, 176)),    // Фиолетовый
                OrderStatus.PaymentFailed => new SolidColorBrush(Color.FromRgb(121, 85, 72)), // Коричневый
                _ => new SolidColorBrush(Colors.Gray)
            };
        }

        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}