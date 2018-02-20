using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

using Newtonsoft.Json;

using OfficeLocator.Model;
using OfficeLocator.Services;

using PCLStorage;

using Plugin.EmbeddedResource;

using Xamarin.Forms;

[assembly: Dependency(typeof(OfflineDataStore))]
namespace OfficeLocator.Services
{
    public class OfflineDataStore : IDataStore
    {
        public Task<IEnumerable<Location>> GetLocationsAsync()
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            var json = ResourceLoader.GetEmbeddedResourceString(Assembly.Load(new AssemblyName("OfficeLocator")), "locations.json");
            return Task.FromResult(JsonConvert.DeserializeObject<IEnumerable<Location>>(json));
        }

        public Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            return Task.FromResult(feedback);
        }

        public Task<Location> AddLocationAsync(Location location)
        {
            return Task.FromResult(location);
        }

        public Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            IEnumerable<Feedback> feedbackList = new List<Feedback>();

            return Task.FromResult(feedbackList);
        }

        public Task Init()
        {
            return Task.CompletedTask;
        }

        public Task<bool> RemoveFeedbackAsync(Feedback feedback)
        {
            return Task.FromResult(true);
        }

        public Task<bool> RemoveLocationAsync(Location location)
        {
            return Task.FromResult(true);
        }

        public Task SyncFeedbacksAsync()
        {
            return Task.CompletedTask;
        }

        public Task SyncStoresAsync()
        {
            return Task.CompletedTask;
        }

        public Task<Location> UpdateStoreAsync(Location location)
        {
            return Task.FromResult(location);
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
