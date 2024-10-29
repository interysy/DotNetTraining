using LibraryManagementSystemV2.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Repositories
{
    public class AuthorBookRepository : GenericRepository<AuthorBook>
    {
        public AuthorBookRepository(DbContext context) : base(context)
        {
        }  

        public async Task<IEnumerable<AuthorBook>> GetBookAuthors(long bookId)
        {
            IEnumerable<AuthorBook> authorBooks =  await _dbSet.Where(authorBook => authorBook.BookId == bookId).Include(authorBook => authorBook.Author).ThenInclude(author => author.Entity).Include(authorBook => authorBook.Book).ToListAsync();
            return authorBooks;
        }
    }
}
