using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WpfShopApp.Models;

public class Customer : Entity, INotifyPropertyChanged
{
    private string _email;
    private string _phone;
    private string _address;

    public string Email
    {
        get => _email;
        set
        {
            _email = value;
            OnPropertyChanged();
        }
    }

    public string Phone
    {
        get => _phone;
        set
        {
            _phone = value;
            OnPropertyChanged();
        }
    }

    public string Address
    {
        get => _address;
        set
        {
            _address = value;
            OnPropertyChanged();
        }
    }

    // История заказов покупателя (композиция)
    public ObservableCollection<Order> OrderHistory { get; private set; }

    // Общее количество заказов
    public int TotalOrders => OrderHistory.Count;

    // Общая сумма всех заказов
    public decimal TotalSpent => OrderHistory.Sum(order => order.TotalAmount);

    public Customer()
    {
        OrderHistory = new ObservableCollection<Order>();
    }

    public Customer(int id, string name, string email) : this()
    {
        Id = id;
        Name = name;
        Email = email;
    }

    // Добавить заказ в историю
    public void AddOrderToHistory(Order order)
    {
        OrderHistory.Add(order);
        OnPropertyChanged(nameof(TotalOrders));
        OnPropertyChanged(nameof(TotalSpent));
    }

    // Получить статистику по заказам
    public string GetOrderStatistics()
    {
        return $"Заказов: {TotalOrders} | Общая сумма: {TotalSpent:C}";
    }

    // Проверить, является ли покупатель постоянным клиентом
    public bool IsRegularCustomer => TotalOrders >= 3;

    public override string ToString()
    {
        return $"{Name} ({Email}) - {GetOrderStatistics()}";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}