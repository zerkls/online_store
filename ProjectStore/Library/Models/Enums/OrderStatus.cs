namespace WpfShopApp.Models.Enums;

public enum OrderStatus
{
    // Ожидает обработки
    Pending = 0,

    // В обработке
    Processing = 1,

    // Завершен
    Completed = 2,

    // Отменен
    Cancelled = 3,

    // Отправлен
    Shipped = 4,

    // Ошибка оплаты
    PaymentFailed = 5
}