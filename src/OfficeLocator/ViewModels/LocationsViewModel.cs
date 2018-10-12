using System;
using MvvmHelpers;

using Xamarin.Forms;
using OfficeLocator.Models;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;
using System.Linq;
using OfficeLocator.Helpers;
using OfficeLocator.Services;
using Xamarin.Forms.Maps;
using Xamarin.Essentials;
using Location = OfficeLocator.Models.Location;
using Microsoft.AppCenter.Crashes;

namespace OfficeLocator.ViewModels
{
    public class LocationsViewModel : BaseViewModel
    {
        public LocationsViewModel()
        {
        }

        AzureService azureService;

        public ObservableCollection<Location> Locations { get; set;}
        public ObservableCollection<Grouping<string, Location>> LocationsGrouped { get; set; }
        public bool ForceSync { get; set; }

        Page _page;

        public LocationsViewModel (Page page) //: base (page)
        {
            Title = "Locations";
            azureService = DependencyService.Get<AzureService>();
            Locations = new ObservableCollection<Location> ();
            LocationsGrouped = new ObservableCollection<Grouping<string, Location>> ();
            _page = page;
        }

        private Command getLocationsCommand;
        public Command GetLocationsCommand
        {
            get {
                return getLocationsCommand ??
                    (getLocationsCommand = new Command (async () => await ExecuteGetLocationsCommand (), () => {return !IsBusy;}));
            }
        }

        private async Task ExecuteGetLocationsCommand()
        {
            if (IsBusy || !(await LoginAsync()))
                return;

            //if (ForceSync)
                //Settings.LastSync = DateTime.Now.AddDays (-30);

            IsBusy = true;
            GetLocationsCommand.ChangeCanExecute ();
            try{
                Locations.Clear();

                //var stores = new List<Store>();
                Geocoder geoCoder = new Geocoder();

                var locations = await azureService.GetLocations();
                foreach (var location in locations)
                {
                    if (string.IsNullOrWhiteSpace(location.Image))
                        location.Image = "http://refractored.com/images/wc_small.jpg";

                    //added by aditmer on 2/14/18 because GeoCoding fails when offline
                    var current = Connectivity.NetworkAccess;
                    if (!(current == NetworkAccess.Internet))
                    {
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
                    }

                    Locations.Add(location);
                }

                Sort();
            }
            catch(Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "LocationsViewModel", "Unable to gather locations" } });
                await _page.DisplayAlert ("Uh Oh :(", "Unable to gather locations.", "OK");
            }
            finally {
                IsBusy = false;
                GetLocationsCommand.ChangeCanExecute ();
            }

        }

        private void Sort()
        {

            LocationsGrouped.Clear();
            var sorted = from location in Locations
                orderby location.Country, location.City 
                group location by location.Country into locationGroup
                select new Grouping<string, Location>(locationGroup.Key, locationGroup);

            foreach(var sort in sorted)
                LocationsGrouped.Add(sort);
        }

        public Task<bool> LoginAsync()
        {
            if (Settings.IsLoggedIn)
                return Task.FromResult(true);


            return azureService.LoginAsync();
        }
    }
}
