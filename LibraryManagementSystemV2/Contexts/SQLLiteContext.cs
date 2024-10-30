using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class SQLiteContext : DbContext
    {

        protected readonly IConfiguration Configuration;  
        private readonly string ConnectionStringProperty = "DevDatabase";

        public SQLiteContext() { }
        public SQLiteContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite(Configuration.GetConnectionString(ConnectionStringProperty));
            }
        }

        public DbSet<Rental> Rentals { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Entity> Entities { get; set; } = null!;
        public DbSet<AuthorBook> AuthorBooks { get; set; } = null!;
        public DbSet<Renter> Renters { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;
    }
}
