using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class EditOperationsPageOperator : Page
    {
        private Users _currentUser;
        private Operations _operationToEdit;
        private decimal _originalAmountFrom;
        private decimal _originalAmountTo;
        private Clients _originalClient;
        private Clients _currentClient;

        public EditOperationsPageOperator(Users user, OperationsPageOperator.OperationData operationData)
        {
            InitializeComponent();
            _currentUser = user;
            LoadOperationFromDatabase(operationData);
            this.Loaded += OnPageLoaded;
        }

        private void LoadOperationFromDatabase(OperationsPageOperator.OperationData operationData)
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                _operationToEdit = context.Operations
                    .Include(o => o.Clients)
                    .Include(o => o.Currencies)
                    .Include(o => o.Currencies1)
                    .FirstOrDefault(o => o.Operation_ID == operationData.Operation_ID);

                if (_operationToEdit == null)
                {
                    MessageBox.Show("Операция не найдена в базе данных", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    this.Loaded += (s, e) => NavigationService?.GoBack();
                    return;
                }

                _originalClient = context.Clients.Find(_operationToEdit.Client_ID);
                _originalAmountFrom = _operationToEdit.Amount_From;
                _originalAmountTo = _operationToEdit.Amount_To;
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
            if (_operationToEdit != null)
            {
                LoadData();
            }
        }

        private void LoadData()
        {
            if (_operationToEdit == null)
            {
                MessageBox.Show("Не удалось загрузить данные операции", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    ClientNameTextBlock.Text = _operationToEdit.Clients.Client_Full_Name;

                    var currencies = context.Currencies
                        .Include("ExchangeRates")
                        .ToList();

                    CurrencyFromComboBox.ItemsSource = currencies;
                    CurrencyToComboBox.ItemsSource = currencies;

                    CurrencyFromComboBox.SelectedValue = _operationToEdit.Currency_ID_From;
                    CurrencyToComboBox.SelectedValue = _operationToEdit.Currency_ID_To;

                    AmountFromTextBox.Text = _operationToEdit.Amount_From.ToString("N2");

                    var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
                    var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

                    BuyRateTextBlock.Text = currencyFrom?.ExchangeRates?.Buy_Rate.ToString("N4") ?? "0";
                    SellRateTextBlock.Text = currencyTo?.ExchangeRates?.Sell_Rate.ToString("N4") ?? "0";

                    AmountToTextBox.Text = _operationToEdit.Amount_To.ToString("N2");

                    OperationDatePicker.SelectedDate = _operationToEdit.Operation_Date;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditClientButton_Click(object sender, RoutedEventArgs e)
        {
            EditClientPanel.Visibility = Visibility.Visible;
            LoadClientData();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrencyFromComboBox.SelectedItem == null ||
                CurrencyToComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(AmountFromTextBox.Text))
            {
                MessageBox.Show("Заполните все поля для расчета", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(AmountFromTextBox.Text, out decimal amountFrom) || amountFrom <= 0)
            {
                MessageBox.Show("Введите корректную сумму для продажи", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
            var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

            BuyRateTextBlock.Text = currencyFrom.ExchangeRates.Buy_Rate.ToString("N4");
            SellRateTextBlock.Text = currencyTo.ExchangeRates.Sell_Rate.ToString("N4");

            decimal amountTo = amountFrom * currencyFrom.ExchangeRates.Buy_Rate / currencyTo.ExchangeRates.Sell_Rate;
            AmountToTextBox.Text = amountTo.ToString("N2");
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
                    var operation = context.Operations.Find(_operationToEdit.Operation_ID);
                    if (operation == null)
                    {
                        MessageBox.Show("Операция не найдена", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var client = context.Clients.Find(_operationToEdit.Client_ID);
                    if (client == null)
                    {
                        MessageBox.Show("Клиент не найден", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    var currencyFrom = (Currencies)CurrencyFromComboBox.SelectedItem;
                    var currencyTo = (Currencies)CurrencyToComboBox.SelectedItem;

                    decimal newAmountFrom = decimal.Parse(AmountFromTextBox.Text);
                    decimal newAmountTo = decimal.Parse(AmountToTextBox.Text);

                    var dbCurrencyFrom = context.Currencies.Find(currencyFrom.Currency_ID);
                    var dbCurrencyTo = context.Currencies.Find(currencyTo.Currency_ID);
                    var oldDbCurrencyFrom = context.Currencies.Find(_operationToEdit.Currency_ID_From);
                    var oldDbCurrencyTo = context.Currencies.Find(_operationToEdit.Currency_ID_To);

                    oldDbCurrencyFrom.Remaining_Amount += _originalAmountFrom;
                    oldDbCurrencyTo.Remaining_Amount -= _originalAmountTo;

                    if (dbCurrencyFrom.Remaining_Amount < newAmountFrom)
                    {
                        MessageBox.Show($"Недостаточно валюты для продажи. Доступно: {dbCurrencyFrom.Remaining_Amount:N2}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    dbCurrencyFrom.Remaining_Amount -= newAmountFrom;
                    dbCurrencyTo.Remaining_Amount += newAmountTo;

                    operation.Currency_ID_From = dbCurrencyFrom.Currency_ID;
                    operation.Currency_ID_To = dbCurrencyTo.Currency_ID;
                    operation.Currency_Name_From = dbCurrencyFrom.Currency_Name;
                    operation.Currency_Name_To = dbCurrencyTo.Currency_Name;
                    operation.Amount_From = newAmountFrom;
                    operation.Amount_To = newAmountTo;
                    operation.User_ID_Correct = _currentUser.User_ID;
                    operation.Last_Update_Date = DateTime.Now;

                    context.SaveChanges();
                    App.RaiseCurrenciesUpdated();

                    MessageBox.Show("Операция успешно обновлена", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.Navigate(new OperationsPageOperator(_currentUser));
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
            NavigationService.Navigate(new OperationsPageOperator(_currentUser));
        }
    }
}