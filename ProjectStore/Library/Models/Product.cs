using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfShopApp.Models
{
    public class Product : Entity, INotifyPropertyChanged
    {
        private decimal _price;
        private int _stockQuantity;

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public int StockQuantity
        {
            get => _stockQuantity;
            set { _stockQuantity = value; OnPropertyChanged(); }
        }

        public Category Category { get; set; }
        public string Description { get; set; }

        public string DisplayInfo => $"{Name} - {Price:C} (В наличии: {StockQuantity})";
        public string Details => $"{Name}\nЦена: {Price:C}\nВ наличии: {StockQuantity}\nКатегория: {Category?.Name}\nОписание: {Description}";

        public event PropertyChangedEventHandler PropertyChanged;

        internal virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}