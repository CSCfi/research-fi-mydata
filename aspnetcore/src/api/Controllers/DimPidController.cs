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
    public class DimPidController : ControllerBase
    {
        private readonly TtvContext _context;

        public DimPidController(TtvContext context)
        {
            _context = context;
        }

        // GET: api/DimPid
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DimPid>>> GetDimPid()
        {
            return await _context.DimPid.ToListAsync();
        }

        // GET: api/DimPid/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DimPid>> GetDimPid(string id)
        {
            var dimPid = await _context.DimPid.FindAsync(id);

            if (dimPid == null)
            {
                return NotFound();
            }

            return dimPid;
        }

        // PUT: api/DimPid/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDimPid(string id, DimPid dimPid)
        {
            if (id != dimPid.PidContent)
            {
                return BadRequest();
            }

            _context.Entry(dimPid).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DimPidExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DimPid
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DimPid>> PostDimPid(DimPid dimPid)
        {
            _context.DimPid.Add(dimPid);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DimPidExists(dimPid.PidContent))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetDimPid", new { id = dimPid.PidContent }, dimPid);
        }

        // DELETE: api/DimPid/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DimPid>> DeleteDimPid(string id)
        {
            var dimPid = await _context.DimPid.FindAsync(id);
            if (dimPid == null)
            {
                return NotFound();
            }

            _context.DimPid.Remove(dimPid);
            await _context.SaveChangesAsync();

            return dimPid;
        }

        private bool DimPidExists(string id)
        {
            return _context.DimPid.Any(e => e.PidContent == id);
        }
    }
}
