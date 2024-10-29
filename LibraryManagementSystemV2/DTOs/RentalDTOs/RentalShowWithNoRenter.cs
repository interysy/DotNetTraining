using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RentalShowWithNoRenterDTO
    {
        public Guid Id { get; set; }
        public required DateTime RentalDate { get; set; }
        public required DateTime ReturnDate { get; set; }
        public required BookShowDTO Book { get; set; }
        public required bool returned { get; set; }

        public static RentalShowWithNoRenterDTO RentalToRentalShowWithNoRenterDTO(Rental rental, BookShowDTO book)
        {
            return new RentalShowWithNoRenterDTO
            {
                Id = rental.Id,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                Book = book,
                returned = rental.returned
            };
        }
    }
}
