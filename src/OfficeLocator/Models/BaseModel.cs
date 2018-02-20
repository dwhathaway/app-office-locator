using System;
using Newtonsoft.Json;

namespace OfficeLocator.Models
{
    public class BaseModel
    {
        public BaseModel()
        {
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>The user identifier.</value>
        [JsonProperty("userId")]
        public string UserId { get; set; }


        [Microsoft.WindowsAzure.MobileServices.Version]
        public string AzureVersion { get; set; }

        [JsonProperty("deleted")]
        public bool Deleted { get; set; }

        [JsonProperty("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonProperty("createdAt")]
        public string CreatedAt { get; set; }
    }
}
