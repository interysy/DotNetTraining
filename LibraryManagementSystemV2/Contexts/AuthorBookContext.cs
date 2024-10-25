using Microsoft.EntityFrameworkCore; 
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Contexts
{
    public class AuthorBookContext : SQLiteContext
    {
        public AuthorBookContext(IConfiguration configuration) : base(configuration)
        {
        }

        //public AuthorBookContext(DbContextOptions<AuthorBookContext> options)
        //    : base(options)
        //{
        //}

        public DbSet<AuthorBook> AuthorBooks { get; set; } = null!;

        public DbSet<Book> Books { get; set; } = null!;


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuthorBook>().HasKey(ab => new { ab.AuthorId, ab.BookId });
        }
    }
}
