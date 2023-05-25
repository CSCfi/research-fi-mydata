using System;

namespace api.Services
{
    public interface IUtilityService
    {
        DateTime GetCurrentDateTime();
        string GetDatasourceOrganizationName_ORCID();
        string GetDatasourceOrganizationName_TTV();
        string GetOrganizationId_OKM();
        decimal? StringToNullableDecimal(string s);
    }
}