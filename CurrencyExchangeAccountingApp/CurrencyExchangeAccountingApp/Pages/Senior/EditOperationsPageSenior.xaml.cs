using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;
using static CurrencyExchangeAccountingApp.OperationsPageSenior;

namespace CurrencyExchangeAccountingApp
{
    public partial class EditOperationsPageSenior : Page
    {
        private Users _currentUser; // Текущий авторизованный пользователь
        private Operations _operationToEdit; // Операция для редактирования
        private decimal _originalAmountFrom; // Исходная сумма продажи
        private decimal _originalAmountTo; // Исходная сумма покупки
        private Clients _originalClient; // Исходный клиент операции
        private Clients _currentClient; // Поле для хранения клиента

        public EditOperationsPageSenior(Users user, OperationData operationData)
        {
            InitializeComponent(); // Инициализация компонентов XAML
            _currentUser = user; // Сохранение текущего пользователя
            LoadOperationFromDatabase(operationData); // Загрузка данных операции
            this.Loaded += OnPageLoaded; // Подписка на событие загрузки страницы
        }

        private void LoadOperationFromDatabase(OperationData operationData)
        {
            using (var context = new CurrencyExchangeAccountingEntities()) // Создание контекста БД
            {
                // Загрузка операции с включением связанных данных
                _operationToEdit = context.Operations
                    .Include(o => o.Clients) // Включение данных клиента
                    .Include(o => o.Currencies) // Включение данных валюты продажи
                    .Include(o => o.Currencies1) // Включение данных валюты покупки
                    .FirstOrDefault(o => o.Operation_ID == operationData.Operation_ID); // Поиск по ID

                if (_operationToEdit == null) // Проверка нахождения операции
                {
                    MessageBox.Show("Операция не найдена в базе данных", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error); // Сообщение об ошибке
                    this.Loaded += (s, e) => NavigationService?.GoBack(); // Возврат назад
                    return;
                }

                // Сохранение исходного клиента операции
                _originalClient = context.Clients.Find(_operationToEdit.Client_ID);

                // Сохранение исходных сумм
                _originalAmountFrom = _operationToEdit.Amount_From;
                _originalAmountTo = _operationToEdit.Amount_To;


                // Загрузка клиента
                _originalClient = context.Clients.Find(_operationToEdit.Client_ID);
                _currentClient = _originalClient;
            }
        }

        private void LoadClientData()
        {
            if (_currentClient == null) return;

            EditClientNameTextBox.Text = _currentClient.Client_Full_Name;
            EditPassportSeriesTextBox.Text = _currentClient.Passport_Series;
            EditPassportNumberTextBox.Text = _currentClient.Passport_Number;
            EditDepartmentCodeTextBox.Text = _currentClient.Department_Code;
            EditIssuedByTextBox.Text = _currentClient.Issued_By;
            EditIssuedDatePicker.SelectedDate = _currentClient.Issued_Date;
        }

