using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Constants;
using LibraryManagementSystemV2.DTOs.EntityDTOs;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntitiesController : ControllerBase
    {
        private readonly LibraryManagementContext _context;

        public EntitiesController(LibraryManagementContext context)
        {
            _context = context;
        }

        // GET: api/Entities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EntityShowDTO>>> GetEntities()
        {
            return await _context.Entities.Select(entity => addEntityType(entity)).ToListAsync();
        }

        // GET: api/Entities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EntityShowDTO>> GetEntity(long id)
        {
            var entity = await _context.Entities.FindAsync(id);

            if (entity == null)
            {
                return NotFound();
            }

            return addEntityType(entity);
        }

        // PUT: api/Entities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEntity(long id, EntityCreateWithTypeDTO entityDTO)
        {
            if (!EntityExists(id))
            {
                return BadRequest();
            }

            var entity = new Entity
            { 
                Id = id,
                FirstName = entityDTO.FirstName,
                LastName = entityDTO.LastName
            };

            _context.Entry(entity).State = EntityState.Modified;

            var entityCurrentType = getEntityType(entity);

            var entityNewType = entityDTO.type; 
            if (entityCurrentType != entityNewType) {
                if (entityCurrentType == EntityType.Author) {
                    var toDelete = await _context.Authors.FindAsync(id);
                    if (toDelete != null) {
                        _context.Authors.Remove(toDelete);
                    }
                    
                }
                if (entityCurrentType == EntityType.Renter) {
                    var toDelete = await _context.Renters.FindAsync(id);
                    if (toDelete != null)
                    {
                        _context.Renters.Remove(toDelete);
                    }
                }

                if (entityNewType == EntityType.Author) {
                    _context.Authors.Add(new Author { Entity = entity, Id = id }); 
                }

                if (entityNewType == EntityType.Renter)
                {
                    _context.Renters.Add(new Renter { Entity = entity, Id = id });
                }
            }

            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Entities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EntityDTO>> PostEntity(EntityCreateWithTypeDTO entityDTO)
        {
            var entity = new Entity
            {
                FirstName = entityDTO.FirstName,
                LastName = entityDTO.LastName
            };

            _context.Entities.Add(entity);
            await _context.SaveChangesAsync();

            if (entityDTO.type != null && entityDTO.type >= 2)  
            {
                return BadRequest();  
            }  
            else if (entityDTO.type == EntityType.Author)
            {

                var author = new Author
                {
                    Id = entity.Id,
                    Entity = entity
                };

                _context.Authors.Add(author);
            }
            else if (entityDTO.type == EntityType.Renter)
            {
                var renter = new Renter
                {
                    Id = entity.Id,
                    Entity = entity
                };

                _context.Renters.Add(renter);
            }

            await _context.SaveChangesAsync(); 

            
            return CreatedAtAction(nameof(GetEntity), new { id = entity.Id }, addEntityType(entity));
        }

        // DELETE: api/Entities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(long id)
        {
            var entity = await _context.Entities.FindAsync(id);
            var author = await _context.Authors.FindAsync(id);
            var renter = await _context.Renters.FindAsync(id); 

            if (entity == null)
            {
                return NotFound();
            }
          
            _context.Entities.Remove(entity);
            

            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            if (renter != null)  
            {
                _context.Renters.Remove(renter);  
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EntityExists(long id)
        {
            return _context.Entities.Any(e => e.Id == id);
        }


        private static EntityDTO EntityToDTO(Entity entity) =>
          new EntityDTO
            {
               Id = entity.Id,
               FirstName = entity.FirstName,
               LastName = entity.LastName
           };
        

        private EntityShowDTO addEntityType(Entity entity)
        {
            string type = string.Empty;
            if (_context.Authors.Any(a => a.Id == entity.Id))
            {
                EntityType.Types.TryGetValue(EntityType.Author, out type);
            }
            else if (_context.Renters.Any(r => r.Id == entity.Id))
            {
                EntityType.Types.TryGetValue(EntityType.Renter, out type);
            }
            else
            {
                EntityType.Types.TryGetValue(EntityType.None, out type);
            }
            return EntityShowDTO.EntityToEntityShowDTO(entity, type ?? string.Empty);
        }

        private int getEntityType(Entity entity) {

            if (_context.Authors.Any(a => a.Id == entity.Id))
            {
                return EntityType.Author;
            }
            else if (_context.Renters.Any(r => r.Id == entity.Id))
            {
                return EntityType.Renter;
            }
            else
            {
                return EntityType.None;
            }

        }
    }
}
