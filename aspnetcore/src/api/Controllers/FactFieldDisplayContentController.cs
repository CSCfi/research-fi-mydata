using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FactFieldDisplayContentController : ControllerBase
    {
        private readonly TtvContext _context;

        public FactFieldDisplayContentController(TtvContext context)
        {
            _context = context;
        }

        // GET: api/FactFieldDisplayContent
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FactFieldDisplayContent>>> GetFactFieldDisplayContent()
        {
            return await _context.FactFieldDisplayContent.ToListAsync();
        }
    }
}
