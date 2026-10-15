using System.Collections.Generic;

namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Profile data request from the public API (schema: query_for_consent_status).
    /// </summary>
    public class ProfileDataRequest
    {
        /// <summary>
        /// Identifiers of the person whose profile data consent is queried. 1-2 items, pidType is "ORCID" or "HETU".
        /// </summary>
        public List<Identifier> PersonIdentifier { get; set; }

        /// <summary>
        /// ROR identifier of the funder to whom permission is granted.
        /// </summary>
        public string FunderROR { get; set; }
    }
}
