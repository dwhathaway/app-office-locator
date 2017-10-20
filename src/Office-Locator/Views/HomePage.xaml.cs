using System;
using System.Collections.Generic;
using Microsoft.Azure.Mobile.Analytics;
using Xamarin.Forms;

namespace OfficeLocator
{
	public partial class HomePage : ContentPage
	{
		public HomePage ()
		{
			InitializeComponent ();

            Analytics.TrackEvent("Home");

			BindingContext = new HomeViewModel (this);

			butShowGovLocations.Clicked += (sender, e) => 
			{
				Navigation.PushModalAsync(new LocationsPage());
			};

			butShowPrivateSectorLocations.Clicked += (sender, e) => 
			{
				//Navigation.PushAsync(new FeedbackPage());
			};
		}
	}
}

