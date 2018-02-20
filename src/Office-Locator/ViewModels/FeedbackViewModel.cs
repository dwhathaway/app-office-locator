using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AppCenter.Analytics;

using OfficeLocator.Model;

using Xamarin.Forms;

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

        public FeedbackViewModel(Location loc)
        {
            location = loc;
            dataStore = AzureDataStore.Instance;
            Title = "Leave Feedback";
            LocationName = location.Name;
        }

        public event EventHandler FeedbackSubmissionCompleted;

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
                OnErrorOcurred("Please enter some feedback for our team");
                return;
            }

            Message = "Submitting feedback...";
            IsBusy = true;

            try
            {
                await dataStore.AddFeedbackAsync(new Feedback
                {
                    Text = Text,
                    FeedbackDate = DateTime.UtcNow,
                    VisitDate = Date,
                    Rating = Rating,
                    ServiceType = ServiceType,
                    LocationName = LocationName,
                    Name = Name,
                    PhoneNumber = PhoneNumber,
                    RequiresCall = RequiresCall,
                });

                OnFeedbackCompleted();
            }
            catch (Exception ex)
            {
                OnErrorOcurred("Unable to save feedback, please try again");
                Analytics.TrackEvent("Exception", new Dictionary<string, string> {
                    { "Message", ex.Message },
                    { "StackTrace", ex.ToString() }
                });
            }
            finally
            {
                IsBusy = false;
            }
        }

        void OnFeedbackCompleted()
        {
            FeedbackSubmissionCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}

