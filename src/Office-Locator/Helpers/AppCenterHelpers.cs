using System;
using System.Collections.Generic;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
    public static class AppCenterHelpers
    {
        public static void Start()
        {
            const string mobileCenteriOSKey = "";
            const string mobileCenterAndroidKey = "";

            switch (Xamarin.Forms.Device.RuntimePlatform)
            {
                case Xamarin.Forms.Device.iOS:
                    if (!string.IsNullOrWhiteSpace(mobileCenteriOSKey))
                        Start(mobileCenteriOSKey);
                    break;
                case Xamarin.Forms.Device.Android:
                    if (!string.IsNullOrWhiteSpace(mobileCenterAndroidKey))
                        Start(mobileCenterAndroidKey);
                    break;
                default:
                    throw new PlatformNotSupportedException();
            }
        }

        public static void TrackEvent(string trackIdentifier, IDictionary<string, string> table = null) =>
            Analytics.TrackEvent(trackIdentifier, table);

        public static void TrackEvent(string trackIdentifier, string key, string value)
        {
            IDictionary<string, string> table = new Dictionary<string, string> { { key, value } };

            if (string.IsNullOrWhiteSpace(key) && string.IsNullOrWhiteSpace(value))
                table = null;

            TrackEvent(trackIdentifier, table);
        }

        public static void LogException(Exception exception, IDictionary<string, string> properties = null)
        {
            var exceptionType = exception.GetType().ToString();
            var message = exception.Message;

            System.Diagnostics.Debug.WriteLine(exceptionType);
            System.Diagnostics.Debug.WriteLine($"Error: {message}");

            Crashes.TrackError(exception, properties);
        }

        static void Start(string appSecret)
        {
            AppCenter.Start(appSecret, typeof(Analytics), typeof(Crashes));
        }
    }
}
