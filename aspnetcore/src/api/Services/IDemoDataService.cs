using System.Threading.Tasks;
using api.Models.Ttv;

namespace api.Services
{
    public interface IDemoDataService
    {
        Task AddDemoDataToUserProfile(string orcidId, DimUserProfile dimUserProfile);
        void AddFieldsOfScience();
        void AddFundingDecisions();
        void AddOrganizations();
        void AddReferenceData();
        void AddRegisteredDatasources();
        string GetDemoOrganization1Name();
        string GetDemoOrganization2Name();
        string GetDemoOrganization3Name();
        string GetDemoOrganizationFunder1Name();
        string GetDemoOrganizationFunder2Name();
        DimOrganization GetOrganization1();
        Task<DimOrganization> GetOrganization1Async();
        DimRegisteredDataSource GetOrganization1RegisteredDataSource();
        Task<DimRegisteredDataSource> GetOrganization1RegisteredDataSourceAsync();
        DimOrganization GetOrganization2();
        Task<DimOrganization> GetOrganization2Async();
        DimRegisteredDataSource GetOrganization2RegisteredDataSource();
        Task<DimRegisteredDataSource> GetOrganization2RegisteredDataSourceAsync();
        DimOrganization GetOrganization3();
        Task<DimOrganization> GetOrganization3Async();
        DimRegisteredDataSource GetOrganization3RegisteredDataSource();
        Task<DimRegisteredDataSource> GetOrganization3RegisteredDataSourceAsync();
        DimOrganization GetOrganizationFunder1();
        Task<DimOrganization> GetOrganizationFunder1Async();
        DimOrganization GetOrganizationFunder2();
        Task<DimOrganization> GetOrganizationFunder2Async();
        string getSourceDescription(string orcidId);
        void InitDemo();
    }
}