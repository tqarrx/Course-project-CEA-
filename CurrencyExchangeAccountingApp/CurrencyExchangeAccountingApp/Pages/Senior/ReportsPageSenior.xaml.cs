using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CurrencyExchangeAccountingApp.Data.Models;
using Microsoft.Office.Interop.Excel;
using Application = Microsoft.Office.Interop.Excel.Application;
using DataTable = System.Data.DataTable;

namespace CurrencyExchangeAccountingApp
{
    // Определение класса ReportsPageSenior, который наследуется от System.Windows.Controls.Page
    public partial class ReportsPageSenior : System.Windows.Controls.Page
    {
        // Поле для хранения текущего пользователя
        private Users _currentUser;

        // Поле для хранения текущего отчета в виде DataTable
        private DataTable _currentReportData;

        // Конструктор класса, принимающий объект Users как параметр
        public ReportsPageSenior(Users user)
        {
            // Инициализация интерфейса
            InitializeComponent();

            // Присвоение текущего пользователя
            _currentUser = user;

            // Установка дат по умолчанию (начало текущего месяца и текущая дата)
            StartDatePicker.SelectedDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDatePicker.SelectedDate = DateTime.Now;

            // Загрузка клиентов
            LoadClients();
        }

        // Метод для загрузки списка клиентов
        private void LoadClients()
        {
            // Тело метода пока пустое
        }

