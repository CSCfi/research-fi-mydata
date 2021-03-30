using System.Linq;
using Microsoft.AspNetCore.Mvc;

public abstract class TtvControllerBase : ControllerBase
{
    // Get ORCID ID from user claims
    protected string GetOrcidId()
    {
        return User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;
    }
}