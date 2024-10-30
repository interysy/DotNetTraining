using LibraryManagementSystemV2.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class CreateDatabaseUtilities
    {

        public static void CreateAndSeedDatabase(WebApplicationFactory<Program> factory)
        {
            using (var scope = factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SQLiteContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

            }
        }
    }
}
