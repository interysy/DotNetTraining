using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class AuthorBookUtility : IDataUtility<AuthorBook>
    { 

        internal List<AuthorBook>? authorBooks;

        public static Task<AuthorBook?> GetAdded(LibraryManagementContext context, long id)
        {
            throw new NotImplementedException();
        }

        public async Task SeedData(LibraryManagementContext context)
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

            context.AuthorBooks.AddRange(authorBooks);
            await context.SaveChangesAsync();
        }
    }
}
