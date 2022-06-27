namespace api.Services
{
    /*
     * DataSourceHelperService contains often needed values
     * from DimRegisteredDataSource and DimOrganization.
     *   - ORCID
     *   - TTV
     *   
     *  Idea is to collect the values into this service during application startup.
     *  In code the values can be used from this service instead of querying the DB again.
     */
    public class DataSourceHelperService
    {
        private int dimRegisteredDataSourceId_ORCID;
        private string dimRegisteredDataSourceName_ORCID;
        private int dimOrganizationId_ORCID;
        private string dimOrganizationNameFi_ORCID;
        private string dimOrganizationNameEn_ORCID;
        private string dimOrganizationNameSv_ORCID;
        private int dimRegisteredDataSourceId_TTV;
        private string dimRegisteredDataSourceName_TTV;
        private int dimOrganizationId_TTV;
        private string dimOrganizationNameFi_TTV;
        private string dimOrganizationNameEn_TTV;
        private string dimOrganizationNameSv_TTV;
        private int dimPurposeId_TTV;

        public int DimRegisteredDataSourceId_ORCID
        {
            get { return this.dimRegisteredDataSourceId_ORCID; }
            set { this.dimRegisteredDataSourceId_ORCID = value; }
        }

        public string DimRegisteredDataSourceName_ORCID
        {
            get { return this.dimRegisteredDataSourceName_ORCID; }
            set { this.dimRegisteredDataSourceName_ORCID = value; }
        }

        public int DimOrganizationId_ORCID
        {
            get { return this.dimOrganizationId_ORCID; }
            set { this.dimOrganizationId_ORCID = value; }
        }

        public string DimOrganizationNameFi_ORCID
        {
            get { return this.dimOrganizationNameFi_ORCID; }
            set { this.dimOrganizationNameFi_ORCID = value; }
        }

        public string DimOrganizationNameEn_ORCID
        {
            get { return this.dimOrganizationNameEn_ORCID; }
            set { this.dimOrganizationNameEn_ORCID = value; }
        }

        public string DimOrganizationNameSv_ORCID
        {
            get { return this.dimOrganizationNameSv_ORCID; }
            set { this.dimOrganizationNameSv_ORCID = value; }
        }


        public int DimRegisteredDataSourceId_TTV
        {
            get { return this.dimRegisteredDataSourceId_TTV; }
            set { this.dimRegisteredDataSourceId_TTV = value; }
        }

        public string DimRegisteredDataSourceName_TTV
        {
            get { return this.dimRegisteredDataSourceName_TTV; }
            set { this.dimRegisteredDataSourceName_TTV = value; }
        }

        public int DimOrganizationId_TTV
        {
            get { return this.dimOrganizationId_TTV; }
            set { this.dimOrganizationId_TTV = value; }
        }

        public string DimOrganizationNameFi_TTV
        {
            get { return this.dimOrganizationNameFi_TTV; }
            set { this.dimOrganizationNameFi_TTV = value; }
        }

        public string DimOrganizationNameEn_TTV
        {
            get { return this.dimOrganizationNameEn_TTV; }
            set { this.dimOrganizationNameEn_TTV = value; }
        }

        public string DimOrganizationNameSv_TTV
        {
            get { return this.dimOrganizationNameSv_TTV; }
            set { this.dimOrganizationNameSv_TTV = value; }
        }

        public int DimPurposeId_TTV
        {
            get { return this.dimPurposeId_TTV; }
            set { this.dimPurposeId_TTV = value; }
        }
    }
}