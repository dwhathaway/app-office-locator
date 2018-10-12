﻿// Helpers/Settings.cs
using System;
using OfficeLocator.Services;
using Xamarin.Essentials;

namespace OfficeLocator.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        //private static ISettings AppSettings
        //{
        //    get
        //    {
        //        return CrossSettings.Current;
        //    }
        //}

        #region Setting Constants

        const string LastSyncKey = "last_sync";
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);


        const string UserIdKey = "userid";
        static readonly string UserIdDefault = string.Empty;

        const string AuthTokenKey = "authtoken";
        static readonly string AuthTokenDefault = string.Empty;

        const string LoginAttemptsKey = "login_attempts";
        const int LoginAttemptsDefault = 0;

        const string NeedsSyncKey = "needs_sync";
        const bool NeedsSyncDefault = true;

        const string HasSyncedDataKey = "has_synced";
        const bool HasSyncedDataDefault = false;

        #endregion


        public static string AuthToken
        {
            get
            {
                return Preferences.Get(AuthTokenKey, AuthTokenDefault);
            }
            set
            {
                Preferences.Set(AuthTokenKey, value);
            }
        }

        public static string UserId
        {
            get
            {
                return Preferences.Get(UserIdKey, UserIdDefault);
            }
            set
            {
                Preferences.Set(UserIdKey, value);
            }
        }

        public static bool IsLoggedIn
        {
            get
            {
                if (!AzureService.UseAuth)
                    return true;

                return !string.IsNullOrWhiteSpace(UserId);
            }
        }
    }
}