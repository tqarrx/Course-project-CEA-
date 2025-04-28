using System;
using System.IO;

namespace CurrencyExchangeAccountingApp.Utilities
{
    public static class Logger
    {
        private static readonly string LogPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "logs",
            "currency_updater.log");

        static Logger()
        {
            // Создаем папку для логов, если ее нет
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
        }

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n");
            }
            catch (Exception ex)
            {
                // Если не удалось записать лог, выводим в консоль
                Console.WriteLine($"Ошибка записи лога: {ex.Message}");
            }
        }
    }
}