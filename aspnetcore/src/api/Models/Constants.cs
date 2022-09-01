namespace api.Models
{
    public static class Constants
    {
        /*
         * FieldIdentifiers should be according to
         * https://koodistot.suomi.fi/codescheme;registryCode=research;schemeCode=ttv_field_identifiers
         */
        public static class FieldIdentifiers
        {
            public const int PERSON_NAME = 110;
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
            public const int ACTIVITY_RESEARCH_ACTIVITY = 900;
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
            public const int MEMORY_CACHE_EXPIRATION_SECONDS_LONG = 3600;
        }

        public static class OrganizationIds
        {
            public const string OKM = "02w52zt87";
        }

        public static class OrganizationNames
        {
            // TODO: check correct name, when these are properly populated in the database
            public const string ORCID = "ORCID (test)";
            public const string TTV = "Tiedejatutkimus.fi (test)";
        }

        public static class PurposeNames
        {
            public const string TTV = "Tiedejatutkimus.fi (test)";
        }

        public static class IdentifierlessDataTypes
        {
            public const string ORGANIZATION_NAME = "organization_name";
            public const string ORGANIZATION_UNIT = "organization_unit";
        }

        public static class ReferenceDataCodeSchemes
        {
            public const string PROFILE_SHARING = "tutkijaprofiilin_luvitus";
        }

        public static class ReferenceDataCodeValues
        {
            public const string PROFILE_SHARING_PROFILE_INFORMATION = "01";
            public const string PROFILE_SHARING_EMAIL_ADDRESS = "02";
            public const string PROFILE_SHARING_PHONE_NUMBER = "03";
            public const string PROFILE_SHARING_AFFILIATION_AND_EDUCATION = "04";
            public const string PROFILE_SHARING_PUBLICATIONS = "05";
            public const string PROFILE_SHARING_DATASETS = "06";
            public const string PROFILE_SHARING_GRANTS = "07";
            public const string PROFILE_SHARING_ACTIVITIES_AND_DISTINCTIONS = "08";
        }
    }
}
