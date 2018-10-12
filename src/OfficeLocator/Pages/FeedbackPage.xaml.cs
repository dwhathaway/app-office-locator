using System;
using System.Collections.Generic;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
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
        }
    }
}
