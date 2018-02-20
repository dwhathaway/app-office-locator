using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using OfficeLocator.Models;
using OfficeLocator.ViewModels;
using Xamarin.Forms;

namespace OfficeLocator.Pages
{
    public partial class FeedbackPage : ContentPage
    {
        FeedbackViewModel viewModel;
        ToolbarItem butSubmitFeedBack;

        public FeedbackPage(Location location)
        {
            InitializeComponent();
            BindingContext = viewModel = new FeedbackViewModel(this, location);

            butSubmitFeedBack = new ToolbarItem
            {
                Text = "Submit"
            };

            butSubmitFeedBack.Command = viewModel.SaveFeedbackCommand;

            ToolbarItems.Add(butSubmitFeedBack);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                //var locations = await viewModel.GetLocationAsync();
                //              foreach(var store in stores)
                //                  PickerStore.Items.Add(store.Name);
            }
            catch (Exception ex)
            {
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
                DisplayAlert("Uh oh :(", "Unable to get locations, don't worry you can still submit feedback.", "OK");
            }
        }
    }
}
