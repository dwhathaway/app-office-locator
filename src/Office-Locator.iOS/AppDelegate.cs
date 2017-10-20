using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using ImageCircle.Forms.Plugin.iOS;
using OfficeLocator;

using Microsoft.Azure.Mobile;
using Microsoft.Azure.Mobile.Analytics;
using Microsoft.Azure.Mobile.Crashes;

namespace OfficeLocator.iOS
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
        string mobileCenterKey = "";

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
            UITabBar.Appearance.TintColor = UIColor.FromRGB(54, 82, 113);

            global::Xamarin.Forms.Forms.Init();

            Xamarin.FormsMaps.Init();

            if (!string.IsNullOrEmpty(mobileCenterKey))
            { 
                MobileCenter.Start(mobileCenterKey,
                       typeof(Analytics), typeof(Crashes));
            }
            
			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
			SQLitePCL.CurrentPlatform.Init();
			ImageCircleRenderer.Init();


			LoadApplication (new App ());

			return base.FinishedLaunching (app, options);
		}
	}
}

