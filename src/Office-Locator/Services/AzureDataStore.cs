using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using Microsoft.AppCenter.Analytics;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;

using OfficeLocator;
using OfficeLocator.Model;

using Plugin.Connectivity;

using Xamarin.Forms;

[assembly: Dependency(typeof(AzureDataStore))]
namespace OfficeLocator
{
    public class AzureDataStore : IDataStore
    {
        static AzureDataStore instance;

        IMobileServiceSyncTable<Location> locationTable;
        IMobileServiceSyncTable<Feedback> feedbackTable;
        bool initialized = false;

        AzureDataStore()
        {
            MobileService = new MobileServiceClient(AzureConstants.MobileServiceClientUrl);
        }

        public static AzureDataStore Instance
        {
            get { return instance ?? (instance = new AzureDataStore()); }
        }

        MobileServiceClient MobileService { get; set; }

        public async Task Init()
        {
            if (initialized)
                return;

            const string path = "synclocations.db";
            var location = new MobileServiceSQLiteStore(path);
            location.DefineTable<Location>();
            location.DefineTable<Feedback>();
            await MobileService.SyncContext.InitializeAsync(location, new MobileServiceSyncHandler());

            locationTable = MobileService.GetSyncTable<Location>();
            feedbackTable = MobileService.GetSyncTable<Feedback>();

            var locations = await GetLocationsAsync();
            initialized = true;
        }

        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            await Init();

            await feedbackTable.InsertAsync(feedback);
            await SyncFeedbacksAsync();
            return feedback;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            await Init();

            await feedbackTable.PullAsync("allFeedbacks", feedbackTable.CreateQuery());

            return await feedbackTable.ToEnumerableAsync();
        }

        public async Task<bool> RemoveFeedbackAsync(Feedback feedback)
        {
            await Init();

            await feedbackTable.DeleteAsync(feedback);
            await SyncFeedbacksAsync();

            return true;
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            await Init();

            await locationTable.InsertAsync(location);
            await SyncLocationsAsync();
            await MobileService.SyncContext.PushAsync();

            return location;
        }

        public async Task<bool> RemoveLocationAsync(Location location)
        {
            await Init();

            await locationTable.DeleteAsync(location);
            await SyncLocationsAsync();
            await MobileService.SyncContext.PushAsync();

            return true;
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            await Init();

            await locationTable.UpdateAsync(location);
            await SyncLocationsAsync();
            await MobileService.SyncContext.PushAsync();

            return location;
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            await Init();

            await SyncLocationsAsync();

            return await locationTable.ToEnumerableAsync();
        }

        public async Task SyncLocationsAsync()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected || !Settings.NeedsSync)
                    return;

                await locationTable.PullAsync("allOffices", locationTable.CreateQuery()/*.Where(o => o.AppId == AppId )*/);
                Settings.LastSync = DateTime.Now;
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
                Debug.WriteLine("Sync Failed:" + ex.Message);
            }
        }

        public async Task SyncFeedbacksAsync()
        {
            try
            {
                Settings.NeedSyncFeedback = true;
                if (!CrossConnectivity.Current.IsConnected)
                    return;


                await MobileService.SyncContext.PushAsync();
                Settings.NeedSyncFeedback = false;
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
                Debug.WriteLine("Sync Failed:" + ex.Message);
            }
        }
    }
}