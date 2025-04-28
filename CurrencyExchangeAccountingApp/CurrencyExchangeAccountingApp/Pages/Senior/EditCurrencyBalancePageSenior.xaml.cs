using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class EditCurrencyBalancePageSenior : Page
    {
        // Текущий пользователь, который выполняет операцию
        private readonly Users _currentUser;

        // Идентификатор валюты, которую нужно отредактировать
        private readonly int _currencyId;

        // Контекст базы данных для работы с данными
        private CurrencyExchangeAccountingEntities _context;

        // Конструктор класса
        public EditCurrencyBalancePageSenior(Users user, Currencies currency)
        {
            InitializeComponent(); // Инициализация интерфейса

            // Проверка, что пользователь не равен null
            _currentUser = user ?? throw new ArgumentNullException(nameof(user));

            // Проверка, что валюта не равна null
            _currencyId = currency?.Currency_ID ?? throw new ArgumentNullException(nameof(currency));

            // Создание нового контекста базы данных
            _context = new CurrencyExchangeAccountingEntities();

            // Подписка на события загрузки и выгрузки страницы
            Loaded += Page_Loaded;
            Unloaded += Page_Unloaded;
        }

        // Обработчик события загрузки страницы
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Поиск валюты в базе данных по её идентификатору
                var currency = _context.Currencies.Find(_currencyId);

                // Если валюта не найдена, показываем ошибку и возвращаемся назад
                if (currency == null)
                {
                    ShowErrorAndNavigateBack("Валюта не найдена");
                    return;
                }

                // Отображение информации о валюте на странице
                CurrencyNameTextBlock.Text = currency.Currency_Name; // Название валюты
                CurrentBalanceTextBlock.Text = currency.Remaining_Amount.ToString("N2"); // Текущий остаток
                NewBalanceTextBox.Text = currency.Remaining_Amount.ToString("N2"); // Предварительно заполняем новое значение
            }
            catch (Exception ex)
            {
                // Обработка исключений при загрузке данных
                ShowErrorAndNavigateBack($"Ошибка загрузки: {ex.Message}");
            }
        }

        // Обработчик события выгрузки страницы
        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            // Освобождение ресурсов контекста базы данных
            _context?.Dispose();

            // Отписка от событий для предотвращения утечек памяти
            Loaded -= Page_Loaded;
            Unloaded -= Page_Unloaded;
        }

        // Метод для показа ошибки и безопасного возврата назад
        private void ShowErrorAndNavigateBack(string message)
        {
            // Отображение сообщения об ошибке
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            // Вызов метода для безопасного возврата назад
            SafeNavigateBack();
        }

        // Метод для безопасного возврата назад
        private void SafeNavigateBack()
        {
            // Проверка, можно ли вернуться назад в историю навигации
            if (NavigationService?.CanGoBack ?? false)
            {
                NavigationService.GoBack(); // Если можно, то возвращаемся назад
            }
            else
            {
                // Если нельзя вернуться назад, переходим на страницу валют
                NavigationService?.Navigate(new CurrenciesPageSenior(_currentUser));
            }
        }

        // Обработчик события нажатия кнопки "Сохранить"
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка корректности ввода нового значения остатка
            if (!decimal.TryParse(NewBalanceTextBox.Text, out decimal newBalance) || newBalance < 0)
            {
                MessageBox.Show("Введите корректное положительное число", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Поиск валюты в базе данных по её идентификатору
                var currency = _context.Currencies.Find(_currencyId);

                // Если валюта найдена, обновляем её остаток
                if (currency != null)
                {
                    currency.Remaining_Amount = newBalance;

                    // Сохраняем изменения в базе данных
                    _context.SaveChanges();

                    // Уведомление приложения о том, что данные валют были обновлены
                    App.RaiseCurrenciesUpdated();

                    // Отображение сообщения об успешном сохранении
                    MessageBox.Show("Остаток валюты успешно обновлен", "Успех",
                                  MessageBoxButton.OK, MessageBoxImage.Information);

                    // Безопасный возврат назад
                    SafeNavigateBack();
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений при сохранении данных
                MessageBox.Show($"Ошибка сохранения: {ex.Message}", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик события нажатия кнопки "Отмена"
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Возвращение на страницу управления валютами
            NavigationService.Navigate(new CurrenciesPageSenior(_currentUser));
        }
    }
}