using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.RentalDTOs
{
    public class RenterWithRentalsShowDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required int RentalCount { get; set; } 
         
        public ICollection<RentalShowDTO>? Rentals { get; set; }  

        public static RenterWithRentalsShowDTO RenterWithRentalsToRenterWithRentalsShowDTO(Renter renter, int count, ICollection<RentalShowDTO> rentals)
        {
            return new RenterWithRentalsShowDTO
            {
                FirstName = renter.Entity.FirstName,
                LastName = renter.Entity.LastName,
                RentalCount = count,
                Rentals = rentals
            };
        }

    }
}
