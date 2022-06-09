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
            public const int ACTIVITY_PUBLICATION_ORCID = 550;
            public const int ACTIVITY_RESEARCH_DATASET = 600;
            public const int ACTIVITY_INFRASTRUCTURE = 700;
            public const int ACTIVITY_FUNDING_DECISION = 800;
        }

        public static class SourceIdentifiers
        {
            public const string DEMO_COMMON = "DEMO common";
            public const string DEMO = "DEMO";
            public const string TIEDEJATUTKIMUS = "Tiedejatutkimus.fi";
            public const string PROFILE_API = "Profile API";
        }

        public static class SourceDescriptions
        {
            public const string PROFILE_API = "Profile API";
            public const string ORCID = "ORCID";
        }

        public static class PidTypes
        {
            public const string ORCID = "ORCID";
            public const string ORCID_PUT_CODE = "ORCID put code";
        }

        public static class CodeSchemes
        {
            public const string USER_CHOICES = "user choices";
        }

        public static class Cache
        {
            public const int MEMORY_CACHE_EXPIRATION_SECONDS = 60;
        }

        public static class OrganizationNames
        {
            // TODO: check correct name, when these are properly populated in the database
            public const string ORCID = "ORCID (test)";
            public const string TTV = "Tiedejatutkimus.fi (test)";
        }

        public static class IdentifierlessDataTypes
        {
            public const string ORGANIZATION_NAME = "organization_name";
            public const string ORGANIZATION_UNIT = "organization_unit";
        }
    }
}
