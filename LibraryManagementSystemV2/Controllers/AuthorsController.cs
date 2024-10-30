using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorsController : ControllerBase
    {
        private readonly LibraryManagementContext _context;

        public AuthorsController(LibraryManagementContext context)
        {
            _context = context;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorShowDTO>>> GetAuthors()
        {
            return await _context.Authors 
                         .Include(author => author.Entity) 
                         .Select(author => AuthorShowDTO.AuthorToAuthorShowDTO(author)) 
                         .ToListAsync();  
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorShowDTO>> GetAuthor(long id)
        {
            Author? author = await _context.Authors 
                            .Include(author => author.Entity) 
                            .FirstOrDefaultAsync(author => author.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            return AuthorShowDTO.AuthorToAuthorShowDTO(author);
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public ActionResult<AuthorShowDTO> PostAuthor(AuthorCreateDTO authorCreateDTO)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Entity entity = Entity.CreateEntity(authorCreateDTO.Entity.FirstName, authorCreateDTO.Entity.LastName);
                    Author author = Author.CreateAuthorFromEntity(entity);

                    _context.Entities.Add(entity);
                    _context.Authors.Add(author);

                    _context.SaveChanges();
                    transaction.Commit();
                    return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, AuthorShowDTO.AuthorToAuthorShowDTO(author));
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(long id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            _context.Authors.Remove(author);
            _context.SaveChanges();

            return NoContent();
        }

        private bool AuthorExists(long id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
