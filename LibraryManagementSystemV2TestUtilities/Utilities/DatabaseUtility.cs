using LibraryManagementSystemV2.Contexts;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class DatabaseUtilities
    {

        public static void RefreshDatabase(LibraryManagementContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        } 

        
    }
}
