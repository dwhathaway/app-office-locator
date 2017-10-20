using System;
using Xamarin.Forms;
using Plugin.ExternalMaps;
using Plugin.Messaging;
using OfficeLocator.Model;

namespace OfficeLocator
{
	public class LocationViewModel : BaseViewModel
	{
		public Location Office { get; set;}
		public string Monday{ get { return string.Format("{0} - {1}", Office.MondayOpen, Office.MondayClose); } }
		public string Tuesday{ get { return string.Format("{0} - {1}", Office.TuesdayOpen, Office.TuesdayClose); } }
		public string Wednesday{ get { return string.Format("{0} - {1}", Office.WednesdayOpen, Office.WednesdayClose); } }
		public string Thursday{ get { return string.Format("{0} - {1}", Office.ThursdayOpen, Office.ThursdayClose); } }
		public string Friday{ get { return string.Format("{0} - {1}", Office.FridayOpen, Office.FridayClose); } }
		public string Saturday{ get { return string.Format("{0} - {1}", Office.SaturdayOpen, Office.SaturdayClose); } }
		public string Sunday{ get { return string.Format("{0} - {1}", Office.SundayOpen, Office.SundayClose); } }


		public string Address1 { get {return Office.StreetAddress;}}
		public string Address2 { get { return string.Format ("{0}, {1} {2}", Office.City, Office.State, Office.ZipCode); } }

		public LocationViewModel (Location location, Page page) : base(page)
		{
			this.Office = location;
		}

		Command navigateCommand;
		public Command NavigateCommand
		{
			get { 
					return navigateCommand ?? (navigateCommand = new Command(() =>
					CrossExternalMaps.Current.NavigateTo (Office.Name, Office.Latitude, Office.Longitude)));
				}
		}

		Command callCommand;
		public Command CallCommand
		{
			get {
				return callCommand ?? (callCommand = new Command (async () => {
					var phoneCallTask = MessagingPlugin.PhoneDialer;
					if (phoneCallTask.CanMakePhoneCall)
					{
						if (!string.IsNullOrEmpty(Office.PhoneNumber))
						{
							if (await page.DisplayAlert("Call?", "Call " + Office.PhoneNumber + "?", "Call", "Cancel"))
							{
								phoneCallTask.MakePhoneCall(Office.PhoneNumber);
							}
						}
					}
				}));
			}
		}

	}
}