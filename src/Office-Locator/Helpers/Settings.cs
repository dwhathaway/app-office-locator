using System;

using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace OfficeLocator
{
    public static class Settings
    {
        const string NeedSyncFeedbackKey = "need_sync_feedback";
        const string LastSyncKey = "last_sync";

        static readonly bool NeedSyncFeedbackDefault = false;
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);

        static ISettings AppSettings
        {
            get { return CrossSettings.Current; }
        }

        public static bool NeedsSync
        {
            get { return LastSync < DateTime.Now.AddDays(-7); }
        }

        public static DateTime LastSync
        {
            get { return AppSettings.GetValueOrDefault(LastSyncKey, LastSyncDefault); }
            set { AppSettings.AddOrUpdateValue(LastSyncKey, value); }
        }

        public static bool NeedSyncFeedback
        {
            get { return AppSettings.GetValueOrDefault(NeedSyncFeedbackKey, NeedSyncFeedbackDefault); }
            set { AppSettings.AddOrUpdateValue(NeedSyncFeedbackKey, value); }
        }
    }
}