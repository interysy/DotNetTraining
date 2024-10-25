using LibraryManagementSystemV2.CustomExceptions.Books;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Services;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestBookController : ControllerBase
    {

        private readonly ITestBookMapping _service;

        public TestBookController(ITestBookMapping service)
        {
            _service = service;
        }

        [HttpGet("GetBooks")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookShowDTO>>> GetAllBooks()
        {
            var books = await _service.GetAllAsync();
            return Ok(books);
        }

        [HttpGet("GetBook/{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<BookShowDTO>> GetBookById(int id)
        {
            if (id < 1)
            {
                return BadRequest("Id must be greater than 0");
            }

            try
            {
                BookShowDTO bookDTO = await _service.GetByIdAsync(id);
            }
            catch (BookNotFoundException exception)
            {
                return NotFound(exception.Message);
            }

            return Ok(await _service.GetByIdAsync(id));
        }

        [HttpPost("CreateBook")]
        public async Task<ActionResult<BookShowDTO>> PostBook(BookCreateDTO bookDTO)
        {
            BookShowDTO bookShowDTO = await _service.AddAsync(bookDTO);
            return Ok(bookShowDTO);

        }
    }
}
