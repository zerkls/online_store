using System;

namespace ProjectStore.Services
{
    public class CreditCardPayment : IPaymentService
    {
        public string CardNumber { get; set; }
        public string CardHolder { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }

        public bool ProcessPayment(decimal amount)
        {
            // Симуляция обработки платежа
            Random rnd = new Random();
            return rnd.Next(100) < 90;
        }

        public string DisplayName => "💳 Банковская карта";

        public string GetPaymentMethodName()
        {
            return "💳 Банковская карта";
        }
    }
}