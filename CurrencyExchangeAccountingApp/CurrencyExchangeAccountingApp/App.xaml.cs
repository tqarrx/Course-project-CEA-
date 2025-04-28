using System;
using System.Threading.Tasks;
using System.Windows;
using CurrencyExchangeAccountingApp.Utilities;

namespace CurrencyExchangeAccountingApp
{
    public partial class App : Application
    {
        // Добавляем событие для обновления валют
        public static event Action CurrenciesUpdated = delegate { };

        // Добавляем метод для вызова события
        public static void RaiseCurrenciesUpdated()
        {
            CurrenciesUpdated?.Invoke();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Logger.Log("Приложение запущено.");

            // Только запуск планировщика
            CurrencyScheduler.Start();
        }

        public static void ShowInfo(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            });
        }

        public static void ShowWarning(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            });
        }

        public static void ShowError(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            });
        }

        protected override void OnExit(ExitEventArgs e)
        {
            CurrencyScheduler.Stop();
            Logger.Log("Приложение завершено");
            base.OnExit(e);
        }
    }
}