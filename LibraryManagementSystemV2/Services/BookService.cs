using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;

namespace LibraryManagementSystemV2.Services
{

    public interface IBookService
    {
        public abstract Task<Book?> GetRawBookOnId(long bookId);
    }
    public class BookService : IBookService
    {

        private readonly SQLiteContext _bookContext;
        public BookService(SQLiteContext bookContext)
        {
            _bookContext = bookContext;
        }

        public async Task<Book?> GetRawBookOnId(long bookId)
        {
            return await _bookContext.Books.FindAsync(bookId);
        }
    }
}
