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
    public partial class ExchangeRatesPageOperator : Page
    {
        private Users _currentUser;
        private List<CurrencyData> _allCurrencies;
        private List<CurrencyData> _filteredCurrencies;
        private DataGridColumn _lastColumnSorted = null;
        private ListSortDirection _lastDirection = ListSortDirection.Ascending;

        public class CurrencyData
        {
            public string Currency_Name { get; set; }
            public decimal Buy_Rate { get; set; }
            public decimal Sell_Rate { get; set; }
        }

        public ExchangeRatesPageOperator(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadCurrencyData();
        }

        private void LoadCurrencyData()
        {
            using (var context = new CurrencyExchangeAccountingEntities())
            {
                _allCurrencies = (from c in context.Currencies
                                  join r in context.ExchangeRates on c.Rate_ID equals r.Rate_ID
                                  select new CurrencyData
                                  {
                                      Currency_Name = c.Currency_Name,
                                      Buy_Rate = r.Buy_Rate,
                                      Sell_Rate = r.Sell_Rate
                                  }).ToList();

                _filteredCurrencies = new List<CurrencyData>(_allCurrencies);
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
                _filteredCurrencies = new List<CurrencyData>(_allCurrencies);
            }
            else
            {
                _filteredCurrencies = _allCurrencies
                    .Where(c => c.Currency_Name.ToLower().Contains(searchText))
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