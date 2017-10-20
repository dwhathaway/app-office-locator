
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using System.Linq;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Diagnostics;
using System;
using Xamarin.Forms;

//Comment back in to use azure
using OfficeLocator;
using Plugin.Connectivity;
using OfficeLocator.Model;
using PCLStorage;
using Plugin.EmbeddedResource;
using Newtonsoft.Json;
using System.Reflection;
using Xamarin;
using Microsoft.Azure.Mobile.Analytics;

[assembly: Dependency(typeof(AzureDataStore))]
namespace OfficeLocator
{
	public class AzureDataStore : IDataStore
	{
         public MobileServiceClient MobileService { get; set; }

		IMobileServiceSyncTable<Location> locationTable;
		IMobileServiceSyncTable<Feedback> feedbackTable;
		bool initialized = false;

		public AzureDataStore()
		{
            // This is a sample read-only azure site for demo
            // Follow the readme.md in the GitHub repo on how to setup your own.
#error Missing Azure Endpoint URL
            MobileService = new MobileServiceClient("[your endpoint here]");
        }

		public async Task Init()
		{
			initialized = true;
			const string path = "synclocations.db";
			var location = new MobileServiceSQLiteStore(path);
			location.DefineTable<Location>();
			location.DefineTable<Feedback> ();
			await MobileService.SyncContext.InitializeAsync(location, new MobileServiceSyncHandler());

			locationTable = MobileService.GetSyncTable<Location>();
			feedbackTable = MobileService.GetSyncTable<Feedback> ();

			var locations = await GetLocationsAsync ();
        }
        
		public async Task<Feedback> AddFeedbackAsync(Feedback feedback)
		{
			if (!initialized)
				await Init();


			await feedbackTable.InsertAsync(feedback);
			await SyncFeedbacksAsync ();
			return feedback;
		}

		public async Task<IEnumerable<Feedback>> GetFeedbackAsync ()
		{

			if (!initialized)
				await Init();

			await feedbackTable.PullAsync("allFeedbacks", feedbackTable.CreateQuery());

			return await feedbackTable.ToEnumerableAsync();
		}

		public async Task<bool> RemoveFeedbackAsync (Feedback feedback)
		{
			if (!initialized)
				await Init();

			await feedbackTable.DeleteAsync (feedback);
			await SyncFeedbacksAsync ();
			return true;
		}

		public async Task<Location> AddLocationAsync (Location location)
		{
			if (!initialized)
				await Init();

			await locationTable.InsertAsync (location);
			await SyncLocationsAsync ();
			await MobileService.SyncContext.PushAsync();
			return location;
		}

		public async Task<bool> RemoveLocationAsync (Location location)
		{
			if (!initialized)
				await Init();

			await locationTable.DeleteAsync (location);
			await SyncLocationsAsync ();
			await MobileService.SyncContext.PushAsync();
			return true;
		}

		public async Task<Location> UpdateLocationAsync (Location location)
		{
			if (!initialized)
				await Init();

			await locationTable.UpdateAsync (location);
			await SyncLocationsAsync ();
			await MobileService.SyncContext.PushAsync();
			return location;
		}			

		public async Task<IEnumerable<Location>> GetLocationsAsync()
		{
			if (!initialized)
				await Init();

			await SyncLocationsAsync();
			return await locationTable.ToEnumerableAsync();
		}

		public async Task SyncLocationsAsync()
		{
			try
			{
                if(!CrossConnectivity.Current.IsConnected || !Settings.NeedsSync)
                	return;
				
				await locationTable.PullAsync("allOffices", locationTable.CreateQuery()/*.Where(o => o.AppId == AppId )*/);
				Settings.LastSync = DateTime.Now;
			}
			catch(Exception ex)
			{
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
				Debug.WriteLine("Sync Failed:" + ex.Message);
			}
		}

		public async Task SyncFeedbacksAsync ()
		{
			try
			{
				Settings.NeedSyncFeedback = true;
				if(!CrossConnectivity.Current.IsConnected)
					return;


				await MobileService.SyncContext.PushAsync();
				Settings.NeedSyncFeedback = false;
			}
			catch(Exception ex)
			{
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
				Debug.WriteLine("Sync Failed:" + ex.Message);
			}
		}

		static readonly AzureDataStore instance = new AzureDataStore();

		/// <summary>
		/// Gets the instance of the Azure Web Service
		/// </summary>
		public static AzureDataStore Instance
		{
			get
			{
				return instance;
			}
		}

	}
}