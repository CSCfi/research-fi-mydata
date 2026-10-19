namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Online platform or repository (schema: repository).
    /// </summary>
    public class Repository
    {
        /// <summary>Platform or repository identifier, pid_type = 'uri'.</summary>
        public Identifier RepositoryIsIdentifiedBy { get; set; }

        /// <summary>Platform or repository name.</summary>
        public string RepositoryName { get; set; }
    }
}
