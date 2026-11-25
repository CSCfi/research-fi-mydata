using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.Log;
using api.Models.Ttv;
using api.Models.Orcid;

namespace api.Services
{
    public interface IOrcidImportService
    {
        Task<OrcidImportStatistics> ImportOrcidRecordJsonIntoUserProfile(int userprofileId, string json, LogUserIdentification logUserIdentification);
        Task<OrcidImportStatistics> ImportAdditionalData(List<FactFieldValue> factFieldValues, String orcidAccessToken, LogUserIdentification logUserIdentification, bool useOrcidPublicApi = false);
    }
}