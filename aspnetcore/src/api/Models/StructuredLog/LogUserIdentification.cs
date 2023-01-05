namespace api.Models.Log
{
    public class LogUserIdentification
    {
        public LogUserIdentification(string orcid, string keycloakId, string ip)
        {
            Orcid = orcid;
            KeycloakId = keycloakId;
            Ip = ip;
        }

        public LogUserIdentification(string orcid)
        {
            Orcid = orcid;
            KeycloakId = "";
            Ip = "";
        }

        public LogUserIdentification()
        {
            Orcid = "";
            KeycloakId = "";
            Ip = "";
        }

        public string Orcid { get; set; }
        public string KeycloakId { get; set; }
        public string Ip { get; set; }
    }
}