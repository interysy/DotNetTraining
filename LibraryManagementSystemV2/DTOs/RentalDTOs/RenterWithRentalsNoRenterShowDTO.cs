using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RenterWithRentalsNoRenterShowDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int RentalCount { get; set; }

        public ICollection<RentalShowWithNoRenterDTO>? Rentals { get; set; }

        public static RenterWithRentalsNoRenterShowDTO RenterWithRentalsToRenterWithRentalsShowDTO(Renter renter, int count, ICollection<RentalShowWithNoRenterDTO> rentals)
        {
            return new RenterWithRentalsNoRenterShowDTO
            {
                FirstName = renter.Entity.FirstName,
                LastName = renter.Entity.LastName,
                RentalCount = count,
                Rentals = rentals
            };
        }

    }
}