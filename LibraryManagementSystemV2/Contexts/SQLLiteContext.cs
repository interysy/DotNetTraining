using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class SQLiteContext : LibraryManagementContext
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
            optionsBuilder.UseSqlite(Configuration.GetConnectionString(ConnectionStringProperty));
        }
    }
}
