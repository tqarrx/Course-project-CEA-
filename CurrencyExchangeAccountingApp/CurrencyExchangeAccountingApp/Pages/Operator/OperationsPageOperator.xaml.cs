using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class OperationsPageOperator : Page
    {
        private Users _currentUser; // Текущий пользователь
        private List<OperationData> _allOperations; // Все операции
        private List<OperationData> _filteredOperations; // Отфильтрованные операции
        private DataGridColumn _lastColumnSorted = null; // Последняя отсортированная колонка
        private ListSortDirection _lastDirection = ListSortDirection.Ascending; // Направление сортировки
        private OperationData _selectedOperation; // Выбранная операция

        // Класс для хранения данных об операции
        public class OperationData
        {
            public int Operation_ID { get; set; } // ID операции
            public string Client_Full_Name { get; set; } // ФИО клиента
            public decimal Amount_From { get; set; } // Сумма продажи
            public string Currency_Name_From { get; set; } // Валюта продажи
            public decimal Amount_To { get; set; } // Сумма покупки
            public string Currency_Name_To { get; set; } // Валюта покупки
            public DateTime Operation_Date { get; set; } // Дата операции
            public DateTime? Last_Update_Date { get; set; } // Дата изменения
            public int? User_ID_Correct { get; set; } // ID пользователя-редактора
            public int Client_ID { get; set; } // ID клиента
        }

        public OperationsPageOperator(Users user)
        {
            InitializeComponent(); // Инициализация компонентов
            _currentUser = user; // Сохранение пользователя
            LoadOperationsData(); // Загрузка операций
        }

        private void OperationsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Сохранение выбранной операции
            _selectedOperation = (OperationData)OperationsDataGrid.SelectedItem;
        }

        private void OperationsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (_selectedOperation == null) return; // Проверка выбора

            // Асинхронный переход на страницу редактирования
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (NavigationService != null)
                {
                    NavigationService.Navigate(new EditOperationsPageOperator(_currentUser, _selectedOperation));
                }
                else
                {
                    MessageBox.Show("Ошибка навигации. Попробуйте ещё раз.", "Ошибка",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }));
        }

        private void LoadOperationsData()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                // Загрузка операций с объединением таблиц
                _allOperations = context.Operations
                    .Join(context.Clients, o => o.Client_ID, c => c.Client_ID, (o, c) => new { o, c }) // Клиенты
                    .Join(context.Currencies, oc => oc.o.Currency_ID_From, currFrom => currFrom.Currency_ID, (oc, currFrom) => new { oc.o, oc.c, currFrom }) // Валюта продажи
                    .Join(context.Currencies, occ => occ.o.Currency_ID_To, currTo => currTo.Currency_ID, (occ, currTo) => new OperationData
                    {
                        Operation_ID = occ.o.Operation_ID,
                        Client_Full_Name = occ.c.Client_Full_Name,
                        Amount_From = occ.o.Amount_From,
                        Currency_Name_From = occ.currFrom.Currency_Name,
                        Amount_To = occ.o.Amount_To,
                        Currency_Name_To = currTo.Currency_Name,
                        Operation_Date = occ.o.Operation_Date,
                        Last_Update_Date = occ.o.Last_Update_Date,
                        User_ID_Correct = occ.o.User_ID_Correct,
                        Client_ID = occ.c.Client_ID // Сохранение ID клиента
                    })
                    .ToList();

                _filteredOperations = new List<OperationData>(_allOperations); // Копия для фильтрации
                OperationsDataGrid.ItemsSource = _filteredOperations; // Привязка к DataGrid
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterAndSort(); // Применение фильтра и сортировки
        }

        private void ApplyFilterAndSort()
        {
            var searchText = SearchTextBox.Text.ToLower(); // Текст поиска

            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredOperations = new List<OperationData>(_allOperations); // Без фильтра
            }
            else
            {
                // Фильтрация по тексту
                _filteredOperations = _allOperations
                    .Where(o => o.Client_Full_Name.ToLower().Contains(searchText) || // По клиенту
                               o.Currency_Name_From.ToLower().Contains(searchText) || // По валюте продажи
                               o.Currency_Name_To.ToLower().Contains(searchText)) // По валюте покупки
                    .ToList();
            }

            // Применение сортировки
            if (_lastColumnSorted != null)
            {
                Sort(_lastColumnSorted.SortMemberPath, _lastDirection);
            }
            else
            {
                OperationsDataGrid.ItemsSource = _filteredOperations; // Обновление данных
            }
        }

        private void OperationsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true; // Отмена стандартной сортировки

            // Определение направления сортировки
            ListSortDirection direction;
            if (e.Column != _lastColumnSorted)
            {
                direction = ListSortDirection.Ascending; // По возрастанию
            }
            else
            {
                // Изменение направления
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            Sort(e.Column.SortMemberPath, direction); // Применение сортировки

            // Сохранение параметров сортировки
            _lastColumnSorted = e.Column;
            _lastDirection = direction;
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(_filteredOperations); // Представление
            dataView.SortDescriptions.Clear(); // Очистка сортировки
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction)); // Новая сортировка
            dataView.Refresh(); // Обновление
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedOperation == null)
            {
                MessageBox.Show("Выберите операцию для изменения", "Ошибка",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Переход на страницу редактирования
            NavigationService.Navigate(new EditOperationsPageOperator(_currentUser, _selectedOperation));
        }

        private void AddOperationBtn_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу добавления операции
            NavigationService.Navigate(new AddOperationsPageOperator(_currentUser));
        }
    }
}