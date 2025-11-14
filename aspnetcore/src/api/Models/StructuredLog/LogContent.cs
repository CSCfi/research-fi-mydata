namespace api.Models.Log
{
    public static class LogContent
    {
        public const string MESSAGE_TEMPLATE = "{@LogUserIdentification}, {@LogApiInfo}";

        public static class Action
        {
            public const string ADMIN_WEBHOOK_ORCID_REGISTER = "Admin: ORCID webhook: register";
            public const string ADMIN_WEBHOOK_ORCID_UNREGISTER = "Admin: ORCID webhook: unregister";
            public const string ADMIN_WEBHOOK_ORCID_REGISTER_ALL = "Admin: ORCID webhook: register all";
            public const string ADMIN_WEBHOOK_ORCID_UNREGISTER_ALL = "Admin: ORCID webhook unregister all";
            public const string ADMIN_ELASTICSEARCH_PROFILE_DELETE = "Admin: Elasticsearch: profile: delete";
            public const string ADMIN_ELASTICSEARCH_PROFILE_UPDATE = "Admin: Elasticsearch: profile: update";
            public const string ADMIN_ELASTICSEARCH_PROFILE_UPDATE_ALL = "Admin: Elasticsearch: profile: update all";
            public const string ADMIN_ORCID_UPDATE_ALL = "Admin: ORCID: profile: update all";
            public const string ADMIN_HEALTH_CHECK = "Admin: Health check";
            public const string BACKGROUND_UPDATE = "Background: update";
            public const string ELASTICSEARCH_DELETE = "Elasticsearch: profile: delete";
            public const string ELASTICSEARCH_UPDATE = "Elasticsearch: profile: update";
            public const string KEYCLOAK_ACCOUNT_DELETE = "Keycloak: account delete";
            public const string KEYCLOAK_GET_ORCID_TOKENS = "Keycloak: get ORCID tokens";
            public const string KEYCLOAK_GET_RAW_USER_DATA = "Keycloak: get raw user data";
            public const string KEYCLOAK_LINK_ORCID = "Keycloak: link ORCID";
            public const string KEYCLOAK_USER_DELETE = "Keycloak: user delete";
            public const string KEYCLOAK_USER_LOGOUT = "Keycloak: user logout";
            public const string KEYCLOAK_SET_ORCID_ATTRIBUTE = "Keycloak: set ORCID attribute";
            public const string PROFILE_ADD_TTV_DATA = "Profile: add TTV data";
            public const string PROFILE_ADD_TTV_DATA_PUBLICATIONS_BY_DOI = "Profile: add TTV data: match publications by DOI";
            public const string PROFILE_CREATE = "Profile: create";
            public const string PROFILE_DELETE = "Profile: delete";
            public const string PROFILE_DELETE_DATABASE = "Profile: delete from database";
            public const string PROFILE_HIDE = "Profile: hide";
            public const string PROFILE_REVEAL = "Profile: reveal";
            public const string PROFILE_MODIFY_COOPERATION_CHOICES = "Profile: modify: co-operation choices";
            public const string PROFILE_MODIFY_FUNDINGDECISION_ADD = "Profile: modify: add funding decision";
            public const string PROFILE_MODIFY_FUNDINGDECISION_DELETE = "Profile: modify: delete funding decision";
            public const string PROFILE_MODIFY_PUBLICATION_ADD = "Profile: modify: add publication";
            public const string PROFILE_MODIFY_PUBLICATION_DELETE = "Profile: modify: delete publication";
            public const string PROFILE_MODIFY_RESEARCHDATASET_ADD = "Profile: modify: add research dataset";
            public const string PROFILE_MODIFY_RESEARCHDATASET_DELETE = "Profile: modify: delete research dataset";
            public const string PROFILE_MODIFY_SHOW = "Profile: modify: toggle show";
            public const string ORCID_RECORD_GET_MEMBER_API = "ORCID: record: get from member API";
            public const string ORCID_RECORD_GET_PUBLIC_API = "ORCID: record: get from public API";
            public const string ORCID_RECORD_IMPORT = "ORCID: record: import";
            public const string ORCID_RECORD_IMPORT_ADDITIONAL = "ORCID: record: import additional";
            public const string ORCID_RECORD_PARSE = "ORCID: record: parse";
            public const string ORCID_REVOKE_TOKEN = "ORCID: revoke token";
            public const string ORCID_WEBHOOK_REGISTER = "ORCID: webhook: register";
            public const string ORCID_WEBHOOK_UNREGISTER = "ORCID: webhook: unregister";
            public const string ORCID_WEBHOOK_RECEIVED = "ORCID: webhook: received";
            public const string SETTINGS_GET = "Settings: get";
            public const string SETTINGS_SET = "Settings: set";
        }

        public static class ActionState
        {
            public const string INITIALIZING = "initializing";
            public const string START = "start";
            public const string IN_PROGRESS = "in progress";
            public const string COMPLETE = "complete";
            public const string FAILED = "failed";
            public const string CANCELLED = "cancelled";
        }

        public static class ErrorMessage
        {
            public const string USER_PROFILE_NOT_FOUND = "user profile not found";
        }
    }
}
