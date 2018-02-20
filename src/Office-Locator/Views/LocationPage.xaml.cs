using System;
using System.Collections.Generic;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Plugin.Messaging;

using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace OfficeLocator
{
    public partial class LocationPage : ContentPage
    {
        readonly Location location;
        readonly LocationViewModel viewModel;

        public LocationPage(Location location)
        {
            InitializeComponent();

            Analytics.TrackEvent("Location", new Dictionary<string, string>
            {
                {"name", location.Name}
            });

            BindingContext = viewModel = new LocationViewModel(location);

            this.location = location;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ButtonLeaveFeedback.Clicked += HandleButtonLeaveFeedbackClicked;

            var position = new Position(viewModel.Office.Latitude, viewModel.Office.Longitude);
            var pin = new Pin
            {
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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ButtonLeaveFeedback.Clicked -= HandleButtonLeaveFeedbackClicked;
        }

        async void HandleButtonLeaveFeedbackClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FeedbackPage(location));
        }

        async void HandleButtonFindLocationClicked(object sender, EventArgs e)
        {
            var phoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (phoneCallTask.CanMakePhoneCall)
            {
                if (!string.IsNullOrEmpty(viewModel.Office.PhoneNumber))
                {
                    if (await DisplayAlert("Call?", "Call " + viewModel.Office.PhoneNumber + "?", "Call", "Cancel"))
                        phoneCallTask.MakePhoneCall(viewModel.Office.PhoneNumber);
                }
            }
        }
    }
}

