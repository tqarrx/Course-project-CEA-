using System;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class AddOperationsPageOperator : Page
    {
        private Users _currentUser;

        public AddOperationsPageOperator(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadData();
            OperationDatePicker.SelectedDate = DateTime.Now;
        }

        private void LoadData()
        {
            try
            {
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    var currencies = context.Currencies
                        .Include("ExchangeRates")
                        .ToList();
                    CurrencyFromComboBox.ItemsSource = currencies;
                    CurrencyToComboBox.ItemsSource = currencies;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            if (string.IsNullOrWhiteSpace(ClientNameTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportSeriesTextBox.Text) ||
                string.IsNullOrWhiteSpace(PassportNumberTextBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля клиента (ФИО, серия и номер паспорта)", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (PassportSeriesTextBox.Text.Length != 4 || PassportNumberTextBox.Text.Length != 6)
            {
                MessageBox.Show("Серия паспорта должна содержать 4 цифры, номер - 6 цифр", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (CurrencyFromComboBox.SelectedItem == null ||
                CurrencyToComboBox.SelectedItem == null ||
                string.IsNullOrWhiteSpace(AmountFromTextBox.Text) ||
                string.IsNullOrWhiteSpace(AmountToTextBox.Text))
            {
                MessageBox.Show("Заполните все поля и выполните расчет", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    var existingClient = context.Clients.FirstOrDefault(c =>
                        c.Passport_Series == PassportSeriesTextBox.Text.Trim() &&
                        c.Passport_Number == PassportNumberTextBox.Text.Trim());

                    if (existingClient != null)
                    {
                        MessageBox.Show("Клиент с такими паспортными данными уже существует", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var client = new Clients
                    {
                        Client_Full_Name = ClientNameTextBox.Text.Trim(),
                        Passport_Series = PassportSeriesTextBox.Text.Trim(),
                        Passport_Number = PassportNumberTextBox.Text.Trim(),
                        Department_Code = string.IsNullOrWhiteSpace(DepartmentCodeTextBox.Text) ?
                            null : DepartmentCodeTextBox.Text.Trim(),
                        Issued_By = string.IsNullOrWhiteSpace(IssuedByTextBox.Text) ?
                            null : IssuedByTextBox.Text.Trim(),
                        Issued_Date = IssuedDatePicker.SelectedDate ?? DateTime.Now
                    };
                    context.Clients.Add(client);
                    context.SaveChanges();

                    var currencyFrom = context.Currencies.Find(((Currencies)CurrencyFromComboBox.SelectedItem).Currency_ID);
                    var currencyTo = context.Currencies.Find(((Currencies)CurrencyToComboBox.SelectedItem).Currency_ID);

                    if (currencyFrom.Remaining_Amount < decimal.Parse(AmountFromTextBox.Text))
                    {
                        MessageBox.Show($"Недостаточно валюты для продажи. Доступно: {currencyFrom.Remaining_Amount:N2}",
                            "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    var newOperation = new Operations
                    {
                        User_ID = _currentUser.User_ID,
                        Client_ID = client.Client_ID,
                        Currency_ID_From = currencyFrom.Currency_ID,
                        Currency_ID_To = currencyTo.Currency_ID,
                        Currency_Name_From = currencyFrom.Currency_Name,
                        Currency_Name_To = currencyTo.Currency_Name,
                        Amount_From = decimal.Parse(AmountFromTextBox.Text),
                        Amount_To = decimal.Parse(AmountToTextBox.Text),
                        Operation_Date = DateTime.Now,
                        Last_Update_Date = null
                    };

                    currencyFrom.Remaining_Amount -= newOperation.Amount_From;
                    currencyTo.Remaining_Amount += newOperation.Amount_To;

                    context.Operations.Add(newOperation);
                    context.SaveChanges();

                    App.RaiseCurrenciesUpdated();

                    MessageBox.Show("Операция успешно сохранена", "Успех",
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