using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Services
{

    public interface IRentalService
    {
        public abstract Task<bool> BookRentedBetweenDates(long bookId, DateTime startDate, DateTime endDate);
        public abstract Task<List<Rental>> CurrentlyRentedBooks();

        public abstract Task<List<Rental>> CurrentlyTakenOutBooks();
    }
    public class RentalService : IRentalService
    {

        private readonly SQLiteContext _context;
        public RentalService(SQLiteContext context)
        {
            _context = context;
        }

        public async Task<bool> BookRentedBetweenDates(long bookId, DateTime startDate, DateTime endDate)
        {
            List<Rental> rentals = await _context.Rentals
                .Where(rental => rental.BookId == bookId && rental.returned == false)
                .Where(rental => (rental.RentalDate >= startDate && rental.RentalDate <= endDate || rental.ReturnDate >= startDate && rental.ReturnDate <= endDate))
                .ToListAsync();


            return rentals.Count != 0;
        }

        public async Task<List<Rental>> CurrentlyRentedBooks()
        {
            DateTime today = DateTime.UtcNow;

            List<Rental> rentals = await _context.Rentals
                .Where(rental => !rental.returned)
                .ToListAsync();


            return rentals;
        }

        public async Task<List<Rental>> CurrentlyTakenOutBooks()
        {
            DateTime today = DateTime.UtcNow;

            List<Rental> rentals = await _context.Rentals
                .Where(rental => !rental.returned && today >= rental.RentalDate && today <= rental.ReturnDate)
                .ToListAsync();


            return rentals;
        }


    }
}
