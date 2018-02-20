using System;
using System.Collections.Generic;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Xamarin.Forms;

namespace OfficeLocator
{
    public partial class FeedbackPage : ContentPage
    {
        readonly FeedbackViewModel viewModel;

        public FeedbackPage(Location location)
        {
            InitializeComponent();

            Analytics.TrackEvent("Feedback - " + location.Name);

            viewModel = new FeedbackViewModel(location);
            BindingContext = viewModel;

            var butSubmitFeedBack = new ToolbarItem { Text = "Submit" };
            butSubmitFeedBack.SetBinding(ToolbarItem.CommandProperty, nameof(FeedbackViewModel.SaveFeedbackCommand));

            ToolbarItems.Add(butSubmitFeedBack);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            viewModel.FeedbackSubmissionCompleted += HandleFeedbackSubmissionCompleted;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            viewModel.FeedbackSubmissionCompleted -= HandleFeedbackSubmissionCompleted;
        }

        async void HandleFeedbackSubmissionCompleted(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}

