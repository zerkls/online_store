using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ProjectStore.Models;

namespace ProjectStore.Services;

/// <summary>
/// Сервис для работы с данными приложения
/// Демонстрирует принципы инкапсуляции и абстракции
/// </summary>
public class DataService
{
    private readonly string _dataDirectory;
    private readonly string _productsFile;
    private readonly string _customersFile;
    private readonly string _ordersFile;
    private readonly string _categoriesFile;

    public ObservableCollection<Product> Products { get; private set; }
    public ObservableCollection<Customer> Customers { get; private set; }
    public ObservableCollection<Order> Orders { get; private set; }
    public ObservableCollection<Category> Categories { get; private set; }

    public DataService()
    {
        _dataDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");
        _productsFile = Path.Combine(_dataDirectory, "products.json");
        _customersFile = Path.Combine(_dataDirectory, "customers.json");
        _ordersFile = Path.Combine(_dataDirectory, "orders.json");
        _categoriesFile = Path.Combine(_dataDirectory, "categories.json");

        Products = new ObservableCollection<Product>();
        Customers = new ObservableCollection<Customer>();
        Orders = new ObservableCollection<Order>();
        Categories = new ObservableCollection<Category>();

        EnsureDataDirectoryExists();
    }

    /// <summary>
    /// Создает директорию для данных если она не существует
    /// </summary>
    private void EnsureDataDirectoryExists()
    {
        if (!Directory.Exists(_dataDirectory))
        {
            Directory.CreateDirectory(_dataDirectory);
        }
    }

    /// <summary>
    /// Инициализирует начальные данные при первом запуске
    /// </summary>
    public void InitializeSampleData()
    {
        if (Categories.Count == 0)
        {
            var categories = new[]
            {
                new Category { Id = 1, Name = "Электроника", Description = "Современные гаджеты и устройства" },
                new Category { Id = 2, Name = "Книги", Description = "Художественная и учебная литература" },
                new Category { Id = 3, Name = "Одежда", Description = "Мужская и женская одежда" },
                new Category { Id = 4, Name = "Спорт", Description = "Спортивные товары и инвентарь" },
                new Category { Id = 5, Name = "Красота", Description = "Косметика и уходовые средства" }
            };

            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        if (Products.Count == 0)
        {
            var products = new[]
            {
                new Product { Id = 1, Name = "Смартфон Samsung", Price = 29999, StockQuantity = 15, Category = Categories[0], Description = "Флагманский смартфон с AMOLED дисплеем" },
                new Product { Id = 2, Name = "Ноутбук ASUS", Price = 59999, StockQuantity = 8, Category = Categories[0], Description = "Игровой ноутбук с RTX видеокартой" },
                new Product { Id = 3, Name = "Наушники Sony", Price = 7999, StockQuantity = 25, Category = Categories[0], Description = "Беспроводные наушники с шумоподавлением" },
                new Product { Id = 4, Name = "Научная фантастика", Price = 499, StockQuantity = 50, Category = Categories[1], Description = "Сборник лучших научно-фантастических произведений" },
                new Product { Id = 5, Name = "Учебник по C#", Price = 1299, StockQuantity = 30, Category = Categories[1], Description = "Подробное руководство по программированию на C#" },
                new Product { Id = 6, Name = "Футболка хлопковая", Price = 999, StockQuantity = 100, Category = Categories[2], Description = "Удобная хлопковая футболка различных цветов" },
                new Product { Id = 7, Name = "Джинсы классические", Price = 2999, StockQuantity = 40, Category = Categories[2], Description = "Классические джинсы прямого кроя" },
                new Product { Id = 8, Name = "Фитнес-браслет", Price = 2499, StockQuantity = 35, Category = Categories[3], Description = "Умный браслет с отслеживанием активности" },
                new Product { Id = 9, Name = "Крем для лица", Price = 1499, StockQuantity = 60, Category = Categories[4], Description = "Увлажняющий крем для ежедневного ухода" },
                new Product { Id = 10, Name = "Планшет iPad", Price = 39999, StockQuantity = 12, Category = Categories[0], Description = "Планшет для работы и развлечений" }
            };

            foreach (var product in products)
            {
                Products.Add(product);
            }

            // Обновляем количество товаров в категориях
            UpdateCategoryProductCounts();
        }

        if (Customers.Count == 0)
        {
            var customers = new[]
            {
                new Customer { Id = 1, Name = "Иван Иванов", Email = "ivan@mail.ru", Phone = "+7-999-123-45-67", Address = "г. Москва, ул. Примерная, д. 1" },
                new Customer { Id = 2, Name = "Мария Петрова", Email = "maria@mail.ru", Phone = "+7-999-234-56-78", Address = "г. Санкт-Петербург, ул. Тестовая, д. 25" },
                new Customer { Id = 3, Name = "Алексей Сидоров", Email = "alex@mail.ru", Phone = "+7-999-345-67-89", Address = "г. Екатеринбург, ул. Образцовая, д. 10" }
            };

            foreach (var customer in customers)
            {
                Customers.Add(customer);
            }
        }
    }

    /// <summary>
    /// Обновляет количество товаров в категориях
    /// </summary>
    private void UpdateCategoryProductCounts()
    {
        foreach (var category in Categories)
        {
            category.ProductCount = Products.Count(p => p.Category?.Id == category.Id);
        }
    }
    /// <summary>
    /// Очищает все данные (для тестирования)
    /// </summary>
    public void ClearAllData()
    {
        Products.Clear();
        Customers.Clear();
        Orders.Clear();
        Categories.Clear();

        // Удаляем файлы данных
        if (File.Exists(_productsFile)) File.Delete(_productsFile);
        if (File.Exists(_customersFile)) File.Delete(_customersFile);
        if (File.Exists(_ordersFile)) File.Delete(_ordersFile);
        if (File.Exists(_categoriesFile)) File.Delete(_categoriesFile);
    }
}