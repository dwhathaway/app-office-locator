using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using MvvmHelpers;
using OfficeLocator.Models;
using OfficeLocator.Services;
using Xamarin.Forms;

namespace OfficeLocator.ViewModels
{
    public class FeedbackViewModel : BaseViewModel
    {
        Location location;
        AzureService azureService;
        Page _page;

        public FeedbackViewModel(Page page, Location loc)
        {
            location = loc;
            azureService = DependencyService.Get<AzureService>();
            this.Title = "Leave Feedback";
            LocationName = location.Name;
            _page = page;
        }

        Command saveFeedbackCommand;
        public Command SaveFeedbackCommand
        {
            get
            {
                return saveFeedbackCommand ??
                    (saveFeedbackCommand = new Command(async () => await ExecuteSaveFeedbackCommand(), () => { return !IsBusy; }));
            }
        }

        async Task ExecuteSaveFeedbackCommand()
        {
            if (IsBusy)
                return;

            if (string.IsNullOrWhiteSpace(Text))
            {
                await _page.DisplayAlert("Enter Feedback", "Please enter some feedback for our team.", "OK");
                return;
            }

            Message = "Submitting feedback...";
            IsBusy = true;
            saveFeedbackCommand.ChangeCanExecute();
            try
            {
                await azureService.AddFeedback(new Feedback
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
                Crashes.TrackError(ex, new Dictionary<string, string> { { "FeedbackViewModel", "Unable to save feedback, please try again" } });
                await _page.DisplayAlert("Uh Oh :(", "Unable to save feedback, please try again.", "OK");
            }
            finally
            {
                IsBusy = false;
                saveFeedbackCommand.ChangeCanExecute();
            }

            await _page.Navigation.PopAsync();

        }

        bool requiresCall = false;
        public bool RequiresCall
        {
            get { return requiresCall; }
            set { SetProperty(ref requiresCall, value); }
        }


        string phone = string.Empty;
        public string PhoneNumber
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        string message = "Loading...";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        string text = string.Empty;
        public string Text
        {
            get { return text; }
            set { SetProperty(ref text, value); }
        }

        int serviceType = 4;
        public int ServiceType
        {
            get { return serviceType; }
            set
            {
                SetProperty(ref serviceType, value);
            }
        }

        int rating = 10;
        public int Rating
        {
            get { return rating; }
            set
            {
                SetProperty(ref rating, value);
            }
        }

        DateTime date = DateTime.Today;
        public DateTime Date
        {
            get { return date; }
            set
            {
                SetProperty(ref date, value);
            }
        }

        public string LocationName { get; set; }

    }
}
