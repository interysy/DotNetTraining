using LibraryManagementSystemV2.DTOs.EntityDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using LibraryManagementSystemV2.Migrations;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RentalShowDTO
    {
        public Guid Id { get; set; }
        public required DateTime RentalDate { get; set; }
        public required DateTime ReturnDate { get; set; }
        public required BookShowDTO Book { get; set; }
        public required RenterShowDTO Renter { get; set; }
        public required bool returned { get; set; }

        public static RentalShowDTO RentalToRentalShowDTO(Rental rental, BookShowDTO book, RenterShowDTO renter)
        {
            return new RentalShowDTO
            {
                Id = rental.Id,
                RentalDate = rental.RentalDate,
                ReturnDate = rental.ReturnDate,
                Book = book,
                Renter = renter,
                returned = rental.returned 
            };
        }
    }
}
