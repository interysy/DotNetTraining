using LibraryManagementSystemV2.Constants;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.EntityDTOs;

public class EntityShowDTO
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public string? Type { get; set; } 


    public static EntityShowDTO EntityToEntityShowDTO(Entity entity, string type) =>
        new EntityShowDTO
        {
            Id = entity.Id,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Type = type
        }; 

}
