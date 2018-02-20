using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace OfficeLocator
{
    public class LocationsViewModel : BaseViewModel
    {
        readonly IDataStore dataStore;

        bool isRefreshing;

        Command locationListRefreshCommand;

        public LocationsViewModel(Page page) : base(page)
        {
            Title = "Locations";
            dataStore = AzureDataStore.Instance;
            Locations = new ObservableCollection<Location>();
            LocationsGrouped = new ObservableCollection<Grouping<string, Location>>();
        }

        public Command LocationListRefreshCommand
        {
            get { return locationListRefreshCommand ?? (locationListRefreshCommand = new Command(async () => await ExecuteLocationListRefreshCommand(), () => !IsRefreshing)); }
        }

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { SetProperty(ref isRefreshing, value); }
        }

        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Grouping<string, Location>> LocationsGrouped { get; set; }
        public bool ForceSync { get; set; }

        public async Task DeleteLocation(Location location)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                await dataStore.RemoveLocationAsync(location).ConfigureAwait(false);
                Locations.Remove(location);
                Sort();
            }
            catch (Exception ex)
            {
                page.DisplayAlert("Uh Oh :(", "Unable to remove location, please try again", "OK");
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task ExecuteLocationListRefreshCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
                Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = IsRefreshing = true;

            try
            {
                Locations.Clear();

                Geocoder geoCoder = new Geocoder();

                var locations = await dataStore.GetLocationsAsync().ConfigureAwait(false);
                foreach (var location in locations)
                {
                    if (string.IsNullOrWhiteSpace(location.Image))
                        location.Image = "http://refractored.com/images/wc_small.jpg";

                    if (location.Latitude == 0 && location.Longitude == 0)
                    {
                        var address = location.StreetAddress + ", " + location.City +
                        ", " + location.State + ", " + location.ZipCode;
                        var approximateLocations = await geoCoder.GetPositionsForAddressAsync(address).ConfigureAwait(false);

                        Position pos = approximateLocations.FirstOrDefault();
                        location.Latitude = pos.Latitude;
                        location.Longitude = pos.Longitude;
                    }

                    Locations.Add(location);
                }

                Sort();
            }
            catch (Exception ex)
            {
                page.DisplayAlert("Uh Oh :(", "Unable to gather locations.", "OK");
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
            }
            finally
            {
                IsBusy = IsRefreshing = false;
            }
        }

        void Sort()
        {
            LocationsGrouped.Clear();

            var sorted = from location in Locations
                         orderby location.Country, location.City
                         group location by location.Country into locationGroup
                         select new Grouping<string, Location>(locationGroup.Key, locationGroup);

            foreach (var sort in sorted)
                LocationsGrouped.Add(sort);
        }
    }
}