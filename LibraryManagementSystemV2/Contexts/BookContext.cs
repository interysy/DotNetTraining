using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Contexts
{
    public class BookContext : SQLiteContext
    {
        public BookContext(IConfiguration configuration) : base(configuration)
        {
        }
    }
}
 

