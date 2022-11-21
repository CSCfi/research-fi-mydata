using System;
using System.Security.Cryptography;
using api.Models.Ttv;
using Nest;

namespace api.Models.ProfileEditor
{
    public partial class ProfileDataFromSql
    {
        public ProfileDataFromSql()
        {
        }

        // FactFieldValues
        public bool? FactFieldValues_Show { get; set; }
        public bool? FactFieldValues_PrimaryValue { get; set; }
        public int FactFieldValues_DimUserProfileId { get; set; }
        public int FactFieldValues_DimNameId { get; set; }
        public int FactFieldValues_DimWebLinkId { get; set; }
        public int FactFieldValues_DimResearcherDescriptionId { get; set; }
        public int FactFieldValues_DimEmailAddrressId { get; set; }
        public int FactFieldValues_DimTelephoneNumberId { get; set; }
        public int FactFieldValues_DimFieldOfScienceId { get; set; }
        public int FactFieldValues_DimKeywordId { get; set; }
        public int FactFieldValues_DimPidId { get; set; }
        public int FactFieldValues_DimAffiliationId { get; set; }
        public int FactFieldValues_DimIdentifierlessDataId { get; set; }
        public int FactFieldValues_DimEducationId { get; set; }
        public int FactFieldValues_DimPublicationId { get; set; }
        public int FactFieldValues_DimOrcidPublicationId { get; set; }
        public int FactFieldValues_DimResearchActivityId { get; set; }
        public int FactFieldValues_DimFundingDecisionId { get; set; }
        public int FactFieldValues_DimResearchDatasetId { get; set; }
        //public int DimPublicationId { get; set; }
        //public int DimPidIdOrcidPutCode { get; set; }
        //public int DimEventId { get; set; }
        //public int DimCompetenceId { get; set; }
        //public int DimResearchCommunityId { get; set; }
        //public int DimResearcherToResearchCommunityId { get; set; }
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
        // DimRegisteredDataSource => DimOrganization => DimSector
        public string DimRegisteredDataSource_DimOrganization_DimSector_SectorId { get; set; }
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
        // DimEmailAddrress
        public string DimEmailAddrress_Email { get; set; }
        // DimTelephoneNumber
        public string DimTelephoneNumber_TelephoneNumber { get; set; }
        // DimFieldOfScience
        public string DimFieldOfScience_NameFi { get; set; }
        public string DimFieldOfScience_NameEn { get; set; }
        public string DimFieldOfScience_NameSv { get; set; }
        // DimKeyword
        public string DimKeyword_Keyword { get; set; }
        // DimPid
        public string DimPid_PidContent { get; set; }
        public string DimPid_PidType { get; set; }
        // DimAffiliation
        public int DimAffiliation_DimOrganization_Id { get; set; }
        public string DimAffiliation_DimOrganization_OrganizationId { get; set; }
        public string DimAffiliation_DimOrganization_NameFi { get; set; }
        public string DimAffiliation_DimOrganization_NameEn { get; set; }
        public string DimAffiliation_DimOrganization_NameSv { get; set; }
        public string DimAffiliation_DimOrganization_DimSector_SectorId { get; set; }
        public string DimAffiliation_DimOrganization_DimSector_NameFi { get; set; }
        public string DimAffiliation_DimOrganization_DimSector_NameEn { get; set; }
        public string DimAffiliation_DimOrganization_DimSector_NameSv { get; set; }
        public string DimAffiliation_PositionNameFi { get; set; }
        public string DimAffiliation_PositionNameEn { get; set; }
        public string DimAffiliation_PositionNameSv { get; set; }
        public int DimAffiliation_StartDate_Year { get; set; }
        public int DimAffiliation_StartDate_Month { get; set; }
        public int DimAffiliation_StartDate_Day { get; set; }
        public int DimAffiliation_EndDate_Year { get; set; }
        public int DimAffiliation_EndDate_Month { get; set; }
        public int DimAffiliation_EndDate_Day { get; set; }
        public string DimAffiliation_DimReferenceData_NameFi { get; set; }
        // DimIdentifierlessData
        public string DimIdentifierlessData_Type { get; set; }
        public string DimIdentifierlessData_ValueFi { get; set; }
        public string DimIdentifierlessData_ValueEn { get; set; }
        public string DimIdentifierlessData_ValueSv { get; set; }
        public string DimIdentifierlessData_UnlinkedIdentifier { get; set; }
        // DimIdentifierlessData (child)
        public string DimIdentifierlessData_Child_Type { get; set; }
        public string DimIdentifierlessData_Child_ValueFi { get; set; }
        public string DimIdentifierlessData_Child_ValueEn { get; set; }
        public string DimIdentifierlessData_Child_ValueSv { get; set; }
        public string DimIdentifierlessData_Child_UnlinkedIdentifier { get; set; }
        // DimEducation
        public string DimEducation_NameFi { get; set; }
        public string DimEducation_NameEn { get; set; }
        public string DimEducation_NameSv { get; set; }
        public string DimEducation_DegreeGrantingInstitutionName { get; set; }
        public int DimEducation_StartDate_Year { get; set; }
        public int DimEducation_StartDate_Month { get; set; }
        public int DimEducation_StartDate_Day { get; set; }
        public int DimEducation_EndDate_Year { get; set; }
        public int DimEducation_EndDate_Month { get; set; }
        public int DimEducation_EndDate_Day { get; set; }
        // DimPublication
        public string DimPublication_PublicationId { get; set; }
        public string DimPublication_PublicationName { get; set; }
        public int DimPublication_PublicationYear { get; set; }
        public string DimPublication_Doi { get; set; }
        public string DimPublication_PublicationTypeCode { get; set; }
        // DimOrcidPublication
        public string DimOrcidPublication_PublicationId { get; set; }
        public string DimOrcidPublication_PublicationName { get; set; }
        public int DimOrcidPublication_PublicationYear { get; set; }
        public string DimOrcidPublication_Doi { get; set; }
        // DimResearchActivity
        public string DimResearchActivity_NameFi { get; set; }
        public string DimResearchActivity_NameEn { get; set; }
        public string DimResearchActivity_NameSv { get; set; }
        public string DimResearchActivity_DescriptionFi { get; set; }
        public string DimResearchActivity_DescriptionEn { get; set; }
        public string DimResearchActivity_DescriptionSv { get; set; }
        public bool DimResearchActivity_InternationalCollaboration { get; set; }
        public int DimResearchActivity_StartDate_Year { get; set; }
        public int DimResearchActivity_StartDate_Month { get; set; }
        public int DimResearchActivity_StartDate_Day { get; set; }
        public int DimResearchActivity_EndDate_Year { get; set; }
        public int DimResearchActivity_EndDate_Month { get; set; }
        public int DimResearchActivity_EndDate_Day { get; set; }
        public string DimResearchActivity_ActivityType_CodeValue { get; set; }
        public string DimResearchActivity_ActivityType_NameFi { get; set; }
        public string DimResearchActivity_ActivityType_NameEn { get; set; }
        public string DimResearchActivity_ActivityType_NameSv { get; set; }
        public string DimResearchActivity_Role_CodeValue { get; set; }
        public string DimResearchActivity_Role_NameFi { get; set; }
        public string DimResearchActivity_Role_NameEn { get; set; }
        public string DimResearchActivity_Role_NameSv { get; set; }
        // DimFundingDecision
        public string DimFundingDecision_Acronym { get; set; }
        public string DimFundingDecision_FunderProjectNumber { get; set; }
        public string DimFundingDecision_NameFi { get; set; }
        public string DimFundingDecision_NameEn { get; set; }
        public string DimFundingDecision_NameSv { get; set; }
        public string DimFundingDecision_DescriptionFi { get; set; }
        public string DimFundingDecision_DescriptionEn { get; set; }
        public string DimFundingDecision_DescriptionSv { get; set; }
        public decimal DimFundingDecision_amount_in_EUR { get; set; }
        public int DimFundingDecision_StartDate_Year { get; set; }
        public int DimFundingDecision_EndDate_Year { get; set; }
        public string DimFundingDecision_DimTypeOfFunding_NameFi { get; set; }
        public string DimFundingDecision_DimTypeOfFunding_NameEn { get; set; }
        public string DimFundingDecision_DimTypeOfFunding_NameSv { get; set; }
        public string DimFundingDecision_DimCallProgramme_NameFi { get; set; }
        public string DimFundingDecision_DimCallProgramme_NameEn { get; set; }
        public string DimFundingDecision_DimCallProgramme_NameSv { get; set; }
        public string DimFundingDecision_Funder_NameFi { get; set; }
        public string DimFundingDecision_Funder_NameEn { get; set; }
        public string DimFundingDecision_Funder_NameSv { get; set; }
        // DimResearchDataset
        public string DimResearchDataset_LocalIdentifier { get; set; }
        public string DimResearchDataset_NameFi { get; set; }
        public string DimResearchDataset_NameEn { get; set; }
        public string DimResearchDataset_NameSv { get; set; }
        public string DimResearchDataset_DescriptionFi { get; set; }
        public string DimResearchDataset_DescriptionEn { get; set; }
        public string DimResearchDataset_DescriptionSv { get; set; }
        public DateTime? DimResearchDataset_DatasetCreated {get; set; }
    }
}
