using System;
using System.Collections.Generic;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Xamarin.Forms;

namespace OfficeLocator
{
    public partial class FeedbackPage : ContentPage
    {
        public FeedbackPage(Location location)
        {
            InitializeComponent();

            Analytics.TrackEvent("Feedback - " + location.Name);

            var viewModel = new FeedbackViewModel(this, location);
            BindingContext = viewModel;

            var butSubmitFeedBack = new ToolbarItem { Text = "Submit" };
            butSubmitFeedBack.SetBinding(ToolbarItem.CommandProperty, nameof(FeedbackViewModel.SaveFeedbackCommand));

            ToolbarItems.Add(butSubmitFeedBack);
        }
    }
}

