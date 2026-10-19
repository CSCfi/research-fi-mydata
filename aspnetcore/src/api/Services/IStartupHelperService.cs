using api.Models.Ttv;

namespace api.Services
{
    public interface IStartupHelperService
    {
        DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_ORCID();
        DimRegisteredDataSource GetDimRegisteredDataSourceId_OnStartup_TTV();
    }
}