using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Contact information (schema: contact_information).
    /// </summary>
    public class ContactInformation
    {
        /// <summary>Phone numbers.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> PhoneNumber { get; set; }

        /// <summary>Email addresses.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<object> Email { get; set; }

        /// <summary>Homepages.</summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public List<WebLink> Homepage { get; set; }
    }
}
