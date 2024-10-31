using LibraryManagementSystemV2.Contexts;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class DatabaseUtilities
    {

        public static void RefreshDatabase(LibraryManagementContext context)
        {
            //using (var scope = factory.Services.CreateScope())
            //{
            //    var scopedServices = scope.ServiceProvider;
            //    var db = scopedServices.GetRequiredService<LibraryManagementContext>();

            //    db.Database.EnsureDeleted();
            //    db.Database.EnsureCreated();
            //}  

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

        } 

        
    }
}
