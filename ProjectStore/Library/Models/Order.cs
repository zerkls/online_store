using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfShopApp.Models.Enums;
using WpfShopApp.Services;

namespace WpfShopApp.Models;

public class Order : Entity, INotifyPropertyChanged
{
    private static int _nextOrderId = 1;

    private Customer _customer;
    private DateTime _orderDate;
    private OrderStatus _status;
    private IPaymentService _paymentMethod;

    public Customer Customer
    {
        get => _customer;
        set
        {
            _customer = value;
            OnPropertyChanged();
        }
    }

    public DateTime OrderDate
    {
        get => _orderDate;
        set
        {
            _orderDate = value;
            OnPropertyChanged();
        }
    }

    public OrderStatus Status
    {
        get => _status;
        set
        {
            _status = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusDescription));
            OnPropertyChanged(nameof(CanBeCancelled));
        }
    }

    // Элементы заказа (композиция)
    public ObservableCollection<OrderItem> Items { get; private set; }

    public IPaymentService PaymentMethod
    {
        get => _paymentMethod;
        set
        {
            _paymentMethod = value;
            OnPropertyChanged();
        }
    }

    // Общая стоимость без скидки
    public decimal Subtotal => Items.Sum(item => item.TotalPrice);

    // Размер скидки (инкапсуляция логики расчета)
    public decimal DiscountAmount => CalculateDiscount();

    // Итоговая стоимость
    public decimal TotalAmount => Subtotal - DiscountAmount;

    // Описание статуса заказа
    public string StatusDescription => GetStatusDescription();

    // Можно ли отменить заказ
    public bool CanBeCancelled => Status == OrderStatus.Pending ||
                                 Status == OrderStatus.Processing ||
                                 Status == OrderStatus.Completed;

    public Order()
    {
        Id = _nextOrderId++;
        OrderDate = DateTime.Now;
        Status = OrderStatus.Pending;
        Items = new ObservableCollection<OrderItem>();

        Items.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(DiscountAmount));
            OnPropertyChanged(nameof(TotalAmount));
        };
    }

    public Order(Customer customer) : this()
    {
        Customer = customer;
    }

    // Добавить товар в заказ
    public bool AddItem(Product product, int quantity)
    {
        if (product == null || quantity <= 0)
            return false;

        if (product.StockQuantity < quantity)
            return false;

        var existingItem = Items.FirstOrDefault(item => item.Product.Id == product.Id);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            Items.Add(new OrderItem(product, quantity));
        }

        return true;
    }

    // Удалить товар из заказа
    public bool RemoveItem(int productId)
    {
        var item = Items.FirstOrDefault(i => i.Product.Id == productId);
        if (item != null)
        {
            Items.Remove(item);
            return true;
        }
        return false;
    }

    // Расчет скидки (инкапсуляция бизнес-логики)
    private decimal CalculateDiscount()
    {
        decimal discount = 0;

        // Скидка 10% для заказов свыше 5000 руб.
        if (Subtotal > 5000)
        {
            discount += Subtotal * 0.1m;
        }

        // Дополнительная скидка 5% для постоянных клиентов
        if (Customer?.IsRegularCustomer == true)
        {
            discount += Subtotal * 0.05m;
        }

        // Максимальная скидка не более 30%
        decimal maxDiscount = Subtotal * 0.3m;
        return Math.Min(discount, maxDiscount);
    }


    // Получить описание статуса
    private string GetStatusDescription()
    {
        return Status switch
        {
            OrderStatus.Pending => "Ожидает обработки",
            OrderStatus.Processing => "В обработке",
            OrderStatus.Completed => "Завершен",
            OrderStatus.Cancelled => "Отменен",
            OrderStatus.Shipped => "Отправлен",
            OrderStatus.PaymentFailed => "Ошибка оплаты",
            _ => "Неизвестный статус"
        };
    }

    // Получить информацию о заказе
    public string GetOrderInfo()
    {
        return $"Заказ #{Id} от {OrderDate:dd.MM.yyyy} - {TotalAmount:C} ({StatusDescription})";
    }

    // Получить детальную информацию о заказе
    public string GetDetailedInfo()
    {
        var itemsInfo = string.Join("\n", Items.Select(item => $"  {item.DetailedInfo}"));

        return $"Заказ #{Id}\n" +
               $"Дата: {OrderDate:dd.MM.yyyy HH:mm}\n" +
               $"Статус: {StatusDescription}\n" +
               $"Покупатель: {Customer?.Name}\n" +
               $"Способ оплаты: {PaymentMethod?.GetPaymentMethodName() ?? "Не выбран"}\n\n" +
               $"Состав заказа:\n{itemsInfo}\n\n" +
               $"Подитог: {Subtotal:C}\n" +
               $"Скидка: {DiscountAmount:C}\n" +
               $"ИТОГО: {TotalAmount:C}";
    }

    public override string ToString()
    {
        return GetOrderInfo();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public bool ProcessOrder()
    {
        // Проверка доступности товаров
        foreach (var item in Items)
        {
            if (item.Product.StockQuantity < item.Quantity)
            {
                Status = OrderStatus.Cancelled;
                return false;
            }
        }

        // Резервирование товаров
        foreach (var item in Items)
        {
            item.Product.StockQuantity -= item.Quantity;
        }

        // Обработка оплаты
        if (PaymentMethod.ProcessPayment(TotalAmount))
        {
            Status = OrderStatus.Completed;
            return true;
        }
        else
        {
            // Возврат товаров при неудачной оплате
            foreach (var item in Items)
            {
                item.Product.StockQuantity += item.Quantity;
            }
            Status = OrderStatus.Cancelled;
            return false;
        }
    }

    public bool CancelOrder()
    {
        if (Status != OrderStatus.Completed && Status != OrderStatus.Processing)
            return false;

        // Возврат товаров на склад
        foreach (var item in Items)
        {
            item.Product.StockQuantity += item.Quantity;
        }

        Status = OrderStatus.Cancelled;
        return true;
    }
}