using System;

namespace ProjectStore.Services;

/// <summary>
/// Реализация оплаты через электронный кошелек
/// Демонстрирует принцип полиморфизма через интерфейс IPaymentService
/// </summary>
public class EWalletPayment : IPaymentService
{
    public string WalletType { get; set; } = "Qiwi";
    public string PhoneNumber { get; set; }
    public string PaymentType { get; set; } = "Электронный кошелек";

    public bool ProcessPayment(decimal amount)
    {
        // Симуляция обработки платежа через электронный кошелек
        Console.WriteLine($"📱 Обработка платежа через {WalletType}");
        Console.WriteLine($"📱 Номер телефона: {PhoneNumber}");
        Console.WriteLine($"📱 Сумма: {amount:C}");

        // В реальном приложении здесь был бы вырос API платежной системы
        bool paymentSuccess = SimulateEWalletProcessing();

        if (paymentSuccess)
        {
            Console.WriteLine($"✅ Платеж через {WalletType} успешно обработан!");
            return true;
        }
        else
        {
            Console.WriteLine($"❌ Ошибка обработки платежа через {WalletType}");
            return false;
        }
    }

    public string GetPaymentMethodName()
    {
        return $"📱 {WalletType} ({PhoneNumber ?? "номер не указан"})";
    }

    public string GetPaymentDetails()
    {
        return $"Тип оплаты: {PaymentType}\n" +
               $"Кошелек: {WalletType}\n" +
               $"Номер телефона: {PhoneNumber ?? "не указан"}";
    }

    private bool SimulateEWalletProcessing()
    {
        // Симуляция: 95% успешных платежей для электронных кошельков
        Random rnd = new Random();
        return rnd.Next(100) < 95;
    }

    public string DisplayName => $"📱 Электронный кошелек ({WalletType})";
}