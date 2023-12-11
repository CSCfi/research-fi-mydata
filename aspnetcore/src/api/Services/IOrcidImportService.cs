using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using api.Models.Log;
using api.Models.Ttv;

namespace api.Services
{
    public interface IOrcidImportService
    {
        Task<bool> ImportOrcidRecordJsonIntoUserProfile(int userprofileId, string json, LogUserIdentification logUserIdentification);
        Task<bool> ImportAdditionalData(List<FactFieldValue> factFieldValues, String orcidAccessToken, LogUserIdentification logUserIdentification, bool useOrcidPublicApi = false);
    }
}