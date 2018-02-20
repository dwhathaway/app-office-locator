using System;

using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace OfficeLocator
{
	public static class Settings
	{
        static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        const string NeedSyncFeedbackKey = "need_sync_feedback";
        static readonly bool NeedSyncFeedbackDefault = false;

        const string LastSyncKey = "last_sync";
        static readonly DateTime LastSyncDefault = DateTime.Now.AddDays(-30);

		#endregion

        public static bool NeedsSync
        {
            get { return LastSync < DateTime.Now.AddDays(-7); }
        }

        public static DateTime LastSync
        {
            get
            {
                return AppSettings.GetValueOrDefault(LastSyncKey, LastSyncDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(LastSyncKey, value);
            }
        }

        public static bool NeedSyncFeedback
        {
            get
            {
                return AppSettings.GetValueOrDefault(NeedSyncFeedbackKey, NeedSyncFeedbackDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(NeedSyncFeedbackKey, value);
            }
        }
	}
}