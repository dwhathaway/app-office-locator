using System;
using Newtonsoft.Json;

namespace OfficeLocator.Models
{
    public class Feedback : BaseModel
    {
        public Feedback()
        {
            Text = string.Empty;
            FeedbackDate = DateTime.UtcNow;
            VisitDate = DateTime.UtcNow;
            Rating = 9;
            ServiceType = 4;
            Name = string.Empty;
            PhoneNumber = string.Empty;
            RequiresCall = false;
            LocationName = string.Empty;
        }

        public string Text { get; set; }
        public DateTime FeedbackDate { get; set; }
        public DateTime VisitDate { get; set; }
        public int Rating { get; set; }
        public int ServiceType { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public bool RequiresCall { get; set; }
        public string LocationName { get; set; }

        [JsonIgnore]
        public string VisitDateDisplay
        {
            get { return FeedbackDate.ToString("g"); }
        }
    }
}
