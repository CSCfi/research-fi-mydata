using Xunit;
using api.Services;

namespace api.Tests
{
    [Collection("Utility service tests")]
    public class UtilityServiceTests
    {
        [Fact(DisplayName = "Get correct data source organization name - ORCID")]
        public void getCorrectDataSourceOrganizationName_ORCID()
        {
            var utilityService = new UtilityService();
            Assert.Equal("ORCID", utilityService.GetDatasourceOrganizationName_ORCID());
        }

        [Fact(DisplayName = "Get correct data source organization name - TTV")]
        public void getCorrectDataSourceOrganizationName_TTV()
        {
            var utilityService = new UtilityService();
            Assert.Equal("Tiedejatutkimus.fi", utilityService.GetDatasourceOrganizationName_TTV());
        }

        [Fact(DisplayName = "Get correct organization ID - OKM")]
        public void getCorrectOrganizationID_OKM()
        {
            var utilityService = new UtilityService();
            Assert.Equal("02w52zt87", utilityService.GetOrganizationId_OKM());
        }
    }
}