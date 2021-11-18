using Xunit;
using api.Services;
using api.Models;
using api.Models.ProfileEditor;

using System;

namespace api.Tests
{
    [Collection("Language service tests")]
    public class LanguageServiceTests
    {
        [Fact(DisplayName = "Get Organization: FI, EN and SV are defined.")]
        public void getOrganization_01()
        {
            var languageService = new LanguageService();

            var organization = languageService.getOrganization(nameFi: "A", nameEn: "B", nameSv: "C");

            Assert.Equal("A", organization.NameFi);
            Assert.Equal("B", organization.NameEn);
            Assert.Equal("C", organization.NameSv);
        }

        [Fact(DisplayName = "Get Organization: value of FI is used for EN and SV, when EN and SV are not defined.")]
        public void getOrganization_02()
        {
            var languageService = new LanguageService();

            var organization = languageService.getOrganization(nameFi: "A", nameEn: "", nameSv: "");

            Assert.Equal("A", organization.NameFi);
            Assert.Equal("A", organization.NameEn);
            Assert.Equal("A", organization.NameSv);
        }
    }
}