        // Метод для генерации отчета операций
        private void GenerateOperationsReport()
        {
            try
            {
                // Получение даты начала и конца периода
                var startDate = StartDatePicker.SelectedDate ?? DateTime.Now.AddMonths(-1);
                var endDate = EndDatePicker.SelectedDate ?? DateTime.Now;
                endDate = endDate.AddDays(1).AddSeconds(-1);

                // Создание контекста базы данных
                using (var context = new CurrencyExchangeAccountingEntities())
                {
                    // Проверка наличия операций за указанный период
                    var operationsExist = context.Operations
                        .Any(o => o.Operation_Date >= startDate && o.Operation_Date <= endDate);
                    if (!operationsExist)
                    {
                        // Если операций нет, показать сообщение об ошибке
                        MessageBox.Show($"За период с {startDate:d} по {endDate:d} операций не найдено",
                                      "Данные не найдены",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                        return;
                    }

                    // Выполнение запроса к базе данных для получения операций
                    var operations = (from o in context.Operations
                                      where o.Operation_Date >= startDate && o.Operation_Date <= endDate
                                      join c in context.Clients on o.Client_ID equals c.Client_ID
                                      join currFrom in context.Currencies on o.Currency_ID_From equals currFrom.Currency_ID
                                      join rateFrom in context.ExchangeRates on currFrom.Rate_ID equals rateFrom.Rate_ID
                                      join currTo in context.Currencies on o.Currency_ID_To equals currTo.Currency_ID
                                      join rateTo in context.ExchangeRates on currTo.Rate_ID equals rateTo.Rate_ID
                                      join u in context.Users on o.User_ID_Correct equals u.User_ID into userJoin
                                      from user in userJoin.DefaultIfEmpty()
                                      select new
                                      {
                                          o.Operation_ID,
                                          ClientName = c.Client_Full_Name,
                                          o.Operation_Date,
                                          CurrencyFrom = currFrom.Currency_Name,
                                          o.Amount_From,
                                          CurrencyTo = currTo.Currency_Name,
                                          o.Amount_To,
                                          BuyRate = rateFrom.Buy_Rate,
                                          SellRate = rateTo.Sell_Rate,
                                          o.Last_Update_Date,
                                          CorrectedBy = user != null ? user.User_Full_Name : null
                                      }).ToList();

                    // Проверка наличия данных
                    if (!operations.Any())
                    {
                        MessageBox.Show($"Операции существуют, но не удалось загрузить полные данные за период с {startDate:d} по {endDate:d}" +
                                       "Проверьте связи между таблицами в базе данных",
                                      "Ошибка загрузки данных",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Warning);
                        return;
                    }

                    // Создание DataTable для хранения данных отчета
                    _currentReportData = new DataTable();
                    _currentReportData.Columns.Add("ID операции", typeof(int));
                    _currentReportData.Columns.Add("Клиент", typeof(string));
                    _currentReportData.Columns.Add("Дата операции", typeof(DateTime));
                    _currentReportData.Columns.Add("Валюта продажи", typeof(string));
                    _currentReportData.Columns.Add("Сумма продажи", typeof(decimal));
                    _currentReportData.Columns.Add("Валюта покупки", typeof(string));
                    _currentReportData.Columns.Add("Сумма покупки", typeof(decimal));
                    _currentReportData.Columns.Add("Курс покупки", typeof(decimal));
                    _currentReportData.Columns.Add("Курс продажи", typeof(decimal));
                    _currentReportData.Columns.Add("Дата изменения", typeof(DateTime));
                    _currentReportData.Columns.Add("Изменено пользователем", typeof(string));

                    // Заполнение DataTable данными из операций
                    foreach (var op in operations)
                    {
                        _currentReportData.Rows.Add(
                            op.Operation_ID,
                            op.ClientName,
                            op.Operation_Date,
                            op.CurrencyFrom,
                            op.Amount_From,
                            op.CurrencyTo,
                            op.Amount_To,
                            op.BuyRate,
                            op.SellRate,
                            op.Last_Update_Date,
                            op.CorrectedBy
                        );
                    }

                    // Отображение отчета
                    DisplayReport(_currentReportData);
                }
            }
            catch (Exception ex)
            {
                // Обработка исключений
                MessageBox.Show($"Ошибка при формировании отчета: {ex.Message}\n{ex.InnerException?.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
                Debug.WriteLine($"Полная ошибка: {ex}");
            }
        }

        // Метод для генерации отчета курсов обмена валют
        private void GenerateExchangeRatesReport()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                // Выполнение запроса к базе данных для получения курсов обмена
                var rates = context.ExchangeRates
                    .Join(context.Currencies, r => r.Rate_ID, c => c.Rate_ID, (r, c) => new
                    {
                        c.Currency_Name,
                        c.Currency_Code,
                        r.Buy_Rate,
                        r.Sell_Rate
                    })
                    .ToList();

                // Создание DataTable для хранения данных отчета
                _currentReportData = new DataTable();
                _currentReportData.Columns.Add("Валюта", typeof(string));
                _currentReportData.Columns.Add("Код валюты", typeof(string));
                _currentReportData.Columns.Add("Курс покупки", typeof(decimal));
                _currentReportData.Columns.Add("Курс продажи", typeof(decimal));

                // Заполнение DataTable данными из курсов обмена
                foreach (var rate in rates)
                {
                    _currentReportData.Rows.Add(
                        rate.Currency_Name,
                        rate.Currency_Code,
                        rate.Buy_Rate,
                        rate.Sell_Rate
                    );
                }

                // Отображение отчета
                DisplayReport(_currentReportData);
            }
        }

        // Метод для генерации отчета о балансах валют
        private void GenerateCurrencyBalancesReport()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                // Получение списка валют из базы данных
                var currencies = context.Currencies.ToList();

                // Создание DataTable для хранения данных отчета
                _currentReportData = new DataTable();
                _currentReportData.Columns.Add("Валюта", typeof(string));
                _currentReportData.Columns.Add("Код валюты", typeof(string));
                _currentReportData.Columns.Add("Остаток", typeof(decimal));

                // Заполнение DataTable данными из валют
                foreach (var currency in currencies)
                {
                    _currentReportData.Rows.Add(
                        currency.Currency_Name,
                        currency.Currency_Code,
                        currency.Remaining_Amount
                    );
                }

