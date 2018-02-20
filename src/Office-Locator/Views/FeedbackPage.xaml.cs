using OfficeLocator.Model;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
	public partial class FeedbackPage : ContentPage
	{
		FeedbackViewModel viewModel;
		ToolbarItem butSubmitFeedBack;

		public FeedbackPage (Location location)
		{
			InitializeComponent ();
            Analytics.TrackEvent ("Feedback - " + location.Name);
			BindingContext = viewModel = new FeedbackViewModel (this, location);

			butSubmitFeedBack = new ToolbarItem {
				Text = "Submit"
			};

			butSubmitFeedBack.Command = viewModel.SaveFeedbackCommand;

			ToolbarItems.Add (butSubmitFeedBack);

//			PickerRating.SelectedIndex = 10;
//			PickerServiceType.SelectedIndex = 0;
//
//			PickerStore.SelectedIndexChanged += (sender, e) => 
//			{
//				viewModel.StoreName = PickerStore.Items[PickerStore.SelectedIndex];
//			};

		}

		protected override  void OnAppearing ()
		{
			base.OnAppearing ();

			try
			{
				//var locations = await viewModel.GetLocationAsync();
//				foreach(var store in stores)
//					PickerStore.Items.Add(store.Name);
			}
			catch(Exception ex) {
				
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
				DisplayAlert ("Uh oh :(", "Unable to get locations, don't worry you can still submit feedback.", "OK");
			}
		}
	}
}

