using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs.RenterDTOs;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentersController : ControllerBase
    {
        private readonly LibraryManagementContext _context;

        public RentersController(LibraryManagementContext context)
        {
            _context = context;
        }

        // GET: api/Renters
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RenterShowDTO>>> GetRenters()
        {
            return await _context.Renters.Include(renter => renter.Entity).Select(renter => RenterShowDTO.RenterToRenterShowDTO(renter)).ToListAsync();
        }

        // GET: api/Renters/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RenterShowDTO>> GetRenter(long id)
        {
            var renter = await _context.Renters.Include(renter => renter.Entity).FirstOrDefaultAsync(a => a.Id == id);

            if (renter == null)
            {
                return NotFound();
            }

            return RenterShowDTO.RenterToRenterShowDTO(renter);
        }


        // POST: api/Renters
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RenterShowDTO>> PostRenter(RenterCreateDTO renterDTO)
        {

            var entity = new Entity
            {
                FirstName = renterDTO.Entity.FirstName,
                LastName = renterDTO.Entity.LastName
            };

            _context.Entities.Add(entity);

            await _context.SaveChangesAsync();

            var renter = new Renter
            {
                Id = entity.Id,
                Entity = entity
            };

            _context.Renters.Add(renter); 

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRenter), new { id = renter.Id }, RenterShowDTO.RenterToRenterShowDTO(renter));
        }

        // DELETE: api/Renters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRenter(long id)
        {
            var renter = await _context.Renters.FindAsync(id);
            if (renter == null)
            {
                return NotFound();
            }

            _context.Renters.Remove(renter);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RenterExists(long id)
        {
            return _context.Renters.Any(e => e.Id == id);
        }
    }
}
