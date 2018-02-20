using System;

using Newtonsoft.Json;

namespace OfficeLocator.Model
{
	public class Location
	{
		public Location()
		{
			Name = string.Empty;
			StreetAddress = string.Empty;
			City = string.Empty;
			State = string.Empty;
			Country = string.Empty;
			ZipCode = string.Empty;
			Image = string.Empty;
			Latitude = 0;
            Longitude = 0;
            PhoneNumber = string.Empty;
            LocationCode = string.Empty;

			//Default is 9am - 5pm
			MondayClose = "5PM";
			MondayOpen = "9AM";
			TuesdayClose = "5PM";
			TuesdayOpen = "9AM";
			WednesdayClose = "5PM";
			WednesdayOpen = "9AM";
			ThursdayClose = "5PM";
			ThursdayOpen = "9AM";
			FridayClose = "5PM";
			FridayOpen = "9AM";
			SaturdayClose = "Closed";
			SaturdayOpen = "Closed";
			SundayClose = "Closed";
			SundayOpen = "Closed";
			URL = string.Empty;
		}

        [JsonIgnore]
        public Uri ImageUri
        {
            get { return new System.Uri(Image); }
        }

        public string AppId { get; set; }

		[JsonProperty(PropertyName = "id")]
		public string Id { get; set; }

		[Microsoft.WindowsAzure.MobileServices.Version]
		public string Version { get; set; }

		public string Name { get; set; }

		public string StreetAddress { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string Country { get; set; }

		public string ZipCode { get; set; }

		public string Image { get; set; }

		public string URL { get; set; }

		public double Latitude { get; set; }
		public double Longitude { get; set; }

		public string MondayOpen { get; set; }
		public string MondayClose { get; set; }
		public string TuesdayOpen { get; set; }
		public string TuesdayClose { get; set; }
		public string WednesdayOpen { get; set; }
		public string WednesdayClose { get; set; }
		public string ThursdayOpen { get; set; }
		public string ThursdayClose { get; set; }
		public string FridayOpen { get; set; }
		public string FridayClose { get; set; }
		public string SaturdayOpen { get; set; }
		public string SaturdayClose { get; set; }
		public string SundayOpen { get; set; }
		public string SundayClose { get; set; }

		public string PhoneNumber { get; set; }
		public string LocationCode { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }

	}

}

