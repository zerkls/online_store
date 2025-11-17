using System.Windows;
using ProjectStore.ViewModels;

namespace ProjectStore.Views;

/// <summary>
/// Окно оформления заказа
/// Демонстрирует принципы MVVM - код-behind содержит минимальную логику
/// </summary>
public partial class CheckoutWindow : Window
{
    public CheckoutWindow()
    {
        InitializeComponent();
    }

    public CheckoutWindow(CheckoutViewModel viewModel) : this()
    {
        DataContext = viewModel;
    }

    /// <summary>
    /// Обработчик загрузки окна - устанавливаем фокус на первом поле
    /// </summary>
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        // Устанавливаем фокус на поле имени, если оно пустое
        if (DataContext is CheckoutViewModel vm &&
            string.IsNullOrEmpty(vm.CurrentCustomer.Name))
        {
            // Можно установить фокус на первое поле ввода
            // NameTextBox.Focus(); - если бы у нас был x:Name для TextBox
        }
    }

    /// <summary>
    /// Обработчик закрытия окна через крестик
    /// </summary>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        // Если окно закрывается через крестик, устанавливаем DialogResult в false
        if (DialogResult == null)
        {
            DialogResult = false;
        }
    }

    /// <summary>
    /// Обработчик нажатия клавиш - ESC для отмены
    /// </summary>
    private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == System.Windows.Input.Key.Escape)
        {
            if (DataContext is CheckoutViewModel vm)
            {
                vm.CancelCommand.Execute(null);
            }
        }
    }

    /// <summary>
    /// Автоматическое форматирование номера карты
    /// </summary>
    private void CardNumberTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        // В реальном приложении здесь можно добавить форматирование номера карты
        // Например: 1234 5678 9012 3456
    }

    /// <summary>
    /// Валидация даты истечения срока карты
    /// </summary>
    private void ExpiryDateTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        // В реальном приложении можно добавить автоматическое добавление "/"
        // и валидацию формата ММ/ГГ
    }

    /// <summary>
    /// Ограничение ввода для CVV (только цифры)
    /// </summary>
    private void CVVTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        if (!char.IsDigit(e.Text, 0))
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Ограничение длины CVV (3-4 цифры)
    /// </summary>
    private void CVVTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {
        var textBox = sender as System.Windows.Controls.TextBox;
        if (textBox != null && textBox.Text.Length > 4)
        {
            textBox.Text = textBox.Text.Substring(0, 4);
            textBox.CaretIndex = textBox.Text.Length;
        }
    }

    /// <summary>
    /// Ограничение ввода для номера телефона
    /// </summary>
    private void PhoneNumberTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
    {
        // Разрешаем только цифры, +, -, (, )
        if (!char.IsDigit(e.Text, 0) && e.Text != "+" && e.Text != "-" && e.Text != "(" && e.Text != ")")
        {
            e.Handled = true;
        }
    }

    /// <summary>
    /// Подсказка при наведении на способ оплаты
    /// </summary>
    private void PaymentComboBox_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        // Можно показать tooltip с информацией о способах оплаты
    }

    /// <summary>
    /// Анимация при наведении на кнопки
    /// </summary>
    private void Button_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        var button = sender as System.Windows.Controls.Button;
        if (button != null)
        {
            // Легкая анимация может быть добавлена здесь
        }
    }
}