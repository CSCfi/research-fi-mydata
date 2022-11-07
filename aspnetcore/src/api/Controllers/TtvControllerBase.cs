using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

/*
 * TtvControllerBase implements utility methods which can be used by all controllers. 
 */
public abstract class TtvControllerBase : ControllerBase
{
    // Get Keycloak user ID from user claims
    [NonAction]
    protected string GetKeycloakUserId()
    {
        //return User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
        return User.Claims.FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
    }

    // Get ORCID ID from user claims
    [NonAction]
    protected string GetOrcidId()
    {
        return User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;
    }

    // Get access token from HttpRequest header Authorization.
    [NonAction]
    protected string GetBearerTokenFromHttpRequest()
    {
        return Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
    }

    // Get ORCID access token from user claims
    [NonAction]
    protected string GetOrcidAccessToken()
    {
        return User.Claims.FirstOrDefault(x => x.Type == "orcid_access_token")?.Value;
    }

    // Get prefix for log message
    // [Keycloak user ID][ORCID ID][ip address]
    [NonAction]
    public string GetLogPrefix()
    {
        return "[ORCID=" + this.GetOrcidId() +  "][IP=" + HttpContext.Connection.RemoteIpAddress?.ToString() + "][Keycloak ID=" + this.GetKeycloakUserId() + "]";
    }

    // Get ORCID public API flag from user claims.
    // This flag is used in testing phase only.
    [NonAction]
    protected string GetOrcidPublicApiFlag()
    {
        return User.Claims.FirstOrDefault(x => x.Type == "use_orcid_public_api")?.Value;
    }
}