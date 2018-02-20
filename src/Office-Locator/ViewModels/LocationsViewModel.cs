using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using OfficeLocator.Model;
using OfficeLocator.Services;
using Xamarin.Forms.Maps;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
    public class LocationsViewModel : BaseViewModel
    {
        readonly IDataStore dataStore;
        public ObservableCollection<Location> Locations { get; set; }
        public ObservableCollection<Grouping<string, Location>> LocationsGrouped { get; set; }
        public bool ForceSync { get; set; }
        public LocationsViewModel(Page page) : base(page)
        {
            Title = "Locations";
            dataStore = DependencyService.Get<IDataStore>();
            Locations = new ObservableCollection<Location>();
            LocationsGrouped = new ObservableCollection<Grouping<string, Location>>();
        }

        public async Task DeleteLocation(Location location)
        {
            if (IsBusy)
                return;
            IsBusy = true;
            try
            {
                await dataStore.RemoveLocationAsync(location);
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

        private Command getLocationsCommand;
        public Command GetLocationsCommand
        {
            get
            {
                return getLocationsCommand ??
                    (getLocationsCommand = new Command(async () => await ExecuteGetLocationsCommand(), () => { return !IsBusy; }));
            }
        }

        private async Task ExecuteGetLocationsCommand()
        {
            if (IsBusy)
                return;

            if (ForceSync)
                Settings.LastSync = DateTime.Now.AddDays(-30);

            IsBusy = true;
            GetLocationsCommand.ChangeCanExecute();
            try
            {
                Locations.Clear();

                //var stores = new List<Store>();
                Geocoder geoCoder = new Geocoder();

                var locations = await dataStore.GetLocationsAsync();
                foreach (var location in locations)
                {
                    if (string.IsNullOrWhiteSpace(location.Image))
                        location.Image = "http://refractored.com/images/wc_small.jpg";

                    //geocode the street address if the data doesn't contain coordinates
                    if (location.Latitude == 0 && location.Longitude == 0)
                    {
                        var address = location.StreetAddress + ", " + location.City +
                        ", " + location.State + ", " + location.ZipCode;
                        var approximateLocations = await geoCoder.GetPositionsForAddressAsync(address);

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
                IsBusy = false;
                GetLocationsCommand.ChangeCanExecute();
            }

        }

        private void Sort()
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

