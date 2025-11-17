using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProjectStore.Models;

public class Category : Entity, INotifyPropertyChanged
{
    private string _description;
    private int _productCount;

    public string Description
    {
        get => _description;
        set
        {
            _description = value;
            OnPropertyChanged();
        }
    }

    public int ProductCount
    {
        get => _productCount;
        set
        {
            _productCount = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsPopular));
        }
    }

    public bool IsPopular => ProductCount > 10;

    public override string ToString()
    {
        return $"{Name} ({ProductCount} товаров)";
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}