using System;
namespace api.Models
{
    public static class Constants
    {
        public static class FieldIdentifiers
        {
            public const int PERSON_LAST_NAME = 101;
            public const int PERSON_FIRST_NAMES = 102;
            public const int PERSON_OTHER_NAMES = 103;
            public const int PERSON_EXTERNAL_IDENTIFIER = 104;
            public const int PERSON_RESEARCHER_DESCRIPTION = 105;
            public const int PERSON_KEYWORD = 106;
            public const int PERSON_FIELD_OF_SCIENCE = 107;
            public const int PERSON_EMAIL_ADDRESS = 108;
            public const int PERSON_TELEPHONE_NUMBER = 109;
            public const int PERSON_WEB_LINK = 110;
            public const int ACTIVITY_AFFILIATION = 300;
            public const int ACTIVITY_EDUCATION = 400;
            public const int ACTIVITY_PUBLICATION = 500;
        }

        public static class SourceIdentifiers
        {
            public const string ORCID = "ORCID";
        }
    }
}
