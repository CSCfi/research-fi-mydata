using Xunit;
using api.Services;
using api.Models.Common;
using api.Models.Ttv;
using System;

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

        [Fact(DisplayName = "Get new DimIdentifierlessDatum for organization name")]
        public void getDimIdentifierlessDatum_organization_name()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            OrganizationHandlerService organizationHandlerService = new OrganizationHandlerService(utilityService);
            // Act
            DimIdentifierlessDatum dimIdentifierlessDatum =
                organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "abc123", nameEn: "bcd234", nameSv:"cde345");
            // Assert
            Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_NAME, dimIdentifierlessDatum.Type);
            Assert.Equal(-1, dimIdentifierlessDatum.DimIdentifierlessDataId);
            Assert.Equal("abc123", dimIdentifierlessDatum.ValueFi);
            Assert.Equal("bcd234", dimIdentifierlessDatum.ValueEn);
            Assert.Equal("cde345", dimIdentifierlessDatum.ValueSv);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, dimIdentifierlessDatum.SourceId);
            Assert.Equal(Constants.SourceDescriptions.ORCID, dimIdentifierlessDatum.SourceDescription);
            Assert.Null(dimIdentifierlessDatum.UnlinkedIdentifier);
        }

        [Fact(DisplayName = "Get new DimIdentifierlessDatum for organization unit")]
        public void getDimIdentifierlessDatum_organization_unit()
        {
            // Arrange
            UtilityService utilityService = new UtilityService();
            OrganizationHandlerService organizationHandlerService = new OrganizationHandlerService(utilityService);
            DimIdentifierlessDatum dimIdentifierlessDatumParent =
                organizationHandlerService.CreateIdentifierlessData_OrganizationName(nameFi: "", nameEn: "", nameSv: "");
            // Act
            DimIdentifierlessDatum dimIdentifierlessDatum =
                organizationHandlerService.CreateIdentifierlessData_OrganizationUnit(
                    parentDimIdentifierlessData: dimIdentifierlessDatumParent,
                    nameFi: "vfr123",
                    nameEn: "bgt234",
                    nameSv: "nhy345");
            // Assert
            Assert.Equal(Constants.IdentifierlessDataTypes.ORGANIZATION_UNIT, dimIdentifierlessDatum.Type);
            Assert.Equal(dimIdentifierlessDatumParent, dimIdentifierlessDatum.DimIdentifierlessData);
            Assert.Equal("vfr123", dimIdentifierlessDatum.ValueFi);
            Assert.Equal("bgt234", dimIdentifierlessDatum.ValueEn);
            Assert.Equal("nhy345", dimIdentifierlessDatum.ValueSv);
            Assert.Equal(Constants.SourceIdentifiers.PROFILE_API, dimIdentifierlessDatum.SourceId);
            Assert.Equal(Constants.SourceDescriptions.ORCID, dimIdentifierlessDatum.SourceDescription);
            Assert.Null(dimIdentifierlessDatum.UnlinkedIdentifier);
        }

        [Fact(DisplayName = "Normalize RORID, ID only")]
        public void normalizeRorId_01()
        {
            // Arrange
            OrganizationHandlerService organizationHandlerService = new OrganizationHandlerService();
            string rorId = "abcd1234";
            // Act
            string actualNormalizedRorId = organizationHandlerService.NormalizeRorId(rorId);
            // Assert
            Assert.Equal(rorId, actualNormalizedRorId);
        }

        [Fact(DisplayName = "Normalize RORID, URL version")]
        public void normalizeRorId_02()
        {
            // Arrange
            OrganizationHandlerService organizationHandlerService = new OrganizationHandlerService();
            string rorIdUrl = "https://ror.org/abcde12345";
            string expectedRorId = "abcde12345";
            // Act
            string actualNormalizedRorId = organizationHandlerService.NormalizeRorId(rorIdUrl);
            // Assert
            Assert.Equal(expectedRorId, actualNormalizedRorId);
        }
    }
}