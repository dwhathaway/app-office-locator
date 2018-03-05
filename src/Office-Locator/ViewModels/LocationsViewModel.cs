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

        public LocationsViewModel()
        {
            Title = "Locations";
            dataStore = AzureDataStore.Instance;
            Locations = new ObservableCollection<Location>();
            GroupedLocationCollection = new ObservableCollection<Grouping<string, Location>>();
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

        public ObservableCollection<Location> Locations { get; }
        public ObservableCollection<Grouping<string, Location>> GroupedLocationCollection { get; set; }
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
                UpdateGroupedLocationCollection();
            }
            catch (Exception ex)
            {
                OnErrorOcurred("Unable to remove location, please try again");

                AppCenterHelpers.LogException(ex);
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
                var geoCoder = new Geocoder();

                var locations = await dataStore.GetLocationsAsync().ConfigureAwait(false);

                if(locations.Any())
                    Locations.Clear();

                foreach (var location in locations)
                {
                    if (string.IsNullOrWhiteSpace(location.Image))
                        location.Image = "http://refractored.com/images/wc_small.jpg";

                    if (location.Latitude == 0 && location.Longitude == 0)
                    {
                        var address = location.StreetAddress + ", " + location.City + ", " + location.State + ", " + location.ZipCode;
                        var approximateAddressLocations = await geoCoder.GetPositionsForAddressAsync(address).ConfigureAwait(false);

                        var addressLocation = approximateAddressLocations.FirstOrDefault();

                        location.Latitude = addressLocation.Latitude;
                        location.Longitude = addressLocation.Longitude;
                    }

                    Locations.Add(location);
                }

                UpdateGroupedLocationCollection();
            }
            catch (Exception ex)
            {
                OnErrorOcurred("Unable to gather locations");

                AppCenterHelpers.LogException(ex);
            }
            finally
            {
                IsBusy = IsRefreshing = false;
            }
        }

        void UpdateGroupedLocationCollection()
        {
            GroupedLocationCollection.Clear();

            var locationGroupingList = from location in Locations
                                       orderby location.Country, location.City
                                       group location by location.Country into locationGroup
                                       select new Grouping<string, Location>(locationGroup.Key, locationGroup);

            foreach (var locationGrouping in locationGroupingList)
                GroupedLocationCollection.Add(locationGrouping);
        }
    }
}