using OfficeLocator.Model;

using Plugin.ExternalMaps;

using Xamarin.Forms;

namespace OfficeLocator
{
    public class LocationViewModel : BaseViewModel
    {
        Command navigateCommand;

        public LocationViewModel(Location location)
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

        public Location Office { get; set; }
    }
}