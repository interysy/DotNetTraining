using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.NewFolder1
{
    public class BookUpdateDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required float Price { get; set; }

        public required int Quantity { get; set; }

        public required ICollection<long> AuthorIDs { get; set; }

        public ICollection<AuthorCreateDTO>? NewAuthors { get; set; }

        public static Book BookUpdateDTOToBook(BookUpdateDTO bookDTO, long id)
        {

            var book = new Book
            {
                Id = id,
                Name = bookDTO.Name,
                Description = bookDTO.Description,
                Price = bookDTO.Price,
                Quantity = bookDTO.Quantity
            };

            return book;
        }
    }
}
