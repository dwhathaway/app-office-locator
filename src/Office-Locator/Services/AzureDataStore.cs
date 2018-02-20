using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        bool isInitialized = false;

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
            if (isInitialized)
                return;

            const string path = "synclocations.db";
            var location = new MobileServiceSQLiteStore(Path.Combine(MobileServiceClient.DefaultDatabasePath, path));
            location.DefineTable<Location>();
            location.DefineTable<Feedback>();
            await MobileService.SyncContext.InitializeAsync(location, new MobileServiceSyncHandler()).ConfigureAwait(false);

            locationTable = MobileService.GetSyncTable<Location>();
            feedbackTable = MobileService.GetSyncTable<Feedback>();

			isInitialized = true;
        }

        public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
        {
            await Init().ConfigureAwait(false);

            await feedbackTable.InsertAsync(feedback).ConfigureAwait(false);
            await SyncFeedbacksAsync().ConfigureAwait(false);
            return feedback;
        }

        public async Task<IEnumerable<Feedback>> GetFeedbackAsync()
        {
            await Init().ConfigureAwait(false);

            await feedbackTable.PullAsync("allFeedbacks", feedbackTable.CreateQuery()).ConfigureAwait(false);

            return await feedbackTable.ToEnumerableAsync().ConfigureAwait(false);
        }

        public async Task<bool> RemoveFeedbackAsync(Feedback feedback)
        {
            await Init().ConfigureAwait(false);

            await feedbackTable.DeleteAsync(feedback).ConfigureAwait(false);
            await SyncFeedbacksAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<Location> AddLocationAsync(Location location)
        {
            await Init().ConfigureAwait(false);

            await locationTable.InsertAsync(location).ConfigureAwait(false);
            await SyncLocationsAsync().ConfigureAwait(false);
            await MobileService.SyncContext.PushAsync().ConfigureAwait(false);

            return location;
        }

        public async Task<bool> RemoveLocationAsync(Location location)
        {
            await Init().ConfigureAwait(false);

            await locationTable.DeleteAsync(location).ConfigureAwait(false);
            await SyncLocationsAsync().ConfigureAwait(false);
            await MobileService.SyncContext.PushAsync().ConfigureAwait(false);

            return true;
        }

        public async Task<Location> UpdateLocationAsync(Location location)
        {
            await Init().ConfigureAwait(false);

            await locationTable.UpdateAsync(location).ConfigureAwait(false);
            await SyncLocationsAsync().ConfigureAwait(false);
            await MobileService.SyncContext.PushAsync().ConfigureAwait(false);

            return location;
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync()
        {
            await Init().ConfigureAwait(false);

            await SyncLocationsAsync().ConfigureAwait(false);

            return await locationTable.ToEnumerableAsync().ConfigureAwait(false);
        }

        public async Task SyncLocationsAsync()
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected || !Settings.NeedsSync)
                    return;

                await locationTable.PullAsync("allOffices", locationTable.CreateQuery()).ConfigureAwait(false);
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
                
                await feedbackTable.PullAsync("allFeedback", feedbackTable.CreateQuery()).ConfigureAwait(false);
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