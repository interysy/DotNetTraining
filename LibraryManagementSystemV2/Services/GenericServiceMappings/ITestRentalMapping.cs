using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Services.GenericServiceMappings
{
    public interface ITestRentalMapping : IGenericService<Rental, RentalShowDTO, RentalCreateWithDaysDTO, RentalCreateWithDaysDTO>
    {

        public abstract Task<IEnumerable<RentalShowDTO>> GetOverdueRentals();

        public abstract Task<RenterWithRentalsShowDTO> GetRenterWithMostRentals();

        public abstract Task<RenterWithRentalsNoRenterShowDTO> GetRenterWithRentals(long renterId);
    }
}
