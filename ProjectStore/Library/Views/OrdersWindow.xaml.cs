using System.Linq;
using System.Windows;
using ProjectStore.ViewModels;

namespace ProjectStore.Views;

public partial class OrdersWindow : Window
{
    public OrdersWindow()
    {
        InitializeComponent();
        Loaded += OrdersWindow_Loaded;
    }

    private void OrdersWindow_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is OrdersViewModel vm && vm.Orders.Any() && vm.SelectedOrder == null)
        {
            vm.SelectedOrder = vm.Orders.First();
        }
    }
}