namespace api.Models.ProfileEditor
{
    public partial class ProfileSettings
    {
        public ProfileSettings()
        {
        }

        /// <summary>
        /// When true, removes profile from Elasticsearch and disables Elasticsearch sync for the profile.
        /// When false, adds profile to Elasticsearch and enables Elasticsearch sync for the profile.
        /// </summary>
        public bool? Hidden { get; set; }

        /// <summary>
        /// When true, new data items are automatically made public.
        /// When false, user must manually set new data items public.
        /// </summary>
        public bool? PublishNewData { get; set; }

        /// <summary>
        /// Indicates whether openess should be highlighted in the profile.
        /// </summary>
        public bool? HighlightOpeness { get; set; }
    }
}
