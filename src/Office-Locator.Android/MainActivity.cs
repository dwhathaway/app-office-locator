using Android.OS;
using Android.App;
using Android.Content.PM;
using Android.Graphics.Drawables;

using ImageCircle.Forms.Plugin.Droid;

using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace OfficeLocator.Droid
{
    [Activity (Label = "Xamarin Office Locator", Icon = "@drawable/ic_launcher", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        string mobileCenterKey = "";

		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);

			global::Xamarin.Forms.Forms.Init (this, savedInstanceState);
			global::Xamarin.FormsMaps.Init (this, savedInstanceState);

			Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            if (!string.IsNullOrEmpty(mobileCenterKey))
            {
                AppCenter.Start(mobileCenterKey,
                       typeof(Analytics), typeof(Crashes));
            }

			LoadApplication (new App ());
			ImageCircleRenderer.Init();

			ActionBar.SetIcon ( new ColorDrawable (Resources.GetColor (Android.Resource.Color.Transparent)));
		}
	}
}

