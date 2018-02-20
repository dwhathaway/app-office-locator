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

            if (viewModel.Locations.Count > 0 || viewModel.IsBusy)
                return;
            
            LocationList.ItemSelected += HandleLocationListItemSelected;
            SearchBar.TextChanged += HandleSearchBarTextChanged;
            SearchBar.SearchButtonPressed += HandleSearchBarSearchButtonPressed;

			LocationList.BeginRefresh();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            LocationList.ItemSelected -= HandleLocationListItemSelected;
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

        async void HandleLocationListItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (LocationList.SelectedItem == null)
                return;

            await Navigation.PushAsync(new LocationPage(e.SelectedItem as Location));

            LocationList.SelectedItem = null;
        }

        void FilterLocations(string filter)
        {
            LocationList.BeginRefresh();

            List<Location> newFilteredItems = null;

            if (string.IsNullOrWhiteSpace(filter))
            {
                LocationList.ItemsSource = viewModel.Locations;
            }
            else
            {
                newFilteredItems = viewModel.Locations
                    .Where(x => x.Name.ToLower()
                    .Contains(filter.ToLower())).ToList();

                LocationList.ItemsSource = newFilteredItems;
            }

            LocationList.EndRefresh();

            if (newFilteredItems?.Count > 0)
            {
                LocationList.ScrollTo(newFilteredItems[0], ScrollToPosition.End, true);
            }
        }
    }
}

