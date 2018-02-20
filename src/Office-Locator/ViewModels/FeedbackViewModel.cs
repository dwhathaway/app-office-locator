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
		readonly IDataStore dataStore;

        bool requiresCall = false;
        string phone = string.Empty;
        string name = string.Empty;
        string message = "Loading...";
        string text = string.Empty;
        int serviceType = 4;
        int rating = 10;

        Location location;
        Command saveFeedbackCommand;
		DateTime date = DateTime.Today;

        public FeedbackViewModel(Page page, Location loc) : base(page)
        {
            location = loc;
            dataStore = DependencyService.Get<AzureDataStore>();
            Title = "Leave Feedback";
            LocationName = location.Name;
        }

        public Command SaveFeedbackCommand
        {
            get
            {
                return saveFeedbackCommand ??
                    (saveFeedbackCommand = new Command(async () => await ExecuteSaveFeedbackCommand(), () => { return !IsBusy; }));
            }
        }

        public bool RequiresCall
        {
            get { return requiresCall; }
            set { SetProperty(ref requiresCall, value); }
        }

        public string PhoneNumber
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        public int ServiceType
        {
            get { return serviceType; }
            set { SetProperty(ref serviceType, value); }
        }

        public int Rating
        {
            get { return rating; }
            set { SetProperty(ref rating, value); }
        }

        public DateTime Date
        {
            get { return date; }
            set { SetProperty(ref date, value); }
        }

        public string LocationName { get; set; }

        async Task ExecuteSaveFeedbackCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Text))
            {
                await page.DisplayAlert("Enter Feedback", "Please enter some feedback for our team.", "OK");
                return;
            }

            Message = "Submitting feedback...";
            IsBusy = true;
            saveFeedbackCommand.ChangeCanExecute();
            try
            {
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
            }
            catch (Exception ex)
            {
                page.DisplayAlert("Uh Oh :(", "Unable to save feedback, please try again.", "OK");
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
            }
            finally
            {
                IsBusy = false;
                saveFeedbackCommand.ChangeCanExecute();
            }

            await page.Navigation.PopAsync();
        }
    }
}

