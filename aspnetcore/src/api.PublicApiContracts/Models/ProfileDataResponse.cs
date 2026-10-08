namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Profile data response for public API.
    /// </summary>
    public class ProfileDataResponse

    {
        /// <summary>
        /// Person has profile.
        /// </summary>
        public bool PersonProfileRecognized { get; set; }

        /// <summary>
        /// Person's data.
        /// </summary>
        public PersonProfileData PersonProfileData { get; set; }

        /// <summary>
        /// Consent is granted for this funder.
        /// </summary>
        public bool ConsentGranted { get; set; }

        /// <summary>
        /// Funder is recognized.
        /// </summary>
        public bool RecognizedFunder { get; set; }
    }
}
