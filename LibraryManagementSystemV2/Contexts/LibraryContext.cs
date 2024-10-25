using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class LibraryContext : DbContext    {
        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;

        public DbSet<AuthorBook> AuthorBooks { get; set; } = null!;

        public DbSet<Author> Authors { get; set; } = null!;

        public DbSet<Entity> Entities { get; set; } = null!;

    }
}
