using Xunit;
using api.Services;

namespace api.Tests
{
    [Collection("Language service tests")]
    public class LanguageServiceTests
    {
        [Fact(DisplayName = "Get NameTranslation: FI, EN and SV are defined.")]
        public void getNameTranslation_01()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: "B", nameSv: "C");

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("B", nameTranslation.NameEn);
            Assert.Equal("C", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for EN and SV, when EN and SV are empty strings.")]
        public void getNameTranslation_02()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: "", nameSv: "");

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("A", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for EN and SV, when EN and SV are null.")]
        public void getNameTranslation_03()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: null, nameSv: null);

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("A", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of EN is used for FI and SV, when FI and SV are empty strings.")]
        public void getNameTranslation_04()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "", nameEn: "B", nameSv: "");

            Assert.Equal("B", nameTranslation.NameFi);
            Assert.Equal("B", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of EN is used for FI and SV, when FI and SV are null.")]
        public void getNameTranslation_05()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: null, nameEn: "B", nameSv: null);

            Assert.Equal("B", nameTranslation.NameFi);
            Assert.Equal("B", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of SV is used for FI and EN, when FI and EN are empty strings.")]
        public void getNameTranslation_06()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "", nameEn: "", nameSv: "C");

            Assert.Equal("C", nameTranslation.NameFi);
            Assert.Equal("C", nameTranslation.NameEn);
            Assert.Equal("C", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of SV is used for FI and EN, when FI and EN are null.")]
        public void getNameTranslation_07()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: null, nameEn: null, nameSv: "C");

            Assert.Equal("C", nameTranslation.NameFi);
            Assert.Equal("C", nameTranslation.NameEn);
            Assert.Equal("C", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for SV, when SV is an empty string.")]
        public void getNameTranslation_08()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: "B", nameSv: "");

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("B", nameTranslation.NameEn);
            Assert.Equal("A", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for SV, when SV is null.")]
        public void getNameTranslation_09()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: "B", nameSv: null);

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("B", nameTranslation.NameEn);
            Assert.Equal("A", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for EN, when EN is an empty string.")]
        public void getNameTranslation_10()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: "", nameSv: "B");

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of FI is used for EN, when EN is null.")]
        public void getNameTranslation_11()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "A", nameEn: null, nameSv: "B");

            Assert.Equal("A", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of SV is used for FI, when FI is an empty string.")]
        public void getNameTranslation_12()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: "", nameEn: "A", nameSv: "B");

            Assert.Equal("B", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }

        [Fact(DisplayName = "Get NameTranslation: value of SV is used for FI, when FI is null.")]
        public void getNameTranslation_13()
        {
            var languageService = new LanguageService();

            var nameTranslation = languageService.GetNameTranslation(nameFi: null, nameEn: "A", nameSv: "B");

            Assert.Equal("B", nameTranslation.NameFi);
            Assert.Equal("A", nameTranslation.NameEn);
            Assert.Equal("B", nameTranslation.NameSv);
        }
    }
}