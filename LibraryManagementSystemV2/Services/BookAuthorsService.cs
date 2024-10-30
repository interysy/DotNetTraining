using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Services;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Services 
{

    public interface IBookAuthorService
    {
        public abstract Task<IEnumerable<Author>> GetBookAuthors(long bookId);
        public abstract Task<IEnumerable<Book>> GetAuthorBooks(long authorId);
        public IEnumerable<AuthorBook> CreateAuthorsForBookFromCreateDTO(IEnumerable<AuthorCreateDTO> authors, Book book);
    }
 

public class BookAuthorsService : IBookAuthorService
    {

        private readonly LibraryManagementContext _libraryContext;


        public BookAuthorsService() { }

        public BookAuthorsService(LibraryManagementContext libraryContext)
        {
            _libraryContext = libraryContext;
        }

        public async Task<IEnumerable<Author>> GetBookAuthors(long bookId)
        {
            var authorBooks = await _libraryContext.AuthorBooks
                                    .Include(ab => ab.Author)
                                    .ThenInclude(author => author.Entity)
                                    .Include(ab => ab.Book)
                                    .Where(ab => ab.BookId == bookId)
                                    .ToListAsync();

            return authorBooks.Select(ab => ab.Author);
        }

        public async Task<IEnumerable<Book>> GetAuthorBooks(long authorId)
        {
            IEnumerable<Book> booksForAuthor = await _libraryContext.AuthorBooks
                                       .Where(bookAuthor => bookAuthor.AuthorId == authorId) 
                                       .Include(bookAuthor => bookAuthor.Book)
                                       .Select(bookAuthor => bookAuthor.Book)
                                       .ToListAsync();

            return booksForAuthor;
        }

        public IEnumerable<AuthorBook> CreateAuthorsForBookFromCreateDTO(IEnumerable<AuthorCreateDTO> authors, Book book)
        {

            IEnumerable<AuthorBook> resultantAuthors = [];

            foreach (var newAuthor in authors)
            {
                Entity entity = Entity.CreateEntity(newAuthor.Entity.FirstName, newAuthor.Entity.LastName);
                _libraryContext.Entities.Add(entity);


                Author author = Author.CreateAuthorFromEntity(entity);
                _libraryContext.Authors.Add(author);

                AuthorBook authorBook = AuthorBook.AuthorAndBookToAuthorBook(author, book);
                _libraryContext.AuthorBooks.Add(authorBook);
                resultantAuthors.Append(authorBook);
            }

            return resultantAuthors;
        }

    }
}
