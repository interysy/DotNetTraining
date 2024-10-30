using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Services.Interfaces;

namespace LibraryManagementSystemV2.Services.GenericServiceMappings
{
    public interface ITestRentalMapping : IReadService<Rental, RentalShowDTO>
    {

        public abstract Task<IEnumerable<RentalShowDTO>> GetOverdueRentals();

        public abstract Task<RenterWithRentalsShowDTO> GetRenterWithMostRentals();

        public abstract Task<RenterWithRentalsNoRenterShowDTO> GetRenterWithRentals(long renterId);
    }
}
