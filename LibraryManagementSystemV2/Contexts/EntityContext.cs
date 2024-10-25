using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class EntityContext : SQLiteContext
    {
        public EntityContext(IConfiguration configuration) : base(configuration)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entity>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();
        }


        public DbSet<Entity> Entities { get; set; } = null!;
        public DbSet<Author> Authors { get; set; } = null!;

        public DbSet<Renter> Renters { get; set; } = null!;
    }
}
