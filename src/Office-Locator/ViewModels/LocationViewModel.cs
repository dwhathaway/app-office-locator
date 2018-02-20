using OfficeLocator.Model;

using Plugin.ExternalMaps;
using Plugin.Messaging;

using Xamarin.Forms;
using System.Threading.Tasks;

namespace OfficeLocator
{
    public class LocationViewModel : BaseViewModel
    {
        Command navigateCommand, callCommand;

        public LocationViewModel(Location location, Page page) : base(page)
        {
            Office = location;
        }

        public string Monday { get { return string.Format("{0} - {1}", Office.MondayOpen, Office.MondayClose); } }
        public string Tuesday { get { return string.Format("{0} - {1}", Office.TuesdayOpen, Office.TuesdayClose); } }
        public string Wednesday { get { return string.Format("{0} - {1}", Office.WednesdayOpen, Office.WednesdayClose); } }
        public string Thursday { get { return string.Format("{0} - {1}", Office.ThursdayOpen, Office.ThursdayClose); } }
        public string Friday { get { return string.Format("{0} - {1}", Office.FridayOpen, Office.FridayClose); } }
        public string Saturday { get { return string.Format("{0} - {1}", Office.SaturdayOpen, Office.SaturdayClose); } }
        public string Sunday { get { return string.Format("{0} - {1}", Office.SundayOpen, Office.SundayClose); } }


        public string Address1 { get { return Office.StreetAddress; } }
        public string Address2 { get { return string.Format("{0}, {1} {2}", Office.City, Office.State, Office.ZipCode); } }

        public Command NavigateCommand
        {
            get
            {
                return navigateCommand ?? (navigateCommand = new Command(() =>
                    CrossExternalMaps.Current.NavigateTo(Office.Name, Office.Latitude, Office.Longitude)));
            }
        }

        public Command CallCommand
        {
            get { return callCommand ?? (callCommand = new Command(async () => await ExecuteCallCommand())); }
        }

        public Location Office { get; set; }

        async Task ExecuteCallCommand()
        {
            var phoneCallTask = CrossMessaging.Current.PhoneDialer;
            if (phoneCallTask.CanMakePhoneCall)
            {
                if (!string.IsNullOrEmpty(Office.PhoneNumber))
                {
                    if (await page.DisplayAlert("Call?", "Call " + Office.PhoneNumber + "?", "Call", "Cancel").ConfigureAwait(false))
                    {
                        phoneCallTask.MakePhoneCall(Office.PhoneNumber);
                    }
                }
            }
        }
    }
}