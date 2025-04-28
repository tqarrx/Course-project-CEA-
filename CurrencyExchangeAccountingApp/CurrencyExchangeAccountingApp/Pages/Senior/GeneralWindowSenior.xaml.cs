using System.Windows;
using System.Windows.Controls;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp.Pages.Senior
{
    public partial class GeneralWindowSenior : Window
    {
        // Объявляем приватное поле для хранения текущего пользователя
        private Users _currentUser;
        public GeneralWindowSenior(Users user)
        {
            InitializeComponent(); // Инициализируем интерфейс
            _currentUser = user; // Присваиваем текущего пользователя
            this.Title = $"Учет обмена валют - {user.User_Full_Name} ({user.Roles.Role_Name})"; // Устанавливаем заголовок окна

            // Навигация на начальную страницу приложения (курсы валют)
            NavigateToExchangeRatesPage();

            // Выделяем кнопку "Курс валют" как выбранную
            ExchangeRatesBtn.FontWeight = FontWeights.Bold;
        }

        // Обработчик события загрузки окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Устанавливаем максимальные размеры окна с учетом панели задач
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;

            // Размещаем окно в верхнем левом углу экрана
            this.Left = 0;
            this.Top = 0;

            // Устанавливаем размеры окна равными максимальным размерам экрана
            this.Width = SystemParameters.MaximizedPrimaryScreenWidth;
            this.Height = SystemParameters.MaximizedPrimaryScreenHeight;
        }

        // Метод для навигации на страницу курсов валют
        private void NavigateToExchangeRatesPage()
        {
            // Навигируем во фрейм главного окна на страницу курсов валют
            MainFrame.Navigate(new ExchangeRatesPageSenior(_currentUser));

            // Выделяем кнопку "Курс валют" как выбранную
            UpdateButtonSelection(ExchangeRatesBtn);
        }

        // Метод для навигации на страницу валют
        private void NavigateToCurrenciesPage()
        {
            // Навигируем во фрейм главного окна на страницу валют
            MainFrame.Navigate(new CurrenciesPageSenior(_currentUser));

            // Выделяем кнопку "Валюты" как выбранную
            UpdateButtonSelection(CurrenciesBtn);
        }

        // Метод для навигации на страницу операций
        private void NavigateToOperationsPage()
        {
            // Навигируем во фрейм главного окна на страницу операций
            MainFrame.Navigate(new OperationsPageSenior(_currentUser));

            // Выделяем кнопку "Операции" как выбранную
            UpdateButtonSelection(OperationsBtn);
        }

        // Метод для навигации на страницу отчетов
        private void NavigateToReportsPage()
        {
            // Навигируем во фрейм главного окна на страницу отчетов
            MainFrame.Navigate(new ReportsPageSenior(_currentUser));

            // Выделяем кнопку "Отчеты" как выбранную
            UpdateButtonSelection(ReportsBtn);
        }

        // Метод для обновления состояния кнопок (выделение выбранной кнопки)
        private void UpdateButtonSelection(Button selectedButton)
        {
            // Сбрасываем выделение со всех кнопок
            ExchangeRatesBtn.FontWeight = FontWeights.Normal;
            CurrenciesBtn.FontWeight = FontWeights.Normal;
            OperationsBtn.FontWeight = FontWeights.Normal;
            ReportsBtn.FontWeight = FontWeights.Normal;

            // Выделяем выбранную кнопку
            selectedButton.FontWeight = FontWeights.Bold;
        }

        // Обработчик события нажатия на кнопку "Курс валют"
        private void ExchangeRatesBtn_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу курсов валют
            NavigateToExchangeRatesPage();
        }

        // Обработчик события нажатия на кнопку "Валюты"
        private void CurrenciesBtn_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу валют
            NavigateToCurrenciesPage();
        }

        // Обработчик события нажатия на кнопку "Операции"
        private void OperationsBtn_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу операций
            NavigateToOperationsPage();
        }

        // Обработчик события нажатия на кнопку "Отчеты"
        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу отчетов
            NavigateToReportsPage();
        }

        // Обработчик события нажатия на кнопку "Выход"
        private void LogoutBtn_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно входа
            var loginWindow = new MainWindow();
            loginWindow.Show();

            // Закрываем текущее окно
            this.Close();
        }
    }
}