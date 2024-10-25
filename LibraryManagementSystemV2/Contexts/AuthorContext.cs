using Microsoft.EntityFrameworkCore;  
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Contexts
{
    public class AuthorContext : SQLiteContext
    {
        public AuthorContext(IConfiguration configuration) : base(configuration)
        {
        }

        public DbSet<Author> Authors { get; set; } = null!;

        public DbSet<Entity> Entities { get; set; } = null!;
    }
}
