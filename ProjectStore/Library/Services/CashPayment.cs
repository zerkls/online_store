using System;

namespace WpfShopApp.Services;

public class CashPayment : IPaymentService
{
    public string PaymentType { get; set; } = "Наличные";

    public bool ProcessPayment(decimal amount)
    {
        // Симуляция обработки наличного платежа
        Console.WriteLine($"💵 Обработка наличного платежа на сумму: {amount:C}");

        // Для наличных платежей всегда возвращаем true
        // В реальном приложении здесь могла бы быть логика
        // подтверждения получения наличных
        return true;
    }

    public string GetPaymentMethodName()
    {
        return "💵 Наличные при получении";
    }

    public string GetPaymentDetails()
    {
        return $"Тип оплаты: {PaymentType}\nОплата производится при получении заказа";
    }

    public string DisplayName => "💵 Наличные при получении";
}