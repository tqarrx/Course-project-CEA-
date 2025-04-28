using System.Windows;
using System.Windows.Controls;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp.Pages.Operator
{
    public partial class GeneralWindowOperator : Window
    {
        private Users _currentUser;

        public GeneralWindowOperator(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            this.Title = $"Учет обмена валют - {user.User_Full_Name} (Оператор)";
            NavigateToExchangeRatesPage();
            ExchangeRatesBtn.FontWeight = FontWeights.Bold;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.Left = 0;
            this.Top = 0;
            this.Width = SystemParameters.MaximizedPrimaryScreenWidth;
            this.Height = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        private void NavigateToExchangeRatesPage()
        {
            MainFrame.Navigate(new ExchangeRatesPageOperator(_currentUser));
            UpdateButtonSelection(ExchangeRatesBtn);
        }

        private void NavigateToCurrenciesPage()
        {
            MainFrame.Navigate(new CurrenciesPageOperator(_currentUser));
            UpdateButtonSelection(CurrenciesBtn);
        }

        private void NavigateToOperationsPage()
        {
            MainFrame.Navigate(new OperationsPageOperator(_currentUser));
            UpdateButtonSelection(OperationsBtn);
        }

        private void UpdateButtonSelection(Button selectedButton)
        {
            ExchangeRatesBtn.FontWeight = FontWeights.Normal;
            CurrenciesBtn.FontWeight = FontWeights.Normal;
            OperationsBtn.FontWeight = FontWeights.Normal;
            selectedButton.FontWeight = FontWeights.Bold;
        }

        private void ExchangeRatesBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToExchangeRatesPage();
        }

        private void CurrenciesBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToCurrenciesPage();
        }

        private void OperationsBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigateToOperationsPage();
        }

        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new MainWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}