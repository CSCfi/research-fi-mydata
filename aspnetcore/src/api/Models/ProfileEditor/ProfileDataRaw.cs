using System;

namespace api.Models.ProfileEditor
{
    public partial class ProfileDataRaw
    {
        public ProfileDataRaw()
        {
        }

        // FactFieldValues
        public bool? FactFieldValues_Show { get; set; }
        public bool? FactFieldValues_PrimaryValue { get; set; }
        public int FactFieldValues_DimUserProfileId { get; set; }
        public int FactFieldValues_DimNameId { get; set; }
        public int FactFieldValues_DimWebLinkId { get; set; }
        public int FactFieldValues_DimResearcherDescriptionId { get; set; }
        //public int DimFundingDecisionId { get; set; }
        //public int DimPublicationId { get; set; }
        //public int DimPidId { get; set; }
        //public int DimPidIdOrcidPutCode { get; set; }
        //public int DimResearchActivityId { get; set; }
        //public int DimEventId { get; set; }
        //public int DimEducationId { get; set; }
        //public int DimCompetenceId { get; set; }
        //public int DimResearchCommunityId { get; set; }
        //public int DimTelephoneNumberId { get; set; }
        //public int DimEmailAddrressId { get; set; }
        //public int DimResearcherDescriptionId { get; set; }
        //public int DimIdentifierlessDataId { get; set; }
        //public int DimOrcidPublicationId { get; set; }
        //public int DimKeywordId { get; set; }
        //public int DimAffiliationId { get; set; }
        //public int DimResearcherToResearchCommunityId { get; set; }
        //public int DimFieldOfScienceId { get; set; }
        //public int DimResearchDatasetId { get; set; }
        //public int DimRegisteredDataSourceId { get; set; }
        // DimFieldDisplaySettings
        public int DimFieldDisplaySettings_Id { get; set; }
        public int DimFieldDisplaySettings_FieldIdentifier { get; set; }
        public bool DimFieldDisplaySettings_Show { get; set; }
        // DimRegisteredDataSource
        public int DimRegisteredDataSource_Id { get; set; }
        public string DimRegisteredDataSource_Name { get; set; }
        // DimRegisteredDataSource => DimOrganization
        public string DimRegisteredDataSource_DimOrganization_NameFi { get; set; }
        public string DimRegisteredDataSource_DimOrganization_NameSv { get; set; }
        public string DimRegisteredDataSource_DimOrganization_NameEn { get; set; }
        // DimName
        public string DimName_LastName { get; set; }
        public string DimName_FirstNames { get; set; }
        public string DimName_FullName { get; set; }
        // DimWebLink
        public string DimWebLink_Url { get; set; }
        public string DimWebLink_LinkLabel { get; set; }
        // DimResearcherDescription
        public string DimResearcherDescription_ResearchDescriptionFi { get; set; }
        public string DimResearcherDescription_ResearchDescriptionEn { get; set; }
        public string DimResearcherDescription_ResearchDescriptionSv { get; set; }
    }
}
