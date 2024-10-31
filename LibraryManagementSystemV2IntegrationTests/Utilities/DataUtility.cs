using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public interface IDataUtility<T> where T: class
    {
        public abstract Task SeedData(WebApplicationFactory<Program> factory);

        public static abstract Task<T?> GetAdded(LibraryManagementContext context, long id);
    }
}
