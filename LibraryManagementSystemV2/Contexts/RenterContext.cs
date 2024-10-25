using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Contexts
{
    public class RenterContext : SQLiteContext
    {
        public RenterContext(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
