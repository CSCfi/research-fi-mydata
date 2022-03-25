﻿using System;
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
        return User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    }

    // Get ORCID ID from user claims
    [NonAction]
    protected string GetOrcidId()
    {
        // TODO: Add handling of missing claim
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
    // [timestamp][Keycloak user ID][ORCID ID][ip address]
    [NonAction]
    public string GetLogPrefix()
    {
        return "[" + DateTime.UtcNow.ToString("s") + "][" + this.GetKeycloakUserId() + "][" + this.GetOrcidId() +  "][" + HttpContext.Connection.RemoteIpAddress?.ToString() + "]";
    }
}