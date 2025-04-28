using System;
using System.Threading;
using System.Windows;
using CurrencyExchangeAccountingApp.Utilities;

namespace CurrencyExchangeAccountingApp
{
    public static class CurrencyScheduler
    {
        private static Timer _dailyTimer; // Таймер для ежедневных обновлений

        public static void Start() // Запуск планировщика
        {
            try // Обработка ошибок
            {
                Logger.Log("Запуск планировщика задач..."); // Логируем запуск

                // Создаем таймер с ежедневным интервалом
                _dailyTimer = new Timer(UpdateExchangeRatesDaily, null,
                    CalculateInitialDelay(SchedulerConfig.DailyUpdateTime),
                    TimeSpan.FromHours(24));

                Logger.Log("Планировщик успешно запущен."); // Логируем успешный запуск
            }
            catch (Exception ex) // Обработка ошибок
            {
                Logger.Log($"Ошибка при запуске планировщика: {ex.Message}"); // Логируем ошибку
            }
        }

        public static void Stop() // Остановка планировщика
        {
            try // Обработка ошибок
            {
                Logger.Log("Остановка планировщика задач..."); // Логируем остановку
                _dailyTimer?.Dispose(); // Освобождаем ресурсы таймера
                _dailyTimer = null; // Обнуляем ссылку
                Logger.Log("Планировщик успешно остановлен."); // Логируем успешную остановку
            }
            catch (Exception ex) // Обработка ошибок
            {
                Logger.Log($"Ошибка при остановке планировщика: {ex.Message}"); // Логируем ошибку
            }
        }

        private static void UpdateExchangeRatesDaily(object state) // Ежедневное обновление
        {
            try // Обработка ошибок
            {
                Logger.Log("Начало ежедневного обновления курсов..."); // Логируем начало
                CurrencyDataUpdater.UpdateExchangeRates(); // Вызываем обновление курсов
                Logger.Log("Курсы валют успешно обновлены."); // Логируем успех
                Application.Current.Dispatcher.Invoke(() => // Вызываем в UI-потоке
                    App.ShowInfo("Курсы валют успешно обновлены")); // Показываем сообщение
            }
            catch (Exception ex) // Обработка ошибок
            {
                Logger.Log($"Ошибка при обновлении курсов: {ex.Message}"); // Логируем ошибку
                Application.Current.Dispatcher.Invoke(() => // Вызываем в UI-потоке
                    App.ShowError($"Ошибка при обновлении курсов: {ex.Message}")); // Показываем ошибку
            }
        }

        private static TimeSpan CalculateInitialDelay(TimeSpan targetTime) // Расчет задержки
        {
            DateTime now = DateTime.Now; // Текущее время
            DateTime targetToday = now.Date.Add(targetTime); // Время сегодняшнего обновления

            // Если сегодняшнее время еще не наступило, возвращаем разницу
            // Иначе возвращаем разницу до завтрашнего обновления
            return now < targetToday
                ? targetToday - now
                : targetToday.AddDays(1) - now;
        }

        public static void PrintApiStatistics() // Вывод статистики
        {
            // Формируем строку статистики
            string stats = $"Статистика API-запросов:\n" +
                           $"ЦБ РФ: {CurrencyDataUpdater.GetCbrApiCallCount()}\n" +
                           $"Последнее обновление: {CurrencyDataUpdater.GetTimeSinceLastUpdate().TotalHours:F1} часов назад\n" +
                           $"Последний вход: {CurrencyDataUpdater.GetTimeSinceLastLogin().TotalHours:F1} часов назад";
            Logger.Log(stats); // Логируем статистику
            MessageBox.Show(stats); // Показываем статистику пользователю
        }
    }
}