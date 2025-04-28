using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class ExchangeRatesPageSenior : Page
    {
        // Текущий пользователь, который работает с этой страницей.
        private Users _currentUser;

        // Список всех валют из базы данных.
        private List<CurrencyData> _allCurrencies;

        // Отфильтрованный список валют, который отображается в таблице.
        private List<CurrencyData> _filteredCurrencies;

        // Последняя отсортированная колонка в таблице.
        private DataGridColumn _lastColumnSorted = null;

        // Последнее направление сортировки (по возрастанию или убыванию).
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        // Внутренний класс для хранения данных о валюте.
        public class CurrencyData
        {
            // Название валюты.
            public string Currency_Name { get; set; }

            // Курс покупки валюты.
            public decimal Buy_Rate { get; set; }

            // Курс продажи валюты.
            public decimal Sell_Rate { get; set; }
        }

        // Конструктор страницы. Принимает текущего пользователя.
        public ExchangeRatesPageSenior(Users user)
        {
            InitializeComponent(); // Инициализация интерфейса.
            _currentUser = user; // Сохранение информации о пользователе.
            LoadCurrencyData();  // Загрузка данных о валютах.
        }

        // Метод загрузки данных о валютах из базы данных.
        private void LoadCurrencyData()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                // Получение всех валют и их курсов из базы данных.
                _allCurrencies = (from c in context.Currencies
                                  join r in context.ExchangeRates on c.Rate_ID equals r.Rate_ID
                                  select new CurrencyData
                                  {
                                      Currency_Name = c.Currency_Name,
                                      Buy_Rate = r.Buy_Rate,
                                      Sell_Rate = r.Sell_Rate
                                  }).ToList();

                // Создание копии всех валют для фильтрации.
                _filteredCurrencies = new List<CurrencyData>(_allCurrencies);

                // Привязка отфильтрованных данных к таблице.
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        // Обработчик изменения текста в поле поиска.
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Применение фильтрации и сортировки после изменения текста.
            ApplyFilterAndSort();
        }

        // Метод для применения фильтрации и сортировки данных.
        private void ApplyFilterAndSort()
        {
            // Получение текста из поля поиска и преобразование его в нижний регистр.
            var searchText = SearchTextBox.Text.ToLower();

            // Если поле поиска пустое или содержит пробелы:
            if (string.IsNullOrWhiteSpace(searchText))
            {
                // Возвращаем все валюты в отфильтрованный список.
                _filteredCurrencies = new List<CurrencyData>(_allCurrencies);
            }
            else
            {
                // Фильтрация валют, где имя валюты содержит заданный текст.
                _filteredCurrencies = _allCurrencies
                    .Where(c => c.Currency_Name.ToLower().Contains(searchText))
                    .ToList();
            }

            // Если была назначена последняя сортировка:
            if (_lastColumnSorted != null)
            {
                // Выполняем сортировку данных.
                Sort(_lastColumnSorted.SortMemberPath, _lastDirection);
            }
            else
            {
                // Если не было назначено предыдущей сортировки, просто привязываем данные к таблице.
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        // Обработчик события сортировки таблицы.
        private void CurrenciesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Предотвращение стандартной сортировки WPF.
            e.Handled = true;

            // Определяем направление сортировки.
            ListSortDirection direction;
            if (e.Column != _lastColumnSorted)
            {
                // Если выбрана новая колонка, сортировка начинается с по возрастанию.
                direction = ListSortDirection.Ascending;
            }
            else
            {
                // Если та же колонка уже отсортирована, меняем направление сортировки.
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            // Выполняем сортировку данных.
            Sort(e.Column.SortMemberPath, direction);

            // Обновляем информацию о последней отсортированной колонке и направлении.
            _lastColumnSorted = e.Column;
            _lastDirection = direction;
        }

        // Метод для выполнения сортировки данных.
        private void Sort(string sortBy, ListSortDirection direction)
        {
            // Получаем объект для управления отображением данных.
            ICollectionView dataView = CollectionViewSource.GetDefaultView(_filteredCurrencies);

            // Очищаем все существующие правила сортировки.
            dataView.SortDescriptions.Clear();

            // Добавляем новое правило сортировки.
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));

            // Обновляем отображение данных.
            dataView.Refresh();
        }
    }
}