using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

/*
 * TtvControllerBase implements utility methods which can be used by all controllers. 
 */
public abstract class TtvControllerBase : ControllerBase
{
    // Get ORCID ID from user claims
    protected string GetOrcidId()
    {
        return User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;
    }

    // Get prefix for log message
    // [timestamp][ORCID ID][ip address]
    public string GetLogPrefix()
    {
        return "[" + DateTime.UtcNow.ToString("s") + "][" + this.GetOrcidId() +  "][" + HttpContext.Connection.RemoteIpAddress?.ToString() + "]";
    }
}