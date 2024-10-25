using Microsoft.AspNetCore.Mvc;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Services;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorBooksController : ControllerBase
    {
        private readonly SQLiteContext _context; 
        private readonly IBookAuthorService _bookAuthorService;

        public AuthorBooksController(SQLiteContext context, IBookAuthorService bookAuthorService)
        {
            _context = context; 
            _bookAuthorService = bookAuthorService;
        }

        // GET: api/AuthorBooks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookShowDTO>>> GetAuthorBooks(long authorId)
        {

            IEnumerable<Book> authorBooks = await _bookAuthorService.GetAuthorBooks(authorId); 

            var bookShowDTOsTasks = authorBooks.Select(async book =>
            {
                var authors = await _bookAuthorService.GetBookAuthors(book.Id);
                return BookShowDTO.BookToBookShowDTO(book, authors);
            });

            var bookShowDTOs = await Task.WhenAll(bookShowDTOsTasks);


            return Ok(bookShowDTOs);
        }


        private bool AuthorBookExists(long id)
        {
            return _context.AuthorBooks.Any(e => e.AuthorId == id);
        }
    }
}
