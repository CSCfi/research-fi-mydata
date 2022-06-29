using api.Models.Ttv;

namespace api.Services
{
    public interface IStartupHelperService
    {
        DimPurpose GetDimPurposeId_OnStartup_TTV();
        DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_ORCID();
        DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_TTV();
    }
}