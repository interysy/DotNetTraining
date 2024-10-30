using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Types;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Repositories.Interfaces
{
    public interface IRentalRepository
    {
        public abstract Task<List<Rental>> GetAllAsync(bool tracked = true);
        public abstract Task<List<Rental>> GetOverdueBooksAsync();
        public abstract Task<RenterWithCount?> GetRenterWithMostRentalsAsync();
    }
}
