using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.LibraryStatisticsDTOs
{
    public class OverdueShowDTO : RentalShowDTO
    {

        public required long BookId { get; set; }
        public required string Name { get; set; }

        public required int OverdueDays { get; set; }

        public static OverdueShowDTO RentalToOverdueShowDTO(RentalShowDTO rental)
        {

            return new OverdueShowDTO
            {
                Id = rental.Id,
                BookId = rental.Book.Id,
                Name = rental.Book.Name,
                OverdueDays = (DateTime.UtcNow - rental.ReturnDate).Days,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                Book = rental.Book,
                Renter = rental.Renter,
                returned = rental.returned
            };
        }

    }
}
