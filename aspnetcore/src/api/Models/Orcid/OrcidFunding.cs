namespace api.Models.Orcid
{
    public partial class OrcidFunding {
        public OrcidFunding(
            string type,
            string name,
            string description,
            string amount,
            string currencyCode,
            string disambiguatedOrganizationIdentifier,
            string disambiguationSource,
            string organizationName,
            OrcidDate startDate,
            OrcidDate endDate,
            OrcidPutCode putCode,
            string url,
            string path
            )
        {
            Type = type;
            Name = name;
            Description = description;
            Amount = amount;
            CurrencyCode = currencyCode;
            DisambiguatedOrganizationIdentifier = disambiguatedOrganizationIdentifier;
            DisambiguationSource = disambiguationSource;
            OrganizationName = organizationName;
            StartDate = startDate;
            EndDate = endDate;
            PutCode = putCode;
            Url = url;
            Path = path;
        }

        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string CurrencyCode { get; set; }
        public string DisambiguatedOrganizationIdentifier { get; set; }
        public string DisambiguationSource { get; set; }
        public string OrganizationName { get; set; }
        public OrcidDate StartDate { get; set; }
        public OrcidDate EndDate { get; set; }
        public OrcidPutCode PutCode { get; set; }
        public string Url { get; set; }
        public string Path { get; set; }
    }
}