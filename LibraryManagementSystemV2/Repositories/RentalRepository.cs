using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Types;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LibraryManagementSystemV2.Repositories
{
    public class RentalRepository : GenericRepository<Rental>
    {
        public RentalRepository(SQLiteContext context) : base(context)
        {
        }

        public new async Task<List<Rental>> GetAllAsync(bool tracked = true)
        {
            var rentals = await _dbSet
                       .Include(rental => rental.Book)
                       .Include(rental => rental.Renter)
                       .ThenInclude(renter => renter.Entity)
                       .ToListAsync();

            return rentals;
        }

        public async Task<List<Rental>> GetOverdueBooksAsync() {

            DateTime rightNow = DateTime.UtcNow;

            List<Rental> overdueRentals = await _dbSet
                                                .Where(rental => !rental.returned && rental.ReturnDate < rightNow)
                                                .Include(rental => rental.Book)
                                                .Include(rental => rental.Renter)
                                                .ThenInclude(renter => renter.Entity)
                                                .OrderBy(renter => renter.ReturnDate)
                                                .ToListAsync();

            return overdueRentals;
        }


        public async Task<RenterWithCount?> GetRenterWithMostRentalsAsync()
        {
            var renter = await _dbSet
                .GroupBy(rental => rental.RenterId)
                .Select(group => new RenterWithCount { RenterId = group.Key, Count = group.Count() })
                .OrderByDescending(group => group.Count)
                .FirstOrDefaultAsync();


            return renter;

        } 
    }
}
