using System.Collections.Generic;
using api.Models.Orcid;

namespace api.Services
{
    public interface IOrcidJsonParserService
    {
        OrcidBiography GetBiography(string json);
        OrcidCreditName GetCreditName(string json);
        List<OrcidDataset> GetDatasets(string json);
        List<OrcidEducation> GetEducations(string json);
        List<OrcidEmail> GetEmails(string json);
        List<OrcidEmployment> GetEmployments(string json);
        List<OrcidExternalIdentifier> GetExternalIdentifiers(string json);
        OrcidFamilyName GetFamilyName(string json);
        OrcidGivenNames GetGivenNames(string json);
        List<OrcidKeyword> GetKeywords(string json);
        List<OrcidOtherName> GetOtherNames(string json);
        List<OrcidResearchActivity> GetProfileOnlyResearchActivityItems(string json);
        List<OrcidPublication> GetPublications(string json);
        List<OrcidResearcherUrl> GetResearcherUrls(string json);
        List<OrcidFunding> GetFundings(string json);
        OrcidFunding GetFundingDetail(string fundingDetailJson);
    }
}