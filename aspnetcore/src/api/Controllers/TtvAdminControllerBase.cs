using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

/*
 * TtvAdminControllerBase implements utility methods which can be used by all admin controllers. 
 */
public abstract class TtvAdminControllerBase : ControllerBase
{
    // Get that request contains required admin token
    [NonAction]
    protected bool IsAdminTokenAuthorized(IConfiguration configuration)
    {
        return !string.IsNullOrWhiteSpace(configuration["ADMINTOKEN"]) && Request.Headers["admintoken"] == configuration["ADMINTOKEN"];
    }
}