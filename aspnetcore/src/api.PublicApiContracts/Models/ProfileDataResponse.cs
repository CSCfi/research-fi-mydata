namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Profile data response for public API (schema: response_for_consent_status).
    /// </summary>
    public class ProfileDataResponse
    {
        /// <summary>
        /// Person has profile.
        /// </summary>
        public bool RecognizedProfile { get; set; }

        /// <summary>
        /// Funder is recognized.
        /// </summary>
        public bool RecognizedFunder { get; set; }

        /// <summary>
        /// Consent is granted for this funder.
        /// </summary>
        public bool ConsentGranted { get; set; }

        /// <summary>
        /// Link to the person's profile.
        /// </summary>
        public LanguageVariant PersonProfileURL { get; set; }

        /// <summary>
        /// Person's data.
        /// </summary>
        public PersonProfileData ProfileData { get; set; }
    }
}
