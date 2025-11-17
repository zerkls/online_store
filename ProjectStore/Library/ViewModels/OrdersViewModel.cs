using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WpfShopApp.Models;

namespace WpfShopApp.ViewModels;

public class OrdersViewModel : INotifyPropertyChanged
{
    private Order _selectedOrder;

    public List<Order> Orders { get; }
    public Order SelectedOrder
    {
        get => _selectedOrder;
        set
        {
            _selectedOrder = value;
            OnPropertyChanged(nameof(SelectedOrder));
            OnPropertyChanged(nameof(OrderDetails));
            OnPropertyChanged(nameof(IsOrderSelected));
        }
    }

    public string OrderDetails => SelectedOrder?.GetDetailedInfo() ?? "Выберите заказ для просмотра деталей";
    public bool IsOrderSelected => SelectedOrder != null;

    public string OrdersSummary => $"Всего заказов: {Orders.Count} | " +
                                 $"Общая сумма: {Orders.Sum(o => o.TotalAmount):C}";

    public ICommand CloseCommand { get; }
    public ICommand CancelOrderCommand { get; }

    public OrdersViewModel(List<Order> orders)
    {
        Orders = orders.OrderByDescending(o => o.OrderDate).ToList();

        CloseCommand = new RelayCommand(Close);
        CancelOrderCommand = new RelayCommand(CancelOrder, _ => IsOrderSelected && SelectedOrder.CanBeCancelled);
    }

    private void Close(object parameter)
    {
        System.Windows.Application.Current.Windows.OfType<Views.OrdersWindow>()
            .FirstOrDefault()?.Close();
    }

    private void CancelOrder(object parameter)
    {
        if (SelectedOrder?.CancelOrder() == true)
        {
            OnPropertyChanged(nameof(SelectedOrder));
            OnPropertyChanged(nameof(OrderDetails));
            System.Windows.MessageBox.Show("Заказ успешно отменен", "Успех",
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}