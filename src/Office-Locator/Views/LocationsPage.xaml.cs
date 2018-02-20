using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Xamarin.Forms;

namespace OfficeLocator
{
    public partial class LocationsPage : ContentPage
    {
        readonly LocationsViewModel viewModel;

        public LocationsPage()
        {
            InitializeComponent();

            Analytics.TrackEvent("Locations");

            BindingContext = viewModel = new LocationsViewModel(this);

            LocationList.IsGroupingEnabled = false;
            LocationList.ItemsSource = viewModel.Locations;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LocationList.ItemTapped += HandleLocationListItemTapped;
            SearchBar.TextChanged += HandleSearchBarTextChanged;
            SearchBar.SearchButtonPressed += HandleSearchBarSearchButtonPressed;

            LocationList.BeginRefresh();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LocationList.ItemTapped -= HandleLocationListItemTapped;
            SearchBar.TextChanged -= HandleSearchBarTextChanged;
            SearchBar.SearchButtonPressed -= HandleSearchBarSearchButtonPressed;
        }

        void HandleSearchBarSearchButtonPressed(object sender, EventArgs e)
        {
            FilterLocations(SearchBar.Text);
        }

        void HandleSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            FilterLocations(SearchBar.Text);
        }

        async void HandleLocationListItemTapped(object sender, ItemTappedEventArgs e)
        {
            LocationList.SelectedItem = null;

            if (e.Item != null)
                await Navigation.PushAsync(new LocationPage(e.Item as Location));
        }

        void FilterLocations(string filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
            {
                LocationList.ItemsSource = viewModel.Locations;
            }
            else
            {
                var newFilteredItems = viewModel.Locations
                    .Where(x => x.Name.ToUpper()
                    .Contains(filter.ToUpper())).ToList();

                LocationList.ItemsSource = newFilteredItems;

                if (newFilteredItems?.Count > 0)
                    LocationList.ScrollTo(newFilteredItems[0], ScrollToPosition.End, true);
            }
        }
    }
}

