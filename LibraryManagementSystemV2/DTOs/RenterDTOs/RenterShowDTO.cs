using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.DTOs.RenterDTOs
{
    public class RenterShowDTO
    {
        public long Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public static RenterShowDTO RenterToRenterShowDTO(Renter renter) {

            return new RenterShowDTO { FirstName = renter.Entity.FirstName, LastName = renter.Entity.LastName, Id = renter.Id };
        }
    }
}
