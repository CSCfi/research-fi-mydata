namespace api.Models.Keycloak
{
    public class KeycloakUserDTO
    {
        public KeycloakUserDTO()
        {
            Id = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Orcid = string.Empty;
        }

        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Orcid { get; set; }
    }
}
