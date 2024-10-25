

using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.AuthorDTOs;

public class AuthorShowDTO
{
    public long Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public static AuthorShowDTO AuthorToAuthorShowDTO(Author author)
    {
        return new AuthorShowDTO
        {
            Id = author.Id,
            FirstName = author.Entity.FirstName,
            LastName = author.Entity.LastName
        };
    }
}
