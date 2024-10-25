using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs;
using System.Net;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using LibraryManagementSystemV2.Services;
using System.Drawing.Text;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly SQLiteContext _context;
        private readonly IBookAuthorService _bookAuthorService;
        private readonly IAuthorService _authorService;

        public BooksController(SQLiteContext context, IBookAuthorService bookAuthorService, IAuthorService authorService)
        {
            _context = context; 
            _bookAuthorService = bookAuthorService;
            _authorService = authorService;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookShowDTO>>> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            var bookDTOs = new List<BookShowDTO>(); 
       
            foreach (var book in books)
            {
                var bookAuthors = (await _bookAuthorService.GetBookAuthors(book.Id));
                bookDTOs.Add(BookShowDTO.BookToBookShowDTO(book, bookAuthors));
            }

            return Ok(bookDTOs);
        }

        //[HttpGet("search")]
        //public ActionResult<IEnumerable<BookShowDTO>> SearchForBook(string searchTerm)
        //{
        //    searchTerm = searchTerm.ToLower();
        //    var authorBooks = _context.AuthorBooks
        //                        .Include(authorBook => authorBook.Author)
        //                        .Include(authorBook => authorBook.Book)
        //                        .Where(authorBook => authorBook.Book.Name.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
        //                                             || authorBook.Book.Price.ToString().Contains(searchTerm)
        //                                             || (authorBook.Book.Description != null && authorBook.Book.Description.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
        //                                             || authorBook.Author.Entity.FirstName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase)
        //                                             || authorBook.Author.Entity.LastName.Contains(searchTerm, StringComparison.CurrentCultureIgnoreCase))
        //                        .AsEnumerable()
        //                        .GroupBy(authorBook => authorBook.BookId)
        //                        .ToList();

        //    return new List<BookShowDTO>();
        //}


        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookShowDTO>> GetBook(long id)
        {
            Book? book = _context.Books.Find(id);

            if (book == null)
            {
                return NotFound();
            } 

            var bookAuthors = (await _bookAuthorService.GetBookAuthors(book.Id));

            return BookShowDTO.BookToBookShowDTO(book, bookAuthors);
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public IActionResult PutBook(long id, BookUpdateDTO bookDTO)
        {

            if (!BookExists(id))
            {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Book bookUpdated = BookUpdateDTO.BookUpdateDTOToBook(bookDTO, id);
                    _context.Entry(bookUpdated).State = EntityState.Modified;


                    IEnumerable<AuthorBook> bookAuthors = _context.AuthorBooks.Where(authorBook => authorBook.BookId == id).Include(authorBook => authorBook.Author).Include(authorBook => authorBook.Book);
                    IEnumerable<AuthorBook> bookAuthorsToRemove = bookAuthors.Where(authorBook => !bookDTO.AuthorIDs.Contains(authorBook.AuthorId));
                    IEnumerable<long> bookAuthorIdsToAdd = bookDTO.AuthorIDs.Where(authorId => !bookAuthors.Select(bookAuthor => bookAuthor.AuthorId).Contains(authorId));


                    _context.AuthorBooks.RemoveRange(bookAuthorsToRemove);


                    foreach (var authorId in bookAuthorIdsToAdd)
                    {
                        var author = _context.Authors.Find(authorId);
                        if (author == null)
                        {
                            return NotFound($"Author with ID {authorId} not found.");
                        }

                        AuthorBook authorBook = AuthorBook.AuthorAndBookToAuthorBook(author, bookUpdated);
                        _context.AuthorBooks.Add(authorBook);
                    }

                    if (bookDTO.NewAuthors == null)
                    {
                        _context.SaveChanges();
                        return NoContent();
                    }


                    _bookAuthorService.CreateAuthorsForBookFromCreateDTO(bookDTO.NewAuthors, bookUpdated);


                    _context.SaveChanges();
                    transaction.Commit();


                    return NoContent();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BookShowDTO>> PostBook(BookCreateDTO bookDTO)
        {
            var book = BookCreateDTO.BookCreateDTOToBook(bookDTO);

            var authors = _context.Authors.Include(author => author.Entity).Where(author => bookDTO.AuthorIDs.Contains(author.Id)).ToList(); 

            _context.Books.Add(book);
            await _context.SaveChangesAsync();


            if (bookDTO.NewAuthors != null) 
            {
                foreach (var newAuthor in bookDTO.NewAuthors ?? new List<AuthorCreateDTO>())
                {
                    var entity = new Entity
                    {
                        FirstName = newAuthor.Entity.FirstName,
                        LastName = newAuthor.Entity.LastName
                    };


                    _context.Entities.Add(entity);
                    await _context.SaveChangesAsync();


                    var author = new Author
                    {
                        Id = entity.Id,
                        Entity = entity
                    };

                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();

                    authors.Add(author);
                }

            }
         

            foreach (var author in authors)
            {

                var authorBook = new AuthorBook
                {
                    AuthorId = author.Id,
                    BookId = book.Id,
                    Author = author,
                    Book = book
                };
                _context.AuthorBooks.Add(authorBook); 
            }

            await _context.SaveChangesAsync();

            var bookToShow = new BookShowDTO
            {
                Name = book.Name,
                Description = book.Description,
                Quantity = book.Quantity,
                Price = book.Price,
                Authors = authors.Select(author => AuthorShowDTO.AuthorToAuthorShowDTO(author)).ToList()
            };


            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookToShow);
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(long id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);

            var authorBooksToRemove = _context.AuthorBooks.Where(authorBook => authorBook.BookId == id);
            _context.AuthorBooks.RemoveRange(authorBooksToRemove);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BookExists(long id)
        {
            return _context.Books.Any(e => e.Id == id);
        }

        private async Task<BookShowDTO> BookToDTO(Book book)
        {
            var bookAuthors = await GetBookAuthors(book.Id);

            return new BookShowDTO
            {
                Id = book.Id,
                Name = book.Name,
                Description = book.Description,
                Price = book.Price,
                Quantity = book.Quantity,
                Authors = bookAuthors.Select(bookAuthor => AuthorShowDTO.AuthorToAuthorShowDTO(bookAuthor)).ToList()
            };
        }

        private async void addNewAuthorsToBook(long bookId, ICollection<AuthorCreateDTO> newAuthors) { 

                foreach (var newAuthor in newAuthors)
                {
                    var entity = new Entity
                    {
                        FirstName = newAuthor.Entity.FirstName,
                        LastName = newAuthor.Entity.LastName
                    };


                    _context.Entities.Add(entity);
                    await _context.SaveChangesAsync();


                    var author = new Author
                    {
                        Id = entity.Id,
                        Entity = entity
                    };

                    _context.Authors.Add(author);
                    await _context.SaveChangesAsync();


                    var book = await _context.Books.FindAsync(bookId);
                    var authorBook = new AuthorBook
                    {
                        AuthorId = author.Id,
                        BookId = bookId,
                        Author = author,
                        Book = book
                    };
                    _context.AuthorBooks.Add(authorBook);
                }


            await _context.SaveChangesAsync();

        }
         
        private async Task<IEnumerable<Author>> GetBookAuthors(long bookId)
        {
            var authorBooks = await _context.AuthorBooks
                                            .Include(ab => ab.Author) 
                                            .ThenInclude(author => author.Entity)
                                            .Include(ab => ab.Book)
                                            .Where(ab => ab.BookId == bookId)
                                            .ToListAsync();

            return authorBooks.Select(ab => ab.Author); ;
        }
    }
}
