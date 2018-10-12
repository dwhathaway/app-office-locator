//#define AUTH

using System;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Sync;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;
using System.Diagnostics;
using Xamarin.Forms;
using OfficeLocator.Helpers;
using OfficeLocator.Authentication;
using OfficeLocator.Models;
using System.IO;
using Xamarin.Essentials;
using OfficeLocator.Services;
using Location = OfficeLocator.Models.Location;
using Microsoft.AppCenter.Crashes;

[assembly: Dependency(typeof(AzureService))]
namespace OfficeLocator.Services
{
    public class AzureService
    {
        public MobileServiceClient Client { get; set; } = null;
        IMobileServiceSyncTable<Location> locationsTable;
        IMobileServiceSyncTable<Feedback> feedbackTable;

#if AUTH
        public static bool UseAuth { get; set; } = true;
#else
        public static bool UseAuth { get; set; } = false;
#endif

        public async Task Initialize()
        {
            if (Client?.SyncContext?.IsInitialized ?? false)
                return;


            var appUrl = "https://my-office-locator.azurewebsites.net";

#if AUTH
            Client = new MobileServiceClient(appUrl, new AuthHandler());

            if (!string.IsNullOrWhiteSpace (Settings.AuthToken) && !string.IsNullOrWhiteSpace (Settings.UserId)) {
                Client.CurrentUser = new MobileServiceUser (Settings.UserId);
                Client.CurrentUser.MobileServiceAuthenticationToken = Settings.AuthToken;
            }
#else
            //Create our client

            Client = new MobileServiceClient(appUrl);

#endif

            //InitialzeDatabase for path
            var path = "syncstore.db";
            path = Path.Combine(MobileServiceClient.DefaultDatabasePath, path);

            //setup our local sqlite store and intialize our table
            var store = new MobileServiceSQLiteStore(path);

            //Define table
            store.DefineTable<Location>();
            store.DefineTable<Feedback>();

            //Initialize SyncContext
            await Client.SyncContext.InitializeAsync(store, new MobileServiceSyncHandler());

            //Get our sync table that will call out to azure
            locationsTable = Client.GetSyncTable<Location>();
            feedbackTable = Client.GetSyncTable<Feedback>();
        }

        public async Task SyncLocations()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (!(current == NetworkAccess.Internet))
                    return;

                await locationsTable.PullAsync("allLocations", locationsTable.CreateQuery());

                await Client.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "AzureService", "Unable to sync locations, that is alright as we have offline capabilities" } });
            }

        }

        public async Task SyncFeedback()
        {
            try
            {
                var current = Connectivity.NetworkAccess;
                if (!(current == NetworkAccess.Internet))
                    return;

                await feedbackTable.PullAsync("allFeedback", feedbackTable.CreateQuery());

                await Client.SyncContext.PushAsync();
            }
            catch (Exception ex)
            {
                Crashes.TrackError(ex, new Dictionary<string, string> { { "AzureService", "Unable to sync feedback, that is alright as we have offline capabilities" } });
            }

        }

        public async Task<IEnumerable<Location>> GetLocations()
        {
            //Initialize & Sync
            await Initialize();
            await SyncLocations();

            return await locationsTable.ToEnumerableAsync();//.OrderBy(c => c.DateUtc).ToEnumerableAsync();

        }

        public async Task<IEnumerable<Feedback>> GetFeedback()
        {
            //Initialize & Sync
            await Initialize();
            await SyncFeedback();

            return await feedbackTable.ToEnumerableAsync();//.OrderBy(c => c.DateUtc).ToEnumerableAsync();
        }

        //public async Task<Location> AddLocation(bool atHome, string locationName)
        //{
        //    await Initialize();

        //    var location = new Location
        //    {
        //        DateUtc = DateTime.UtcNow,
        //        MadeAtHome = atHome,
        //        OS = Device.RuntimePlatform,
        //        Location = location ?? string.Empty
        //    };

        //    await locationsTable.InsertAsync(location);

        //    await SyncLocations();

        //    //return coffee
        //    return location;
        //}

        public async Task<Feedback> AddFeedback(Feedback feedback)
        {
            await Initialize();

            await feedbackTable.InsertAsync(feedback);
            await SyncFeedback();

            return feedback;
        }

        public async Task<bool> LoginAsync()
        {

            await Initialize();

//            var provider = MobileServiceAuthenticationProvider.Twitter;
//            var uriScheme = "coffeecups";


//#if __ANDROID__
//            var user = await Client.LoginAsync(Forms.Context, provider, uriScheme);

//#elif __IOS__
//            OfficeLocator.iOS.AppDelegate.ResumeWithURL = url => url.Scheme == uriScheme && Client.ResumeWithURL(url);
//            var user = await Client.LoginAsync(GetController(), provider, uriScheme);

//#else
//            var user = await Client.LoginAsync(provider, uriScheme);
            
//#endif
            //if (user == null)
            //{
            //    Settings.AuthToken = string.Empty;
            //    Settings.UserId = string.Empty;
            //    Device.BeginInvokeOnMainThread(async () =>
            //    {
            //        await App.Current.MainPage.DisplayAlert("Login Error", "Unable to login, please try again", "OK");
            //    });
            //    return false;
            //}
            //else
            //{
            //    Settings.AuthToken = user.MobileServiceAuthenticationToken;
            //    Settings.UserId = user.UserId;
            //}

            return true;
        }


#if __IOS__
        UIKit.UIViewController GetController()
        {
            var window = UIKit.UIApplication.SharedApplication.KeyWindow;
            var root = window.RootViewController;
            if (root == null)
                return null;

            var current = root;
            while (current.PresentedViewController != null)
            {
                current = current.PresentedViewController;
            }

            return current;
        }
#endif
    }
}
