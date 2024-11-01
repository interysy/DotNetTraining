using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace LibraryManagementSystemV2.Contexts
{
    public class LibraryManagementContext : DbContext
    {

        protected readonly IConfiguration Configuration;  
        private readonly string ConnectionStringProperty = "ConnectionString:DevDatabase";

        public LibraryManagementContext(IConfiguration configuration, DbContextOptions<LibraryManagementContext>? options = null) : base(options ?? new DbContextOptions<LibraryManagementContext>())
        { 

            Configuration = configuration;
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            { 
                optionsBuilder.UseSqlite(Configuration[ConnectionStringProperty]);
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
