using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace OfficeLocator
{
	public class App : Application
	{
		public App ()
		{
			MainPage = new NavigationPage(new LocationsPage())
			{
				BarTextColor = Color.White
			};
		}

        protected override void OnStart()
        {
            AppCenterHelpers.Start();
        }
	}
}

