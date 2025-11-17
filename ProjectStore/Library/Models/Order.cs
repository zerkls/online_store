using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ProjectStore.Models.Enums;
using ProjectStore.Services;

namespace ProjectStore.Models;

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

    /// <summary>
    /// Элементы заказа (композиция)
    /// </summary>
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

    /// <summary>
    /// Общая стоимость без скидки
    /// </summary>
    public decimal Subtotal => Items.Sum(item => item.TotalPrice);

    /// <summary>
    /// Размер скидки (инкапсуляция логики расчета)
    /// </summary>
    public decimal DiscountAmount => CalculateDiscount();

    /// <summary>
    /// Итоговая стоимость
    /// </summary>
    public decimal TotalAmount => Subtotal - DiscountAmount;

    /// <summary>
    /// Описание статуса заказа
    /// </summary>
    public string StatusDescription => GetStatusDescription();

    /// <summary>
    /// Можно ли отменить заказ
    /// </summary>
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

    /// </summary>
    /// Добавить товар в заказ
    /// </summary>
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

    /// <summary>
    /// Удалить товар из заказа
    /// </summary>
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

    /// <summary>
    /// Расчет скидки 
    /// </summary>
    private decimal CalculateDiscount()
    {
        decimal discount = 0;

        /// <summary>
        /// Скидка 10% для заказов свыше 5000 руб.
        /// </summary>
        if (Subtotal > 5000)
        {
            discount += Subtotal * 0.1m;
        }
        /// <summary>
        /// Дополнительная скидка 5% для постоянных клиентов
        /// <summary>
        if (Customer?.IsRegularCustomer == true)
        {
            discount += Subtotal * 0.05m;
        }

        /// <summary>
        /// Максимальная скидка не более 30%
        /// <summary>
        decimal maxDiscount = Subtotal * 0.3m;
        return Math.Min(discount, maxDiscount);
    }

    /// <summary>
    /// Получить описание статуса
    /// </summary>
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

    /// <summary>
    /// Получить информацию о заказе
    /// </summary>
    public string GetOrderInfo()
    {
        return $"Заказ #{Id} от {OrderDate:dd.MM.yyyy} - {TotalAmount:C} ({StatusDescription})";
    }

    /// <summary>
    /// Получить детальную информацию о заказе
    /// </summary>
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
        /// </summary>
        /// Проверка доступности товаров
        /// </summary>
        foreach (var item in Items)
        {
            if (item.Product.StockQuantity < item.Quantity)
            {
                Status = OrderStatus.Cancelled;
                return false;
            }
        }
        /// </summary>
        /// Резервирование товаров
        /// </summary>
        foreach (var item in Items)
        {
            item.Product.StockQuantity -= item.Quantity;
        }
        /// </summary>
        /// Обработка оплаты
        /// </summary>
        if (PaymentMethod.ProcessPayment(TotalAmount))
        {
            Status = OrderStatus.Completed;
            return true;
        }
        else
        {
            /// </summary>
            /// Возврат товаров при неудачной оплате
            /// </summary>
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
        /// </summary>
        /// Возврат товаров на склад
        /// </summary>
        foreach (var item in Items)
        {
            item.Product.StockQuantity += item.Quantity;
        }

        Status = OrderStatus.Cancelled;
        return true;
    }
}