                // Отображение отчета
                DisplayReport(_currentReportData);
            }
        }

        // Метод для отображения отчета в DataGrid
        private void DisplayReport(DataTable data)
        {
            // Привязка данных к DataGrid
            ReportDataGrid.ItemsSource = data.DefaultView;

            // Включение автоматической генерации столбцов
            ReportDataGrid.AutoGenerateColumns = true;
        }

        // Обработчик события AutoGeneratingColumn для форматирования столбцов
        private void ReportDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Форматирование столбцов с типом DateTime
            if (e.PropertyType == typeof(DateTime))
            {
                var column = e.Column as DataGridTextColumn;
                if (column != null)
                {
                    column.Binding = new Binding(e.PropertyName)
                    {
                        StringFormat = "dd.MM.yyyy HH:mm"
                    };
                }
            }

            // Установка стиля для текстовых элементов столбцов
            var style = new System.Windows.Style(typeof(TextBlock));
            style.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));
            style.Setters.Add(new Setter(TextBlock.PaddingProperty, new Thickness(5)));
            style.Setters.Add(new Setter(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center));

            // Применение стиля к столбцам
            if (e.Column is DataGridTextColumn textColumn)
            {
                textColumn.ElementStyle = style;
            }
        }

        // Метод для экспорта отчета в Excel
        private void ExportToExcel()
        {
            // Проверка наличия данных для экспорта
            if (_currentReportData == null || _currentReportData.Rows.Count == 0)
            {
                MessageBox.Show("Нет данных для экспорта", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Application excelApp = null;
            Workbook workbook = null;

            try
            {
                // Создание экземпляра Excel
                excelApp = new Application();
                excelApp.Visible = true;

                // Создание нового рабочего листа
                workbook = excelApp.Workbooks.Add();
                Worksheet worksheet = (Worksheet)workbook.ActiveSheet;

                // 1. Запись заголовков
                for (int i = 0; i < _currentReportData.Columns.Count; i++)
                {
                    worksheet.Cells[1, i + 1] = _currentReportData.Columns[i].ColumnName;
                }

                // 2. Запись данных
                for (int i = 0; i < _currentReportData.Rows.Count; i++)
                {
                    for (int j = 0; j < _currentReportData.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = _currentReportData.Rows[i][j];
                    }
                }

                // 3. Форматирование заголовков
                Range headerRange = worksheet.Range[
                    worksheet.Cells[1, 1],
                    worksheet.Cells[1, _currentReportData.Columns.Count]];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = XlRgbColor.rgbLightGray;
                headerRange.HorizontalAlignment = XlHAlign.xlHAlignCenter;

                // 4. Добавление границ для всей таблицы
                Range dataRange = worksheet.Range[
                    worksheet.Cells[1, 1],
                    worksheet.Cells[_currentReportData.Rows.Count + 1, _currentReportData.Columns.Count]];
                dataRange.Borders.LineStyle = XlLineStyle.xlContinuous;
                dataRange.Borders.Weight = XlBorderWeight.xlThin;

                // 5. Автоподбор ширины столбцов
                worksheet.Columns.AutoFit();

                // 6. Отключение автофильтров
                try
                {
                    worksheet.AutoFilterMode = false;
                }
                catch
                {
                    // Игнорирование ошибок при отключении фильтров
                }

                // Сообщение об успешном экспорте
                MessageBox.Show("Отчет успешно экспортирован в Excel",
                              "Успех",
                              MessageBoxButton.OK,
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                // Обработка ошибок при экспорте
                MessageBox.Show($"Ошибка при экспорте в Excel: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                // Освобождение ресурсов
                if (workbook != null)
                    Marshal.ReleaseComObject(workbook);
                if (excelApp != null)
                    Marshal.ReleaseComObject(excelApp);
            }
        }

        // Обработчик кнопки для генерации отчета операций
        private void OperationsReportBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateOperationsReport();
        }

        // Обработчик кнопки для генерации отчета курсов обмена
        private void ExchangeRatesReportBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateExchangeRatesReport();
        }

        // Обработчик кнопки для генерации отчета балансов валют
        private void CurrencyBalancesReportBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateCurrencyBalancesReport();
        }

        // Обработчик кнопки для экспорта отчета в Excel
        private void ExportToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            ExportToExcel();
        }
    }
}