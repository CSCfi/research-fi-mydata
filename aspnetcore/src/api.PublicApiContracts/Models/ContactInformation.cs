using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Contact information (schema: contact_information).
    /// </summary>
    public class ContactInformation
    {
        /// <summary>Phone numbers.</summary>
        public List<object> PhoneNumber { get; set; }

        /// <summary>Email addresses.</summary>
        public List<object> Email { get; set; }

        /// <summary>Homepages.</summary>
        public List<WebLink> Homepage { get; set; }
    }
}
