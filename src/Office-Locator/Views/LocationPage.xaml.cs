using OfficeLocator.Model;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
	public partial class LocationPage : ContentPage
	{
		LocationViewModel viewModel;
		public LocationPage (Location location)
		{
			InitializeComponent ();

            Analytics.TrackEvent ("Location", new Dictionary<string, string>
			{
				{"name", location.Name}
			});
			BindingContext = viewModel = new LocationViewModel (location, this);
            ButtonLeaveFeedback.Clicked += (sender, e) =>
            {
                Navigation.PushAsync(new FeedbackPage(location));
            };
        }



		protected override void OnAppearing ()
		{
			base.OnAppearing ();
			var position = new Position(viewModel.Office.Latitude,viewModel.Office.Longitude); // Latitude, Longitude
			var pin = new Pin {
				Type = PinType.Place,
				Position = position,
				Label = viewModel.Office.Name,
				Address = viewModel.Office.StreetAddress
			};
			MyMap.Pins.Add(pin);

			MyMap.MoveToRegion(
				MapSpan.FromCenterAndRadius(
					position, Distance.FromMiles(.2)));
		}
	}
}

