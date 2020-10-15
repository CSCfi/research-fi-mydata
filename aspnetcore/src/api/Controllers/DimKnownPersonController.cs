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
    public class DimKnownPersonController : ControllerBase
    {
        private readonly TtvContext _context;

        public DimKnownPersonController(TtvContext context)
        {
            _context = context;
        }

        // GET: api/DimKnownPerson
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DimKnownPerson>>> GetDimKnownPerson()
        {
            return await _context.DimKnownPerson.ToListAsync();
        }

        // GET: api/DimKnownPerson/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DimKnownPerson>> GetDimKnownPerson(int id)
        {
            var dimKnownPerson = await _context.DimKnownPerson.FindAsync(id);

            if (dimKnownPerson == null)
            {
                return NotFound();
            }

            return dimKnownPerson;
        }

        // PUT: api/DimKnownPerson/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDimKnownPerson(int id, DimKnownPerson dimKnownPerson)
        {
            if (id != dimKnownPerson.Id)
            {
                return BadRequest();
            }

            _context.Entry(dimKnownPerson).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DimKnownPersonExists(id))
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

        // POST: api/DimKnownPerson
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DimKnownPerson>> PostDimKnownPerson(DimKnownPerson dimKnownPerson)
        {
            _context.DimKnownPerson.Add(dimKnownPerson);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDimKnownPerson", new { id = dimKnownPerson.Id }, dimKnownPerson);
        }

        // DELETE: api/DimKnownPerson/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DimKnownPerson>> DeleteDimKnownPerson(int id)
        {
            var dimKnownPerson = await _context.DimKnownPerson.FindAsync(id);
            if (dimKnownPerson == null)
            {
                return NotFound();
            }

            _context.DimKnownPerson.Remove(dimKnownPerson);
            await _context.SaveChangesAsync();

            return dimKnownPerson;
        }

        private bool DimKnownPersonExists(int id)
        {
            return _context.DimKnownPerson.Any(e => e.Id == id);
        }
    }
}
