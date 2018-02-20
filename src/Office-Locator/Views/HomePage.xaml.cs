using System;
using System.Collections.Generic;

using Microsoft.AppCenter.Analytics;

using Xamarin.Forms;

namespace OfficeLocator
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();

            Analytics.TrackEvent("Home");

            BindingContext = new HomeViewModel(this);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ButShowGovLocations.Clicked += HandleButShowGocLocationsClicked;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            ButShowGovLocations.Clicked -= HandleButShowGocLocationsClicked;
        }

        void HandleButShowGocLocationsClicked(object sender, EventArgs e)
        {
            Navigation.PushModalAsync(new LocationsPage());
        }
    }
}

