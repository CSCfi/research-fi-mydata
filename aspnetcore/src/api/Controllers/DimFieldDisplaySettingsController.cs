using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;

namespace api
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimFieldDisplaySettingsController : ControllerBase
    {
        private readonly TtvContext _context;

        public DimFieldDisplaySettingsController(TtvContext context)
        {
            _context = context;
        }

        // GET: api/DimFieldDisplaySettings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DimFieldDisplaySettings>>> GetDimFieldDisplaySettings()
        {
            return await _context.DimFieldDisplaySettings.ToListAsync();
        }

        // GET: api/DimFieldDisplaySettings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DimFieldDisplaySettings>> GetDimFieldDisplaySettings(int id)
        {
            var dimFieldDisplaySettings = await _context.DimFieldDisplaySettings.FindAsync(id);

            if (dimFieldDisplaySettings == null)
            {
                return NotFound();
            }

            return dimFieldDisplaySettings;
        }

        // PUT: api/DimFieldDisplaySettings/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDimFieldDisplaySettings(int id, DimFieldDisplaySettings dimFieldDisplaySettings)
        {
            if (id != dimFieldDisplaySettings.Id)
            {
                return BadRequest();
            }

            _context.Entry(dimFieldDisplaySettings).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DimFieldDisplaySettingsExists(id))
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

        // POST: api/DimFieldDisplaySettings
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DimFieldDisplaySettings>> PostDimFieldDisplaySettings(DimFieldDisplaySettings dimFieldDisplaySettings)
        {
            _context.DimFieldDisplaySettings.Add(dimFieldDisplaySettings);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDimFieldDisplaySettings", new { id = dimFieldDisplaySettings.Id }, dimFieldDisplaySettings);
        }

        // DELETE: api/DimFieldDisplaySettings/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DimFieldDisplaySettings>> DeleteDimFieldDisplaySettings(int id)
        {
            var dimFieldDisplaySettings = await _context.DimFieldDisplaySettings.FindAsync(id);
            if (dimFieldDisplaySettings == null)
            {
                return NotFound();
            }

            _context.DimFieldDisplaySettings.Remove(dimFieldDisplaySettings);
            await _context.SaveChangesAsync();

            return dimFieldDisplaySettings;
        }

        private bool DimFieldDisplaySettingsExists(int id)
        {
            return _context.DimFieldDisplaySettings.Any(e => e.Id == id);
        }
    }
}
