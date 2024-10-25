using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class LibraryManagementContext : DbContext
    {
        public DbSet<Rental> Rentals { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Entity> Entities { get; set; } = null!;
        public DbSet<AuthorBook> AuthorBooks { get; set; } = null!;
        public DbSet<Renter> Renters { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
    }
}
