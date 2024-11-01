using LibraryManagementSystemV2.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;

namespace LibraryManagementSystemV2IntegrationTests
{
    public class IntegrationTestFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<LibraryManagementContext>));

                services.Remove(dbContextDescriptor);

                var dbConnectionDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbConnection));

                services.Remove(dbConnectionDescriptor);

                services.AddSingleton<DbConnection>(container =>
                {
                    var connection = new SqliteConnection("DataSource=:memory:");
                    connection.Open();

                    return connection;
                });

                services.AddDbContext<LibraryManagementContext>((container, options) =>
                {
                    var connection = container.GetRequiredService<DbConnection>();
                    options.UseSqlite(connection);
                    options.EnableSensitiveDataLogging();
                });
                //services.AddSingleton<LibraryManagementContext>(container =>
                //{
                //    var options = new DbContextOptionsBuilder<LibraryManagementContext>()
                //        .UseSqlite(container.GetRequiredService<DbConnection>())
                //        .EnableSensitiveDataLogging()
                //        .Options;
                //    return new LibraryManagementContext(container.GetRequiredService<IConfiguration>(), options);
                //});
                //var options = new DbContextOptionsBuilder<LibraryManagementContext>()
                //        .UseInMemoryDatabase(databaseName: "TestDb")
                //        .Options;
                //services.AddSingleton(x => new LibraryManagementContext(new ConfigurationBuilder().Build(), options));
            });

            builder.UseEnvironment("Development");
        }
    }
}