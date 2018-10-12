using System;
using System.Collections.Generic;
using System.Linq;
using FFImageLoading.Forms.Platform;
using Foundation;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using ObjCRuntime;
using UIKit;

namespace OfficeLocator.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        string appCenterKey = "9b7f85a9-d143-4a07-8eae-e226483721ae";
        public static Func<NSUrl, bool> ResumeWithURL;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            
            UINavigationBar.Appearance.BarTintColor = UIColor.FromRGB(28, 43, 67); //bar background
            UINavigationBar.Appearance.TintColor = UIColor.White; //Tint color of button items
            UINavigationBar.Appearance.SetTitleTextAttributes(new UITextAttributes()
            {
                //Font = UIFont.FromName("HelveticaNeue-Light", (nfloat)20f),
                TextColor = UIColor.White
            });
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent, false);
            UITabBar.Appearance.TintColor = UIColor.FromRGB(28, 43, 67);

            UIView statusBar = UIApplication.SharedApplication.ValueForKey(new NSString("statusBar")) as UIView;
            if (statusBar.RespondsToSelector(new ObjCRuntime.Selector("setBackgroundColor:")))
            {
                statusBar.BackgroundColor = UIColor.FromRGB(28, 43, 67);
            }

            global::Xamarin.Forms.Forms.Init();
            global::Xamarin.FormsMaps.Init();

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            CachedImageRenderer.Init();

            if (!string.IsNullOrEmpty(appCenterKey))
            {
                AppCenter.Start(appCenterKey,
                       typeof(Analytics), typeof(Crashes));
            }

            LoadApplication(new App());


            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            return ResumeWithURL != null && ResumeWithURL(url);
        }
    }
}
