using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ProjectStore.Models;
using ProjectStore.Services;

namespace ProjectStore.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly DataService _dataService;
    private Product _selectedProduct;
    private Category _selectedCategory;
    private string _statusMessage = "Добро пожаловать в интернет-магазин!";
    private string _searchText;

    public ObservableCollection<Product> Products { get; } = new();
    public ObservableCollection<Category> Categories { get; } = new();
    public ObservableCollection<OrderItem> CartItems { get; } = new();
    public ObservableCollection<Order> Orders { get; } = new();

    public Product SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            _selectedProduct = value;
            OnPropertyChanged(nameof(SelectedProduct));
            OnPropertyChanged(nameof(IsProductSelected));
        }
    }

    public Category SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            _selectedCategory = value;
            OnPropertyChanged(nameof(SelectedCategory));
            OnPropertyChanged(nameof(FilteredProducts));
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged(nameof(StatusMessage));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            _searchText = value;
            OnPropertyChanged(nameof(SearchText));
            OnPropertyChanged(nameof(FilteredProducts));
        }
    }

    public bool IsProductSelected => SelectedProduct != null;
    public bool IsCartEmpty => !CartItems.Any();
    public bool HasOrders => Orders.Any();

    public string CartSummary
    {
        get
        {
            var total = CartItems.Sum(item => item.TotalPrice);
            var count = CartItems.Sum(item => item.Quantity);
            return $"🛒 {count} товаров на сумму {total:C}";
        }
    }

    public IEnumerable<Product> FilteredProducts
    {
        get
        {
            var products = Products.AsEnumerable();
            /// <summary>
            /// Фильтрация по категории
            /// </summary>
            if (SelectedCategory != null && SelectedCategory.Id != 0)
                products = products.Where(p => p.Category?.Id == SelectedCategory.Id);
            /// <summary>
            /// Поиск по тексту
            /// </summary>
            if (!string.IsNullOrWhiteSpace(SearchText))
                products = products.Where(p => p.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                             p.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            return products;
        }
    }

    public ICommand AddToCartCommand { get; private set; }
    public ICommand RemoveFromCartCommand { get; private set; }
    public ICommand IncreaseQuantityCommand { get; private set; }
    public ICommand DecreaseQuantityCommand { get; private set; }
    public ICommand ClearCartCommand { get; private set; }
    public ICommand CheckoutCommand { get; private set; }
    public ICommand ViewOrderHistoryCommand { get; private set; }
    public ICommand SearchCommand { get; private set; }
    public ICommand ClearSearchCommand { get; private set; }

    public MainViewModel()
    {
        _dataService = new DataService();
        InitializeCommands(); 
        InitializeData();    
    }

    private void InitializeCommands()
    {
        AddToCartCommand = new RelayCommand(
            _ => AddToCart(null),
            _ => IsProductSelected && SelectedProduct?.StockQuantity > 0);

        RemoveFromCartCommand = new RelayCommand(
            RemoveFromCart,
            _ => !IsCartEmpty);

        IncreaseQuantityCommand = new RelayCommand(
            IncreaseQuantity,
            _ => !IsCartEmpty);

        DecreaseQuantityCommand = new RelayCommand(
            DecreaseQuantity,
            _ => !IsCartEmpty);

        ClearCartCommand = new RelayCommand(
            ClearCart,
            _ => !IsCartEmpty);

        CheckoutCommand = new RelayCommand(
            Checkout,
            _ => !IsCartEmpty);

        ViewOrderHistoryCommand = new RelayCommand(ViewOrderHistory);
        SearchCommand = new RelayCommand(Search);
        ClearSearchCommand = new RelayCommand(ClearSearch);
    }

    private void InitializeData()
    {
        /// <summary>
        /// Загрузка данных
        /// </summary>
        _dataService.InitializeSampleData();
        /// <summary>
        /// Заполнение коллекций
        /// </summary>
        foreach (var product in _dataService.Products)
            Products.Add(product);

        foreach (var category in _dataService.Categories)
            Categories.Add(category);
        /// <summary>
        /// Добавляем "Все категории"
        /// </summary>
        Categories.Insert(0, new Category { Id = 0, Name = "Все категории" });
        SelectedCategory = Categories[0];
    }

    private void AddToCart(object parameter)
    {
        if (SelectedProduct == null) return;

        var existingItem = CartItems.FirstOrDefault(item => item.Product.Id == SelectedProduct.Id);
        if (existingItem != null)
        {
            if (existingItem.Quantity < SelectedProduct.StockQuantity)
            {
                existingItem.Quantity++;
                StatusMessage = $"Товар \"{SelectedProduct.Name}\" добавлен в корзину!";
            }
            else
            {
                StatusMessage = $"Недостаточно товара \"{SelectedProduct.Name}\" на складе!";
                return;
            }
        }
        else
        {
            if (SelectedProduct.StockQuantity > 0)
            {
                CartItems.Add(new OrderItem(SelectedProduct, 1));
                StatusMessage = $"Товар \"{SelectedProduct.Name}\" добавлен в корзину!";
            }
            else
            {
                StatusMessage = $"Товар \"{SelectedProduct.Name}\" отсутствует на складе!";
                return;
            }
        }

        UpdateCart();
    }

    private void RemoveFromCart(object parameter)
    {
        if (parameter is OrderItem item)
        {
            CartItems.Remove(item);
            UpdateCart();
            StatusMessage = "Товар удален из корзины";
        }
    }

    private void IncreaseQuantity(object parameter)
    {
        Console.WriteLine($"IncreaseQuantity called with parameter: {parameter}");
        Console.WriteLine($"Parameter type: {parameter?.GetType().Name}");

        if (parameter is OrderItem item)
        {
            Console.WriteLine($"Item: {item.Product.Name}, Current quantity: {item.Quantity}");
            if (item.Quantity < item.Product.StockQuantity)
            {
                item.Quantity++;
                Console.WriteLine($"New quantity: {item.Quantity}");
                UpdateCart();
                StatusMessage = $"Количество '{item.Product.Name}' увеличено до {item.Quantity}";
            }
            else
            {
                StatusMessage = "Нельзя увеличить - достигнут максимум";
            }
        }
        else
        {
            Console.WriteLine("Parameter is not OrderItem!");
            StatusMessage = "Выберите товар в корзине";
        }
    }

    private void DecreaseQuantity(object parameter)
    {
        Console.WriteLine($"DecreaseQuantity called with parameter: {parameter}");

        if (parameter is OrderItem item)
        {
            Console.WriteLine($"Item: {item.Product.Name}, Current quantity: {item.Quantity}");
            if (item.Quantity > 1)
            {
                item.Quantity--;
                Console.WriteLine($"New quantity: {item.Quantity}");
                UpdateCart();
                StatusMessage = $"Количество '{item.Product.Name}' уменьшено до {item.Quantity}";
            }
            else
            {
                StatusMessage = "Нельзя уменьшить - минимальное количество 1";
            }
        }
        else
        {
            Console.WriteLine("Parameter is not OrderItem!");
            StatusMessage = "Выберите товар в корзине";
        }
    }

    private void ClearCart(object parameter)
    {
        if (MessageBox.Show("Очистить всю корзину?", "Подтверждение",
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            CartItems.Clear();
            UpdateCart();
            StatusMessage = "Корзина очищена";
        }
    }

    private void Checkout(object parameter)
    {
        if (IsCartEmpty)
        {
            StatusMessage = "Корзина пуста!";
            return;
        }

        var checkoutVm = new CheckoutViewModel(CartItems.ToList(), this);
        var checkoutWindow = new Views.CheckoutWindow { DataContext = checkoutVm };

        if (checkoutWindow.ShowDialog() == true)
        {
            /// <summary>
            ///Заказ успешно оформлен
            /// </summary>
            CartItems.Clear();
            UpdateCart();
            StatusMessage = "Заказ успешно оформлен!";
        }
    }

    private void ViewOrderHistory(object parameter)
    {
        var ordersVm = new OrdersViewModel(Orders.ToList());
        var ordersWindow = new Views.OrdersWindow { DataContext = ordersVm };
        ordersWindow.ShowDialog();
    }

    private void Search(object parameter)
    {
        OnPropertyChanged(nameof(FilteredProducts));
        StatusMessage = $"Найдено товаров: {FilteredProducts.Count()}";
    }

    private void ClearSearch(object parameter)
    {
        SearchText = string.Empty;
        SelectedCategory = Categories[0];
        StatusMessage = "Поиск очищен";
    }

    private void UpdateCart()
    {
        /// <summary>
        /// Создаем новую коллекцию чтобы заставить WPF обновиться
        /// </summary>
        var temp = CartItems.ToList();
        CartItems.Clear();
        foreach (var item in temp)
        {
            CartItems.Add(item);
        }

        OnPropertyChanged(nameof(CartSummary));
        OnPropertyChanged(nameof(IsCartEmpty));
        OnPropertyChanged(nameof(CartItems));

        /// <summary>
        /// Принудительно обновляем отображение
        /// </summary>
        CommandManager.InvalidateRequerySuggested();

        foreach (var product in Products)
        {
            product.OnPropertyChanged(nameof(product.StockQuantity));
            product.OnPropertyChanged(nameof(product.DisplayInfo));
            product.OnPropertyChanged(nameof(product.Details));
        }

        /// <summary>
        /// Обновляем фильтрованные товары
        /// </summary>
        OnPropertyChanged(nameof(FilteredProducts));
    }

    public void AddOrder(Order order)
    {
        Orders.Add(order);
        OnPropertyChanged(nameof(HasOrders));
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void UpdateProductStockDisplay()
    {
        /// <summary>
        /// Принудительно обновляем отображение всех товаров
        /// </summary>
        foreach (var product in Products)
        {
            product.OnPropertyChanged(nameof(product.StockQuantity));
        }
        OnPropertyChanged(nameof(FilteredProducts));
        /// <summary>
        /// Обновляем команды, которые зависят от наличия товаров
        /// </summary>
        CommandManager.InvalidateRequerySuggested();
    }


}