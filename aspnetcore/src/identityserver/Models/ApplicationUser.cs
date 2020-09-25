using Microsoft.AspNetCore.Identity;

namespace identityserver.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string OrcidIdentifier { get; set; }
    }
}
