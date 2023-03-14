using Xunit;
using api.Services;
using api.Models.Common;

namespace api.Tests
{
    [Collection("User profile service tests")]
    public class UserProfileServiceTests
    {
        [Fact(DisplayName = "Get FieldIdentifiers")]
        public void getFieldIdentifiers_01()
        {
            var userProfileService = new UserProfileService();
            var fieldIdentifiers = userProfileService.GetFieldIdentifiers();

            Assert.Equal(16, fieldIdentifiers.Count);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_EMAIL_ADDRESS, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_EXTERNAL_IDENTIFIER, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_FIELD_OF_SCIENCE, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_KEYWORD, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_NAME, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_OTHER_NAMES, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_RESEARCHER_DESCRIPTION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_TELEPHONE_NUMBER, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.PERSON_WEB_LINK, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_AFFILIATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_EDUCATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_PUBLICATION_PROFILE_ONLY, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_FUNDING_DECISION, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_DATASET, fieldIdentifiers);
            Assert.Contains<int>(Constants.FieldIdentifiers.ACTIVITY_RESEARCH_ACTIVITY, fieldIdentifiers);
        }
    }
}