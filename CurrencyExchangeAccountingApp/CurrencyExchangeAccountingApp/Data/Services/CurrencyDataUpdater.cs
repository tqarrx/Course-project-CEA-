using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CurrencyExchangeAccountingApp.Utilities;
using CurrencyExchangeAccountingApp;
using CurrencyExchangeAccountingApp.Data.Models;

public static class CurrencyDataUpdater
{
    private static int _cbrApiCallCount = 0; // Счетчик вызовов API ЦБ РФ
    private static DateTime _lastUpdateTime = DateTime.MinValue; // Время последнего обновления
    private static DateTime _lastLoginTime = DateTime.MinValue; // Время последнего входа пользователя

    public static void UpdateExchangeRates() // Метод обновления курсов валют
    {
        _cbrApiCallCount++; // Увеличиваем счетчик вызовов
        Logger.Log($"Запрос к ЦБ РФ #{_cbrApiCallCount}"); // Логируем факт вызова

        try // Обработка возможных ошибок
        {
            string url = "https://www.cbr.ru/scripts/XML_daily.asp"; // URL API ЦБ РФ
            XDocument xml = XDocument.Load(url); // Загружаем XML с курсами валют

            using (var context = new CurrencyExchangeAccountingEntities()) // Создаем контекст БД
            {
                var currencies = context.Currencies.ToList(); // Получаем список валют из БД

                foreach (var currency in currencies) // Перебираем все валюты
                {
                    // Ищем валюту в XML по коду
                    var valute = xml.Descendants("Valute")
                        .FirstOrDefault(v => v.Element("CharCode")?.Value == currency.Currency_Code);

                    if (valute != null) // Если валюта найдена в ответе ЦБ
                    {
                        // Парсим курс покупки с учетом культуры (разделитель запятая)
                        decimal buyRate = decimal.Parse(valute.Element("Value")?.Value ?? "0",
                            CultureInfo.GetCultureInfo("ru-RU"));
                        // Парсим номинал (например, для японской йены - 100)
                        decimal nominal = decimal.Parse(valute.Element("Nominal")?.Value ?? "1");
                        buyRate /= nominal; // Приводим курс к единичному номиналу

                        decimal sellRate = buyRate * 1.02m; // Курс продажи = курс покупки + 2%

                        // Находим соответствующий курс в БД
                        var rate = context.ExchangeRates.FirstOrDefault(r => r.Rate_ID == currency.Rate_ID);
                        if (rate != null) // Если курс найден
                        {
                            rate.Buy_Rate = buyRate; // Обновляем курс покупки
                            rate.Sell_Rate = sellRate; // Обновляем курс продажи
                        }
                    }
                }
                context.SaveChanges(); // Сохраняем изменения в БД
                _lastUpdateTime = DateTime.Now; // Фиксируем время обновления
            }
        }
        catch (Exception ex) // Обработка ошибок
        {
            Logger.Log($"Ошибка при обновлении курсов: {ex.Message}"); // Логируем ошибку
            throw; // Пробрасываем исключение дальше
        }
    }

    public static bool NeedsUpdate() // Нужно ли обновление?
    {
        // Проверяем, прошло ли больше 24 часов с последнего обновления
        return (DateTime.Now - _lastUpdateTime).TotalHours > 24;
    }

    public static void SetLastLoginTime(DateTime loginTime) // Установка времени последнего входа
    {
        _lastLoginTime = loginTime; // Сохраняем время входа
    }

    public static TimeSpan GetTimeSinceLastUpdate() // Время с последнего обновления
    {
        return DateTime.Now - _lastUpdateTime; // Возвращаем разницу во времени
    }

    public static TimeSpan GetTimeSinceLastLogin() // Время с последнего входа
    {
        return DateTime.Now - _lastLoginTime; // Возвращаем разницу во времени
    }

    public static int GetCbrApiCallCount() => _cbrApiCallCount; // Получение счетчика вызовов
}