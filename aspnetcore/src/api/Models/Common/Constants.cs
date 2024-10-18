namespace api.Models.Common
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
            public const int ACTIVITY_PUBLICATION_PROFILE_ONLY = 550; // TODO: check value
            public const int ACTIVITY_RESEARCH_DATASET = 600;
            public const int ACTIVITY_INFRASTRUCTURE = 700;
            public const int ACTIVITY_FUNDING_DECISION = 800;
            public const int ACTIVITY_RESEARCH_ACTIVITY = 900;
        }

        /*
         * Item meta types are used in application internal logic.
         * These must be never stored in the database.
         * They must only be used in the data provided to the UI.
         * The use case is to map UI's modification data to correct columns in table fact_field_values.
         */
        public static class ItemMetaTypes
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
            public const int ACTIVITY_PUBLICATION_PROFILE_ONLY = 550;
            public const int ACTIVITY_RESEARCH_DATASET = 600;
            public const int ACTIVITY_RESEARCH_DATASET_PROFILE_ONLY = 650;
            public const int ACTIVITY_INFRASTRUCTURE = 700;
            public const int ACTIVITY_FUNDING_DECISION = 800;
            public const int ACTIVITY_FUNDING_DECISION_PROFILE_ONLY = 850;
            public const int ACTIVITY_RESEARCH_ACTIVITY = 900;
            public const int ACTIVITY_RESEARCH_ACTIVITY_PROFILE_ONLY = 950;
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

        public static class Cache
        {
            public const int MEMORY_CACHE_EXPIRATION_SECONDS = 60;
            public const int MEMORY_CACHE_EXPIRATION_SECONDS_LONG = 3600;
        }

        public static class Delays
        {
            public const int ADMIN_UPDATE_ALL_PROFILES_IN_ELASTICSEARCH_DELAY_BETWEEN_API_CALLS_MS = 500;
            public const int ADMIN_UPDATE_ALL_PROFILES_ORCID_DATA_DELAY_BETWEEN_API_CALLS_MS = 500;
        }

        public static class OrganizationIds
        {
            public const string OKM = "02w52zt87";
        }

        public static class OrganizationNames
        {
            // TODO: check correct name, when these are properly populated in the database
            public const string ORCID = "ORCID";
            public const string TTV = "Tiedejatutkimus.fi";
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
            public const string ORCID_RESEARCH_ACTIVITY = "aktiviteetitjaroolit";
            public const string ORCID_FUNDING = "aktiviteetitjaroolit";
            public const string PROFILE_SHARING = "tutkijaprofiilin_luvitus";
            public const string USER_CHOICES = "Kiinnostuksen_ilmaiseminen";
        }

        // https://koodistot.suomi.fi/codescheme;registryCode=research;schemeCode=aktiviteetitjaroolit
        public static class OrcidResearchActivity_To_ReferenceDataCodeValue
        {
            public const string DISTINCTION = "9.2";
            public const string INVITED_POSITION = "6.1";
            public const string MEMBERSHIP = "5";
            public const string PEER_REVIEW = "2.3";
            public const string QUALIFICATION = "16";
            public const string SERVICE = "5";
            public const string WORK_CONFERENCE = "3.1";
            public const string WORK_LECTURE_SPEECH = "3.3";
            public const string WORK_OTHER = "14";
            public const string WORK_SUPERVISED_STUDENT_PUBLICATION = "11.1";
            public const string WORK_TRANSLATION = "2.2"; // TODO: change to "2.2.1" when dim_referencedata includes parent-child relations
        }

        public static class OrcidFundingTypes
        {
            public const string AWARD = "award";
            public const string CONTRACT = "contract";
            public const string GRANT = "grant";
            public const string SALARY_AWARD = "salary-award";
        }

        // https://koodistot.suomi.fi/codescheme;registryCode=research;schemeCode=aktiviteetitjaroolit
        public static class OrcidFundingType_To_ReferenceDataCodeValue
        {
            public const string AWARD = "13.1";
            public const string CONTRACT = "13";
            public const string GRANT = "13";
            public const string SALARY_AWARD = "13";
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

        public static class ApiResponseReasons
        {
            public const string PROFILE_ALREADY_EXISTS = "profile already exists";
            public const string PROFILE_NOT_FOUND = "profile not found";
            public const string PROFILE_CREATE_FAILED = "create profile failed";
            public const string INVALID_REQUEST = "invalid request data";
            public const string NOTHING_TO_MODIFY = "nothing to modify";
            public const string NOTHING_TO_ADD = "nothing to add";
            public const string NOTHING_TO_REMOVE = "nothing to remove";
        }
    }
}
