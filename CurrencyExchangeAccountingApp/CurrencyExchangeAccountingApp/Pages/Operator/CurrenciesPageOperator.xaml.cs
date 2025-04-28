using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CurrencyExchangeAccountingApp.Data.Models;

namespace CurrencyExchangeAccountingApp
{
    public partial class CurrenciesPageOperator : Page
    {
        private Users _currentUser;
        private List<Currencies> _allCurrencies;
        private List<Currencies> _filteredCurrencies;
        private DataGridColumn _lastColumnSorted = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public CurrenciesPageOperator(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadCurrencyData();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).Title = $"Валюты - {_currentUser.User_Full_Name} - {_currentUser.Roles.Role_Name}";
        }

        public void LoadCurrencyData()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                _allCurrencies = context.Currencies.ToList();
                _filteredCurrencies = new List<Currencies>(_allCurrencies);
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyFilterAndSort();
        }

        private void ApplyFilterAndSort()
        {
            var searchText = SearchTextBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                _filteredCurrencies = new List<Currencies>(_allCurrencies);
            }
            else
            {
                _filteredCurrencies = _allCurrencies
                    .Where(c => c.Currency_Name.ToLower().Contains(searchText) ||
                               c.Currency_Code.ToLower().Contains(searchText))
                    .ToList();
            }

            if (_lastColumnSorted != null)
            {
                Sort(_lastColumnSorted.SortMemberPath, _lastDirection);
            }
            else
            {
                CurrenciesDataGrid.ItemsSource = _filteredCurrencies;
            }
        }

        private void CurrenciesDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            e.Handled = true;
            ListSortDirection direction;
            if (e.Column != _lastColumnSorted)
            {
                direction = ListSortDirection.Ascending;
            }
            else
            {
                direction = _lastDirection == ListSortDirection.Ascending
                    ? ListSortDirection.Descending
                    : ListSortDirection.Ascending;
            }

            Sort(e.Column.SortMemberPath, direction);
            _lastColumnSorted = e.Column;
            _lastDirection = direction;
        }

        private void Sort(string sortBy, ListSortDirection direction)
        {
            ICollectionView dataView = CollectionViewSource.GetDefaultView(_filteredCurrencies);
            dataView.SortDescriptions.Clear();
            dataView.SortDescriptions.Add(new SortDescription(sortBy, direction));
            dataView.Refresh();
        }
    }
}