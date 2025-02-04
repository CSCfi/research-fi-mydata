using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

// https://www.keycloak.org/docs-api/latest/rest-api/#UserRepresentation
namespace api.Models.Keycloak
{
    public class KeycloakUserDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("emailVerified")]
        public bool EmailVerified { get; set; }

        [JsonPropertyName("createdTimestamp")]
        public long CreatedTimestamp { get; set; }

        [JsonPropertyName("enabled")]
        public bool Enabled { get; set; }

        [JsonPropertyName("totp")]
        public bool Totp { get; set; }
        [JsonPropertyName("attributes")]
        public UserAttributes Attributes { get; set; }
        [JsonPropertyName("disableableCredentialTypes")]
        public List<string> DisableableCredentialTypes { get; set; }

        [JsonPropertyName("requiredActions")]
        public List<string> RequiredActions { get; set; }

        [JsonPropertyName("federatedIdentities")]
        public List<FederatedIdentity> FederatedIdentities { get; set; }

        [JsonPropertyName("notBefore")]
        public int NotBefore { get; set; }

        [JsonPropertyName("access")]
        public Access Access { get; set; }
    }

    public class UserAttributes {
        [JsonPropertyName("orcid")]
        public List<string> Orcid { get; set; }
    }
    public class FederatedIdentity
    {
        [JsonPropertyName("identityProvider")]
        public string IdentityProvider { get; set; }
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("userName")]
        public string UserName { get; set; }
    }

    public class Access
    {
        [JsonPropertyName("manageGroupMembership")]
        public bool ManageGroupMembership { get; set; }
        [JsonPropertyName("view")]
        public bool View { get; set; }
        [JsonPropertyName("mapRoles")]
        public bool MapRoles { get; set; }
        [JsonPropertyName("impersonate")]
        public bool Impersonate { get; set; }
        [JsonPropertyName("manage")]
        public bool Manage { get; set; }
    }
}
