using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class AuthorBookUtility : IDataUtility<AuthorBook>
    { 

        internal List<AuthorBook>? authorBooks;
        public static async Task<AuthorBook?> GetAdded(WebApplicationFactory<Program> factory, long id)
        {
            using (var scope = factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<LibraryManagementContext>();
                return await db.AuthorBooks.FindAsync(id);
            }
        }

        public static Task<AuthorBook?> GetAdded(LibraryManagementContext context, long id)
        {
            throw new NotImplementedException();
        }

        public Task SeedData(WebApplicationFactory<Program> factory)
        {
            throw new NotImplementedException();
        }

        //public async Task SeedData(WebApplicationFactory<Program> factory)
        //{
        //    List<AuthorBook> authorBooks = new List<AuthorBook>
        //            {
        //                 new AuthorBook { AuthorId = 1, BookId = 1, Author = await AuthorUtility.GetAdded(factory, 1), Book = await BookUtility.GetAdded(factory, 1) },
        //                 new AuthorBook { AuthorId = 2, BookId = 2, Author = await AuthorUtility.GetAdded(factory, 2), Book = await BookUtility.GetAdded(factory, 2) },
        //                 new AuthorBook { AuthorId = 3, BookId = 3, Author = await AuthorUtility.GetAdded(factory, 3), Book = await BookUtility.GetAdded(factory, 3) },
        //                 new AuthorBook { AuthorId = 4, BookId = 4, Author = await AuthorUtility.GetAdded(factory, 4), Book = await BookUtility.GetAdded(factory, 4) },
        //                 new AuthorBook { AuthorId = 5, BookId = 5, Author = await AuthorUtility.GetAdded(factory, 5), Book = await BookUtility.GetAdded(factory, 5) },
        //                 new AuthorBook { AuthorId = 6, BookId = 5, Author = await AuthorUtility.GetAdded(factory, 6), Book = await BookUtility.GetAdded(factory, 5) }
        //            };

        //    using (var scope = factory.Services.CreateScope())
        //    {
        //        var scopedServices = scope.ServiceProvider;
        //        var db = scopedServices.GetRequiredService<LibraryManagementContext>();

        //        db.ChangeTracker.Clear();
        //        await db.Database.EnsureCreatedAsync();
        //        db.AuthorBooks.AddRange(authorBooks);
        //        await db.SaveChangesAsync();
        //    }
        //} 

        public async Task SeedDataContext(LibraryManagementContext context)
        {
            authorBooks = new List<AuthorBook>
                    {
                         new AuthorBook { AuthorId = 1, BookId = 1, Author = await AuthorUtility.GetAdded(context, 1), Book = await BookUtility.GetAdded(context, 1) },
                         new AuthorBook { AuthorId = 2, BookId = 2, Author = await AuthorUtility.GetAdded(context, 2), Book = await BookUtility.GetAdded(context, 2) },
                         new AuthorBook { AuthorId = 3, BookId = 3, Author = await AuthorUtility.GetAdded(context, 3), Book = await BookUtility.GetAdded(context, 3) },
                         new AuthorBook { AuthorId = 4, BookId = 4, Author = await AuthorUtility.GetAdded(context, 4), Book = await BookUtility.GetAdded(context, 4) },
                         new AuthorBook { AuthorId = 5, BookId = 5, Author = await AuthorUtility.GetAdded(context, 5), Book = await BookUtility.GetAdded(context, 5) },
                         new AuthorBook { AuthorId = 6, BookId = 5, Author = await AuthorUtility.GetAdded(context, 6), Book = await BookUtility.GetAdded(context, 5) }
                    };

            
                //context.ChangeTracker.Clear();
                //await context.Database.EnsureCreatedAsync();
                context.AuthorBooks.AddRange(authorBooks);
                await context.SaveChangesAsync();
            
        }
    }
}
