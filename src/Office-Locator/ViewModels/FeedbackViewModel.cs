using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using OfficeLocator.Model;
using Microsoft.AppCenter.Analytics;

namespace OfficeLocator
{
	public class FeedbackViewModel : BaseViewModel
	{
        Location location;
        IDataStore dataStore;
		public FeedbackViewModel (Page page, Location loc) : base (page)
		{
            location = loc;
		    this.dataStore = DependencyService.Get<AzureDataStore> ();
			this.Title = "Leave Feedback";
            LocationName = location.Name;
		}

		//public async Task<IEnumerable<Location>> GetLocationAsync()
		//{
		//	if (IsBusy)
		//		return new List<Location>();

		//	IsBusy = true;
		//	IEnumerable<Location> locations = null;
		//	try{


		//		return await dataStore.GetLocationsAsync ();

		//	}catch(Exception ex) {
		//		page.DisplayAlert ("Uh Oh :(", "Unable to gather locations.", "OK");
		//		Xamarin.Insights.Report (ex);
		//	}
		//	finally {
		//		IsBusy = false;
		//	}

		//	return new List<Location> ();

		//}

		Command saveFeedbackCommand;
		public Command SaveFeedbackCommand
		{
			get {
				return saveFeedbackCommand ??
					(saveFeedbackCommand = new Command (async () => await ExecuteSaveFeedbackCommand (), () => {return !IsBusy;}));
			}
		}

		async Task ExecuteSaveFeedbackCommand()
		{
			if (IsBusy)
				return;

			if(string.IsNullOrWhiteSpace(Text))
			{
				await page.DisplayAlert("Enter Feedback", "Please enter some feedback for our team.", "OK");
				return;
			}

			Message = "Submitting feedback...";
			IsBusy = true;
			saveFeedbackCommand.ChangeCanExecute ();
			try{
				await dataStore.AddFeedbackAsync(new Feedback
					{
						Text = this.Text,
						FeedbackDate = DateTime.UtcNow,
						VisitDate = Date,
						Rating = Rating,
						ServiceType = ServiceType,
						LocationName = LocationName,
						Name = Name,
						PhoneNumber = PhoneNumber,
						RequiresCall = RequiresCall,
					});
			}catch(Exception ex) {
				page.DisplayAlert ("Uh Oh :(", "Unable to save feedback, please try again.", "OK");
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
			}
			finally {
				IsBusy = false;
				saveFeedbackCommand.ChangeCanExecute ();
			}

			await page.Navigation.PopAsync ();

		}

		bool requiresCall = false;
		public bool RequiresCall 
		{
			get { return requiresCall; }
			set { SetProperty (ref requiresCall, value);}
		}


		string phone = string.Empty;
		public string PhoneNumber 
		{
			get { return phone; }
			set { SetProperty (ref phone, value);}
		}

		string name = string.Empty;
		public string Name 
		{
			get { return name; }
			set { SetProperty (ref name, value);}
		}

		string message = "Loading...";
		public string Message 
		{
			get { return message; }
			set { SetProperty (ref message, value);}
		}

		string text = string.Empty;
		public string Text 
		{
			get { return text; }
			set { SetProperty (ref text, value);}
		}

		int serviceType = 4;
		public int ServiceType
		{
			get { return serviceType; }
			set {
				SetProperty (ref serviceType, value);
			}
		}

		int rating = 10;
		public int Rating
		{
			get { return rating; }
			set {
				SetProperty (ref rating, value);
			}
		}

		DateTime date = DateTime.Today;
		public DateTime Date
		{
			get { return date; }
			set {
				SetProperty (ref date, value);
			}
		}

		public string LocationName {get;set;}

	}
}

