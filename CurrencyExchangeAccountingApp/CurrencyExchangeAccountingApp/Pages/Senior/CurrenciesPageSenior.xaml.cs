using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Navigation;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class CurrenciesPageSenior : Page
    {
        // Текущий пользователь, который работает с приложением.
        private Users _currentUser;

        // Список всех доступных валют из базы данных.
        private List<Currencies> _allCurrencies;

        // Отфильтрованный список валют, отображаемый в интерфейсе.
        private List<Currencies> _filteredCurrencies;

        // Последняя отсортированная колонка в таблице.
        private DataGridColumn _lastColumnSorted = null;

        // Направление последней сортировки (по возрастанию или убыванию).
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        // Выбранная валюта в таблице.
        private Currencies _selectedCurrency;

        // Конструктор класса, инициализирующий страницу и загружающий данные о валютах.
        public CurrenciesPageSenior(Users user)
        {
            // Инициализация компонентов WPF (UI элементов).
            InitializeComponent();

            // Установка текущего пользователя.
            _currentUser = user;

            // Подписка на событие обновления данных о валютах.
            App.CurrenciesUpdated += OnCurrenciesUpdated;

            // Подписка на событие выгрузки страницы для освобождения ресурсов.
            this.Unloaded += OnPageUnloaded;

            // Загрузка данных о валютах из базы данных.
            LoadCurrencyData();
        }

        // Обработчик события загрузки страницы.
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Установка заголовка окна с информацией о текущем пользователе и его роли.
            Window.GetWindow(this).Title = $"Валюты - {_currentUser.User_Full_Name} - {_currentUser.Roles.Role_Name}";
        }

        // Обработчик изменения выбора в таблице валют.
        private void CurrenciesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Получение выбранной валюты из таблицы.
            _selectedCurrency = (Currencies)CurrenciesDataGrid.SelectedItem;
        }

        // Обработчик двойного клика по строке таблицы.
        private void CurrenciesListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Если валюта не выбрана, выходим из метода.
            if (_selectedCurrency == null) return;

            // Переход на страницу редактирования баланса выбранной валюты.
            NavigationService?.Navigate(new EditCurrencyBalancePageSenior(_currentUser, _selectedCurrency));
        }

        // Метод для перехода на страницу редактирования баланса валюты.
        private void NavigateToEditBalancePage()
        {
            // Если валюта не выбрана, показываем предупреждение.
            if (_selectedCurrency == null)
            {
                MessageBox.Show("Выберите валюту для изменения остатка", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Если навигационный сервис доступен, переходим на страницу редактирования.
            if (NavigationService != null)
            {
                NavigationService.Navigate(new EditCurrencyBalancePageSenior(_currentUser, _selectedCurrency));
            }
        }

        // Обработчик события обновления данных о валютах.
        private void OnCurrenciesUpdated()
        {
            // Вызов метода загрузки данных в потоке UI.
            Dispatcher.Invoke(LoadCurrencyData);
        }

        // Обработчик события выгрузки страницы.
        private void OnPageUnloaded(object sender, RoutedEventArgs e)
        {
            // Отписка от события обновления данных о валютах.
            App.CurrenciesUpdated -= OnCurrenciesUpdated;

            // Отписка от события выгрузки страницы.
            this.Unloaded -= OnPageUnloaded;
        }

        // Метод для загрузки данных о валютах из базы данных.
        public void LoadCurrencyData()
        {
            // Создание контекста базы данных.
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                // Получение всех валют из базы данных.
                _allCurrencies = context.Currencies.ToList();

                // Вывод информации о каждой валюте в консоль для отладки.
                foreach (var currency in _allCurrencies)
                {
                    Debug.WriteLine($"Currency: {currency.Currency_Code}, Symbol: {currency.Currency_Symbol}");
                }

                // Инициализация отфильтрованного списка всеми валютами.
                _filteredCurrencies = new List<Currencies>(_allCurrencies);

                // Привязка отфильтрованного списка к таблице.
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        // Обработчик изменения текста в поле поиска.
        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Применение фильтрации и сортировки.
            ApplyFilterAndSort();
        }

        // Метод для применения фильтрации и сортировки.
        private void ApplyFilterAndSort()
        {
            // Получение текста из поля поиска в нижнем регистре.
            var searchText = SearchTextBox.Text.ToLower();

            // Если текст пустой, отображаем все валюты.
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredCurrencies = new List<Currencies>(_allCurrencies);
            }
            else
            {
                // Фильтрация валют по имени или коду.
                _filteredCurrencies = _allCurrencies
                    .Where(c => c.Currency_Name.ToLower().Contains(searchText) ||
                               c.Currency_Code.ToLower().Contains(searchText))
                    .ToList();
            }

            // Если была выполнена сортировка ранее, применяем её снова.
            if (_lastColumnSorted != null)
            {
                Sort(_lastColumnSorted.SortMemberPath, _lastDirection);
            }
            else
            {
                // Иначе просто обновляем источник данных таблицы.
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        // Обработчик события сортировки таблицы.
        private void CurrenciesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            // Отмена стандартной сортировки таблицы.
            e.Handled = true;

            // Определение направления сортировки.
            ListSortDirection direction;
            if (e.Column != _lastColumnSorted)
            {
                // Если это новый столбец, сортируем по возрастанию.
                direction = ListSortDirection.Ascending;
            }
            else
            {
                // Если это тот же столбец, меняем направление сортировки.
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            // Выполнение сортировки.
            Sort(e.Column.SortMemberPath, direction);

            // Сохранение информации о последней сортировке.
            _lastColumnSorted = e.Column;
            _lastDirection = direction;
        }

        // Метод для выполнения сортировки.
        private void Sort(string sortBy, ListSortDirection direction)
        {
            // Получение представления коллекции данных.
            ICollectionView dataView = CollectionViewSource.GetDefaultView(_filteredCurrencies);

            // Очистка текущих параметров сортировки.
            dataView.SortDescriptions.Clear();

            // Добавление нового параметра сортировки.
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));

            // Обновление представления данных.
            dataView.Refresh();
        }

        // Обработчик клика по кнопке редактирования баланса.
        private void EditBalanceBtn_Click(object sender, RoutedEventArgs e)
        {
            // Если валюта не выбрана, показываем предупреждение.
            if (_selectedCurrency == null)
            {
                MessageBox.Show("Выберите валюту для изменения остатка", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Переход на страницу редактирования баланса.
            NavigateToEditBalancePage();
        }
    }
}