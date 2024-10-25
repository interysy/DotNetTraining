using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class RentalContext : DbContext
    {
        public RentalContext(DbContextOptions<RentalContext> options)
            : base(options)
        { }

        public DbSet<Rental> Rentals { get; set; } = null!;
    }
}
