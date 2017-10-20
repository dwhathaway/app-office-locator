using OfficeLocator;
using OfficeLocator.Model;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using PCLStorage;
using Plugin.EmbeddedResource;
using System.Threading.Tasks;
using System.Reflection;
using Xamarin.Forms;
using OfficeLocator.Services;

[assembly: Dependency(typeof(OfflineDataStore))]
namespace OfficeLocator.Services
{
    public class OfflineDataStore : IDataStore
    {

        public async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var json = ResourceLoader.GetEmbeddedResourceString(Assembly.Load(new AssemblyName("OfficeLocator")), "locations.json");
            return JsonConvert.DeserializeObject<List<Location>>(json);
        }

        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
           
            return feedback;
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            return location;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            return new List<Feedback>();
        }


        public Task Init()
        {
            return Task.Run(() => { });
        }

        public async Task<bool> RemoveFeedbackAsync(Feedback feedback)
        {
            return true;
        }

        public async Task<bool> RemoveLocationAsync(Location location)
        {
            return true;
        }

        public Task SyncFeedbacksAsync()
        {
            return Task.Run(() => { });
        }

        public Task SyncStoresAsync()
        {
            return Task.Run(() => { });
        }

        public async Task<Location> UpdateStoreAsync(Location location)
        {
            return location;
        }

        public Task<Location> UpdateLocationAsync(Location office)
        {
            throw new NotImplementedException();
        }

        public Task SyncLocationsAsync()
        {
            throw new NotImplementedException();
        }
    }

}
