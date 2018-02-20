using Android.App;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;

using ImageCircle.Forms.Plugin.Droid;

namespace OfficeLocator.Droid
{
    [Activity(Label = "Xamarin Office Locator", Icon = "@drawable/ic_launcher", ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        string mobileCenterKey = "";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            global::Xamarin.FormsMaps.Init(this, savedInstanceState);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();

            LoadApplication(new App());
            ImageCircleRenderer.Init();

            ActionBar.SetIcon(new ColorDrawable(Color.Transparent));
        }
    }
}

