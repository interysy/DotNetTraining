using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.NewFolder1
{
    public class BookShowDTO
    {
        public long Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required float Price { get; set; }  
         
        public required int Quantity { get; set; }

        public required ICollection<AuthorShowDTO> Authors { get; set;}

        public static BookShowDTO BookToBookShowDTO(Book book, IEnumerable<Author> authors)
        {

            var bookToShow = new BookShowDTO
            { 
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                Quantity = book.Quantity,
                Price = book.Price,
                Authors = authors.Select(author => AuthorShowDTO.AuthorToAuthorShowDTO(author)).ToList()
            };
            return bookToShow;
        }
    }
}
