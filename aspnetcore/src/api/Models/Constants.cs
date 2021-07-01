using System;
namespace api.Models
{
    public static class Constants
    {
        public static class FieldIdentifiers
        {
            public const int PERSON_NAME = 110;
            public const int PERSON_LAST_NAME = 111;
            public const int PERSON_FIRST_NAMES = 112;
            public const int PERSON_OTHER_NAMES = 120;
            public const int PERSON_EXTERNAL_IDENTIFIER = 130;
            public const int PERSON_RESEARCHER_DESCRIPTION = 140;
            public const int PERSON_KEYWORD = 150;
            public const int PERSON_FIELD_OF_SCIENCE = 160;
            public const int PERSON_EMAIL_ADDRESS = 171;
            public const int PERSON_TELEPHONE_NUMBER = 172;
            public const int PERSON_WEB_LINK = 180;
            public const int ACTIVITY_ROLE_IN_RESERCH_COMMUNITY = 200;
            public const int ACTIVITY_AFFILIATION = 300;
            public const int ACTIVITY_EDUCATION = 400;
            public const int ACTIVITY_PUBLICATION = 500;
        }

        public static class SourceIdentifiers
        {
            public const string ORCID = "ORCID";
            public const string DEMO = "DEMO";
            public const string TIEDEJATUTKIMUS = "Tiedejatutkimus.fi";
        }

        public static class SourceDescriptions
        {
            public const string PROFILE_API = "Profile API";
        }

        public static class PidTypes
        {
            public const string ORCID = "ORCID";
        }
    }
}
