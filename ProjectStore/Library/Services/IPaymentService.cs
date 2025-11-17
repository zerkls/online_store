namespace ProjectStore.Services
{
    public interface IPaymentService
    {
        bool ProcessPayment(decimal amount);
        string GetPaymentMethodName();
    }
}