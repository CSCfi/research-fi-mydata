using api.Services;
using api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Route("orcid")]
[Authorize]
public class OrcidController : ControllerBase
{
    private readonly OrcidService _orcidService;
    private readonly TtvContext _ttvContext;

    public OrcidController(OrcidService orcidService, TtvContext ttvContext)
    {
        _orcidService = orcidService;
        _ttvContext = ttvContext;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        // Get ORCID ID from user claims
        var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;



        // Create entry into table DimKnownPerson
        var knownPerson = new DimKnownPerson();
        _ttvContext.DimKnownPerson.Add(knownPerson);
        await _ttvContext.SaveChangesAsync();



        // Create entry into table DimPid
        var pid = new DimPid()
        {
            PidContent = orcid,
            DimKnownPersonId = knownPerson.Id,
        };
        _ttvContext.DimPid.Add(pid);
        await _ttvContext.SaveChangesAsync();



        // Create entry into table DimUserProfile
        var userprofile = new DimUserProfile();
        userprofile.DimKnownPerson = knownPerson;
        userprofile.DimKnownPerson = knownPerson;
        _ttvContext.DimUserProfile.Add(userprofile);
        await _ttvContext.SaveChangesAsync();

        // Get record JSON from ORCID and return it as response
        return await _orcidService.GetRecord(orcid);
    }
}