        // Метод для сохранения изменений клиента
        private void SaveClientChanges()
        {
            if (string.IsNullOrWhiteSpace(EditClientNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EditPassportSeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(EditPassportNumberTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля (ФИО, серия и номер паспорта)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    var clientToUpdate = context.Clients.Find(_currentClient.Client_ID);
                    if (clientToUpdate == null) return;

                    clientToUpdate.Client_Full_Name = EditClientNameTextBox.Text.Trim();
                    clientToUpdate.Passport_Series = EditPassportSeriesTextBox.Text.Trim();
                    clientToUpdate.Passport_Number = EditPassportNumberTextBox.Text.Trim();
                    clientToUpdate.Department_Code = string.IsNullOrWhiteSpace(EditDepartmentCodeTextBox.Text) ?
                        null : EditDepartmentCodeTextBox.Text.Trim();
                    clientToUpdate.Issued_By = string.IsNullOrWhiteSpace(EditIssuedByTextBox.Text) ?
                        null : EditIssuedByTextBox.Text.Trim();
                    clientToUpdate.Issued_Date = EditIssuedDatePicker.SelectedDate ?? DateTime.Now;

                    context.SaveChanges();
                    _currentClient = clientToUpdate;
                    ClientNameTextBlock.Text = clientToUpdate.Client_Full_Name;
                    EditClientPanel.Visibility = Visibility.Collapsed;

                    MessageBox.Show("Данные клиента успешно обновлены", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении клиента: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnPageLoaded(object sender, RoutedEventArgs e)
        {
            if (_operationToEdit != null) // Если операция загружена
            {
                LoadData(); // Загрузка данных на форму
            }
        }

        private void LoadData()
        {
            if (_operationToEdit == null) // Проверка наличия операции
            {
                MessageBox.Show("Не удалось загрузить данные операции", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new CurrencyExchangeAccountingEntities()) // Новый контекст БД
                {
                    // Установка имени клиента
                    ClientNameTextBlock.Text = _operationToEdit.Clients.Client_Full_Name;

                    // Загрузка валют с курсами
                    var currencies = context.Currencies
                        .Include("ExchangeRates") // Включение курсов валют
                        .ToList();

                    // Настройка ComboBox для валют
                    CurrencyFromComboBox.ItemsSource = currencies;
                    CurrencyToComboBox.ItemsSource = currencies;

                    // Установка выбранных валют
                    CurrencyFromComboBox.SelectedValue = _operationToEdit.Currency_ID_From;
                    CurrencyToComboBox.SelectedValue = _operationToEdit.Currency_ID_To;

                    // Установка суммы продажи
                    AmountFromTextBox.Text = _operationToEdit.Amount_From.ToString("N2");

                    // Получение выбранных валют
                    var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
                    var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

                    // Установка курсов валют
                    BuyRateTextBlock.Text = currencyFrom?.ExchangeRates?.Buy_Rate.ToString("N4") ?? "0";
                    SellRateTextBlock.Text = currencyTo?.ExchangeRates?.Sell_Rate.ToString("N4") ?? "0";

                    // Установка суммы покупки
                    AmountToTextBox.Text = _operationToEdit.Amount_To.ToString("N2");

                    // Установка даты операции
                    OperationDatePicker.SelectedDate = _operationToEdit.Operation_Date;
                }
            }
            catch (Exception ex) // Обработка ошибок
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Обработчик кнопки редактирования клиента
        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            EditClientPanel.Visibility = Visibility.Visible;
            LoadClientData();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (CurrencyFromComboBox.SelectedItem == null ||
                CurrencyToComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(AmountFromTextBox.Text))
            {
                MessageBox.Show("Заполните все поля для расчета", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Проверка корректности суммы
            if (!decimal.TryParse(AmountFromTextBox.Text, out decimal amountFrom) || amountFrom <= 0)
            {
                MessageBox.Show("Введите корректную сумму для продажи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получение выбранных валют
            var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
            var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

            // Отображение курсов
            BuyRateTextBlock.Text = currencyFrom.ExchangeRates.Buy_Rate.ToString("N4");
            SellRateTextBlock.Text = currencyTo.ExchangeRates.Sell_Rate.ToString("N4");

            // Расчет суммы покупки
            decimal amountTo = amountFrom * currencyFrom.ExchangeRates.Buy_Rate / currencyTo.ExchangeRates.Sell_Rate;
            AmountToTextBox.Text = amountTo.ToString("N2"); // Отображение результата
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка заполнения полей
            if (CurrencyFromComboBox.SelectedItem == null ||
                CurrencyToComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(AmountFromTextBox.Text) ||
                string.IsNullOrWhiteSpace(AmountToTextBox.Text))
            {
                MessageBox.Show("Заполните все поля и выполните расчет", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (EditClientPanel.Visibility == Visibility.Visible)
            {
                SaveClientChanges();
            }

            try
            {
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    // Поиск операции в БД
                    var operation = context.Operations.Find(_operationToEdit.Operation_ID);
                    if (operation == null)
                    {
                        MessageBox.Show("Операция не найдена", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получение клиента из БД
                    var client = context.Clients.Find(_operationToEdit.Client_ID);
                    if (client == null)
                    {
                        MessageBox.Show("Клиент не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    // Получение выбранных валют
                    var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
                    var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

                    // Парсинг сумм
                    decimal newAmountFrom = decimal.Parse(AmountFromTextBox.Text);
                    decimal newAmountTo = decimal.Parse(AmountToTextBox.Text);

                    // Получение валют из БД
                    var dbCurrencyFrom = context.Currencies.Find(currencyFrom.Currency_ID);
                    var dbCurrencyTo = context.Currencies.Find(currencyTo.Currency_ID);
                    var oldDbCurrencyFrom = context.Currencies.Find(_operationToEdit.Currency_ID_From);
                    var oldDbCurrencyTo = context.Currencies.Find(_operationToEdit.Currency_ID_To);

                    // Возврат старых сумм
                    oldDbCurrencyFrom.Remaining_Amount += _originalAmountFrom;
                    oldDbCurrencyTo.Remaining_Amount -= _originalAmountTo;

                    // Проверка достаточности средств
                    if (dbCurrencyFrom.Remaining_Amount < newAmountFrom)
                    {
                        MessageBox.Show($"Недостаточно валюты для продажи. Доступно: {dbCurrencyFrom.Remaining_Amount:N2}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    // Списание и зачисление новых сумм
                    dbCurrencyFrom.Remaining_Amount -= newAmountFrom;
                    dbCurrencyTo.Remaining_Amount += newAmountTo;

                    // Обновление данных операции
                    operation.Currency_ID_From = dbCurrencyFrom.Currency_ID;
                    operation.Currency_ID_To = dbCurrencyTo.Currency_ID;
                    operation.Currency_Name_From = dbCurrencyFrom.Currency_Name;
                    operation.Currency_Name_To = dbCurrencyTo.Currency_Name;
                    operation.Amount_From = newAmountFrom;
                    operation.Amount_To = newAmountTo;
                    operation.User_ID_Correct = _currentUser.User_ID; // Установка пользователя-редактора
                    operation.Last_Update_Date = DateTime.Now; // Дата изменения

                    context.SaveChanges(); // Сохранение изменений
                    App.RaiseCurrenciesUpdated(); // Обновление данных о валютах

                    MessageBox.Show("Операция успешно обновлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Возврат на страницу операций
                    NavigationService.Navigate(new OperationsPageSenior(_currentUser));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Возврат на страницу операций без сохранения
            NavigationService.Navigate(new OperationsPageSenior(_currentUser));
        }
    }
}