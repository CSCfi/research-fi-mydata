﻿using System.Collections.Generic;
using api.Models.Orcid;

namespace api.Services
{
    public interface IOrcidJsonParserService
    {
        OrcidBiography GetBiography(string json);
        OrcidCreditName GetCreditName(string json);
        List<OrcidEducation> GetEducations(string json);
        List<OrcidEmail> GetEmails(string json);
        List<OrcidEmployment> GetEmployments(string json);
        List<OrcidExternalIdentifier> GetExternalIdentifiers(string json);
        OrcidFamilyName GetFamilyName(string json);
        OrcidGivenNames GetGivenNames(string json);
        List<OrcidResearchActivity> GetInvitedPositionsAndDistinctions(string json);
        List<OrcidKeyword> GetKeywords(string json);
        List<OrcidOtherName> GetOtherNames(string json);
        List<OrcidPublication> GetPublications(string json);
        List<OrcidResearcherUrl> GetResearcherUrls(string json);
    }
}