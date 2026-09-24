namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Profile data request from the public API.
    /// </summary>
    public class ProfileDataRequest
    {
        /// <summary>
        /// Who's profile information permission status is queried.
        /// </summary>
        public string PersonKeyIdentifier { get; set; }

        /// <summary>
        /// Funder to whom permission is granted.
        /// </summary>
        public string GrantedController { get; set; }
    }
}
