namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Source of profile data (schema: registered_data_source).
    /// </summary>
    public class RegisteredDataSource
    {
        /// <summary>Name of the organization that provided the data.</summary>
        public LanguageVariant DataSourceOrganizationName { get; set; }

        /// <summary>Name of the data source, for example Tiedejatutkimus.fi or ORCID.</summary>
        public string DataSourceName { get; set; }
    }
}
