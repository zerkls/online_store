using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfShopApp.Models;
/// <summary>
/// Элемент заказа - демонстрирует композицию
/// </summary>
public class OrderItem : INotifyPropertyChanged
{
    private Product _product;
    private int _quantity;

    public Product Product
    {
        get => _product;
        set
        {
            _product = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(UnitPrice));
            OnPropertyChanged(nameof(TotalPrice));
        }
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            _quantity = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalPrice));
        }
    }
    /// <summary>
    /// Цена за единицу (делегирование к Product)
    /// </summary>
    public decimal UnitPrice => Product?.Price ?? 0;
    /// <summary>
    /// Общая стоимость позиции
    /// </summary>
    public decimal TotalPrice => UnitPrice * Quantity;

    public OrderItem() { }

    public OrderItem(Product product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }
    /// <summary>
    /// Увеличить количество
    /// </summary>
    public void IncreaseQuantity(int amount = 1)
    {
        Quantity += amount;
    }
    /// <summary>
    /// Уменьшить количество
    /// </summary>
    public bool DecreaseQuantity(int amount = 1)
    {
        if (Quantity - amount >= 1)
        {
            Quantity -= amount;
            return true;
        }
        return false;
    }

    public string DisplayInfo => $"{Product?.Name} x {Quantity} = {TotalPrice:C}";

    public string DetailedInfo => $"{Product?.Name}\n" +
                                 $"Количество: {Quantity}\n" +
                                 $"Цена: {UnitPrice:C}\n" +
                                 $"Итого: {TotalPrice:C}";

    public override string ToString()
    {
        return DisplayInfo;
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}