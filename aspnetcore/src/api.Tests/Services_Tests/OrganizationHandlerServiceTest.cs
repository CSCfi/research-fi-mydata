using Xunit;
using api.Services;

namespace api.Tests
{
    [Collection("Organization handler service tests")]
    public class OrganizationHandlerServiceTests
    {
        [Fact(DisplayName = "Get correct DimPid.PidType value for ORCID disambiguation source")]
        public void getPidTypeFromOrcidDisambiguationSource()
        {
            var organizationHandlerService = new OrganizationHandlerService();

            Assert.Equal("GRIDID", organizationHandlerService.MapOrcidDisambiguationSourceToPidType("GRID"));
            Assert.Equal("RinggoldID", organizationHandlerService.MapOrcidDisambiguationSourceToPidType("RINGGOLD"));
            Assert.Equal("RORID", organizationHandlerService.MapOrcidDisambiguationSourceToPidType("ROR"));
            // Unknown disambiguation source must be mapped to empty string
            Assert.Equal("", organizationHandlerService.MapOrcidDisambiguationSourceToPidType("foobar123"));
        }
    }
}