using LibraryManagementSystemV2.DTOs.EntityDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.AuthorDTOs;

public class AuthorCreateDTO
{
    public virtual required EntityCreateDTO Entity { get; set; }
}
