using OfficeLocator.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
	public partial class LocationsPage : ContentPage
	{
		private LocationsViewModel viewModel;
		public LocationsPage ()
		{
			InitializeComponent ();

            Analytics.TrackEvent ("Locations");

			BindingContext = viewModel = new LocationsViewModel (this);

			LocationList.ItemSelected += (sender, e) => 
			{
				if(LocationList.SelectedItem == null)
					return;


				Navigation.PushAsync(new LocationPage(e.SelectedItem as Location));


				LocationList.SelectedItem = null;
			};

            searchBar.TextChanged += (sender, e) => FilterLocations(searchBar.Text);
            searchBar.SearchButtonPressed += (sender, e) => {
                FilterLocations(searchBar.Text);
            };

           // if (Device.OS == TargetPlatform.WinPhone)
			{
				LocationList.IsGroupingEnabled = false;
				LocationList.ItemsSource = viewModel.Locations;
			}
		}

        public void FilterLocations(string filter)
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
                   .Contains(filter.ToLower())).ToList<Location>();
                LocationList.ItemsSource = newFilteredItems;
            }

            LocationList.EndRefresh();
            if(newFilteredItems != null && newFilteredItems.Count>0)
            {
                LocationList.ScrollTo(newFilteredItems[0], ScrollToPosition.End, true);
            }
        }

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnAppearing ()
		{
			base.OnAppearing ();
			if (viewModel.Locations.Count > 0 || viewModel.IsBusy)
				return;

			viewModel.GetLocationsCommand.Execute (null);
		}
	}
}

