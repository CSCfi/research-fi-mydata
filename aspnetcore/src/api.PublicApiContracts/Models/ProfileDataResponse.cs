namespace ResearchFi.ProfilePublicApi
{
    /// <summary>
    /// Profile data response for public API (schema: response_for_consent_status).
    /// </summary>
    public class ProfileDataResponse
    {
        /// <summary>
        /// Person has profile.
        /// </summary>
        public bool ProfileFound { get; set; }

        /// <summary>
        /// Controller is registered in the system.
        /// </summary>
        public bool ControllerFound { get; set; }

        /// <summary>
        /// Active consent exists between the person and the controller. Set only when both the controller and the profile are found.
        /// </summary>
        public bool? ConsentActive { get; set; }

        /// <summary>
        /// Link to the page where the person manages consents.
        /// </summary>
        public LanguageVariant ConsentManagementURL { get; set; }

        /// <summary>
        /// Person's data.
        /// </summary>
        public PersonProfileData ProfileData { get; set; }
    }
}
