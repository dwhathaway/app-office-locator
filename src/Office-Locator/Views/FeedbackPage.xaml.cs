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
        readonly ToolbarItem butSubmitFeedBack;

        public FeedbackPage(Location location)
        {
            InitializeComponent();

            Analytics.TrackEvent("Feedback - " + location.Name);

            BindingContext = viewModel = new FeedbackViewModel(this, location);

            butSubmitFeedBack = new ToolbarItem { Text = "Submit" };
            butSubmitFeedBack.Command = viewModel.SaveFeedbackCommand;

            ToolbarItems.Add(butSubmitFeedBack);
        }
    }
}

