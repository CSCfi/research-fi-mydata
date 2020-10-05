using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

[Route("orcid")]
[Authorize]
public class OrcidController : ControllerBase
{
    private readonly OrcidService _orcidService;

    public OrcidController(OrcidService orcidService)
    {
        _orcidService = orcidService;
    }

    [HttpGet]
    public async Task<string> Get()
    {
        var orcid = User.Claims.FirstOrDefault(x => x.Type == "orcid")?.Value;
        return await _orcidService.GetRecord(orcid);
    }
}