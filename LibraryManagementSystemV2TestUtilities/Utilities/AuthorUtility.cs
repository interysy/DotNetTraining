using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2IntegrationTests.Utilities
{
    public class AuthorUtility : IDataUtility<Author>
    {
        internal static readonly List<Author> testAuthors =
        [
             new Author { Id = 1, Entity = new Entity { Id = 1, FirstName = "F. Scott", LastName = "Fitzgerald" } },
             new Author { Id = 2, Entity = new Entity { Id = 2, FirstName = "George", LastName = "Orwell" } },
             new Author { Id = 3, Entity = new Entity { Id = 3, FirstName = "Harper", LastName = "Lee" } },
             new Author { Id = 4, Entity = new Entity { Id = 4, FirstName = "Terry", LastName = "Pratchett" } },
             new Author { Id = 5, Entity = new Entity { Id = 5, FirstName = "Neil", LastName = "Gaiman" } },
             new Author { Id = 6, Entity = new Entity { Id = 6, FirstName = "Jane", LastName = "Austen" } }
        ];

        public static async Task<Author?> GetAdded(LibraryManagementContext context, long id)
        {
          

          return await context.Authors
                       .Include(author => author.Entity)
                        .FirstOrDefaultAsync(author => author.Id == id);
            
        }

        public async Task<List<Author>> GetAll(LibraryManagementContext context)
        {
            return await context.Authors.Include(author => author.Entity).ToListAsync(); 
        } 

        public async Task SeedData(LibraryManagementContext context)
        {
           context.Authors.AddRange(testAuthors);
           await context.SaveChangesAsync();
        }
    }
}
