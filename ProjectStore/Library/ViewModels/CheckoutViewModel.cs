using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfShopApp.Models;
using WpfShopApp.Services;

namespace WpfShopApp.ViewModels;

public class CheckoutViewModel : INotifyPropertyChanged
{
    private readonly MainViewModel _mainViewModel;
    private IPaymentService _selectedPaymentMethod;
    private string _cardNumber;
    private string _cardHolder;
    private string _expiryDate;
    private string _cvv;
    private string _phoneNumber;

    public List<OrderItem> CartItems { get; }
    public Customer CurrentCustomer { get; }
    public List<IPaymentService> PaymentMethods { get; }

    public decimal Subtotal => CartItems.Sum(item => item.TotalPrice);
    public decimal DiscountAmount => CalculateDiscount();
    public decimal TotalAmount => Subtotal - DiscountAmount;

    public string OrderSummary => $@"Товаров: {CartItems.Sum(item => item.Quantity)}
Подитог: {Subtotal:C}
Скидка: {DiscountAmount:C}
ИТОГО: {TotalAmount:C}";

    public IPaymentService SelectedPaymentMethod
    {
        get => _selectedPaymentMethod;
        set
        {
            _selectedPaymentMethod = value;
            OnPropertyChanged(nameof(SelectedPaymentMethod));
            OnPropertyChanged(nameof(IsCardPayment));
            OnPropertyChanged(nameof(IsEWalletPayment));
            OnPropertyChanged(nameof(IsCashPayment));
        }
    }

    public string CardNumber
    {
        get => _cardNumber;
        set { _cardNumber = value; OnPropertyChanged(nameof(CardNumber)); }
    }

    public string CardHolder
    {
        get => _cardHolder;
        set { _cardHolder = value; OnPropertyChanged(nameof(CardHolder)); }
    }

    public string ExpiryDate
    {
        get => _expiryDate;
        set { _expiryDate = value; OnPropertyChanged(nameof(ExpiryDate)); }
    }

    public string CVV
    {
        get => _cvv;
        set { _cvv = value; OnPropertyChanged(nameof(CVV)); }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set { _phoneNumber = value; OnPropertyChanged(nameof(PhoneNumber)); }
    }

    public bool IsCardPayment => SelectedPaymentMethod is CreditCardPayment;
    public bool IsEWalletPayment => SelectedPaymentMethod is EWalletPayment;

    public ICommand CompleteOrderCommand { get; }
    public ICommand CancelCommand { get; }

    public CheckoutViewModel(List<OrderItem> cartItems, MainViewModel mainViewModel)
    {
        CartItems = cartItems;
        _mainViewModel = mainViewModel;


        PaymentMethods = new List<IPaymentService>
        {
            new CreditCardPayment(),
            new CashPayment(),
            new EWalletPayment { WalletType = "Qiwi" }
        };

        SelectedPaymentMethod = PaymentMethods[0];

        CompleteOrderCommand = new RelayCommand(CompleteOrder);
        CancelCommand = new RelayCommand(Cancel);
    }

    private decimal CalculateDiscount()
    {
        decimal discount = 0;
        /// <summary>
        /// Скидка 10% для заказов свыше 5000 руб.
        /// </summary>
        if (Subtotal > 5000)
            discount += Subtotal * 0.1m;

        return discount;
    }

    private void CompleteOrder(object parameter)
    {
        /// <summary>
        /// Настройка выбранного метода оплаты
        /// </summary>
        if (SelectedPaymentMethod is CreditCardPayment cardPayment)
        {
            if (string.IsNullOrWhiteSpace(CardNumber) || string.IsNullOrWhiteSpace(CardHolder))
            {
                MessageBox.Show("Заполните все поля банковской карты", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            cardPayment.CardNumber = CardNumber;
            cardPayment.CardHolder = CardHolder;
            cardPayment.ExpiryDate = ExpiryDate;
            cardPayment.CVV = CVV;
        }
        else if (SelectedPaymentMethod is EWalletPayment eWalletPayment)
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber))
            {
                MessageBox.Show("Введите номер телефона", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            eWalletPayment.PhoneNumber = PhoneNumber;
        }
        /// <summary>
        /// Создание и обработка заказа
        /// </summary>
        var order = new Order(CurrentCustomer)
        {
            PaymentMethod = SelectedPaymentMethod
        };

        foreach (var item in CartItems)
        {
            order.AddItem(item.Product, item.Quantity);
        }

        if (order.ProcessOrder())
        {
            _mainViewModel.AddOrder(order);
            /// <summary>
            /// Обновляем отображение запасов в главном окне
            /// </summary>
            _mainViewModel.UpdateProductStockDisplay();

            MessageBox.Show($"Заказ #{order.Id} успешно оформлен!\nСумма: {order.TotalAmount:C}",
                          "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            /// <summary>
            /// Закрытие окна с успешным результатом
            /// </summary>
            CloseWindow(true);
        }
        else
        {
            MessageBox.Show("Ошибка при оформлении заказа. Проверьте доступность товаров.",
                          "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Cancel(object parameter)
    {
        CloseWindow(false);
    }

    private void CloseWindow(bool dialogResult)
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.DialogResult = dialogResult;
                window.Close();
                break;
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool IsCashPayment => SelectedPaymentMethod is CashPayment;
}