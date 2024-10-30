using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Services
{

    public interface IRenterService
    {
        public abstract Task<Renter?> GetRawRenterOnId(long renterId);
    }
    public class RenterService : IRenterService
    {

        private readonly LibraryManagementContext _renterContext;

        public RenterService(LibraryManagementContext renterContext)  
        {
            _renterContext = renterContext;
        }

        public async Task<Renter?> GetRawRenterOnId(long renterId)
        {
            return await _renterContext.Renters.Include(renter => renter.Entity).FirstOrDefaultAsync(renter => renter.Id == renterId);
        }


    }
}
