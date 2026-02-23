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

        [Fact(DisplayName = "Convert string to nullable decimal")]
        public void convertStringToNullableDecimal_01()
        {
            var utilityService = new UtilityService();
            Assert.Null(utilityService.StringToNullableDecimal(""));
            Assert.Equal<decimal?>(123.45m, utilityService.StringToNullableDecimal("123.45"));
        }

        [Fact(DisplayName = "Capitalize first letter of string")]
        public void capitalizeFirstLetter_01()
        {
            var utilityService = new UtilityService();
            Assert.Equal("Hello world <1234!?", utilityService.CapitalizeFirstLetter("hello world <1234!?"));
            Assert.Equal("Hello world", utilityService.CapitalizeFirstLetter("HELLO WORLD"));
            Assert.Equal("Hello world", utilityService.CapitalizeFirstLetter("hELLO wORLD"));
            Assert.Equal("", utilityService.CapitalizeFirstLetter(""));
            Assert.Null(utilityService.CapitalizeFirstLetter(null));
        }
    }
}