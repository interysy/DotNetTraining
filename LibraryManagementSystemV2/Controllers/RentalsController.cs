using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.DTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Services;
using Microsoft.AspNetCore.Routing.Matching;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using LibraryManagementSystemV2.Migrations;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RentalsController : ControllerBase
    {
        private readonly SQLiteContext _context;
        private readonly IBookAuthorService _bookAuthorsService;
        private readonly IBookService _bookService;
        private readonly IRenterService _renterService;
        private readonly IRentalService _rentalService; 

        public RentalsController(SQLiteContext context, IBookAuthorService bookAuthorsService, IBookService bookService, IRenterService renterService, IRentalService rentalService)
        {
            _context = context;
            _bookAuthorsService = bookAuthorsService;
            _bookService = bookService;
            _renterService = renterService;
            _rentalService = rentalService;
        }


        // GET: api/Rentals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RentalShowDTO>>> GetRentals()
        {

            var rentals = await _context.Rentals
                        .Include(rental => rental.Book)
                        .Include(rental => rental.Renter)
                        .ThenInclude(renter => renter.Entity)
                        .ToListAsync();

            var rentalDTOs = new List<RentalShowDTO>();

            foreach (var rental in rentals)
            {
                var bookAuthors = await _bookAuthorsService.GetBookAuthors(rental.Book.Id);
                var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
                var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

                rentalDTOs.Add(RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO));
            }

            return Ok(rentalDTOs);
        }

        // GET: api/Rentals/ForBook
        [HttpGet("ForBook")]
        public async Task<ActionResult<IEnumerable<RentalShowDTO>>> GetRentalsForBook(long bookId)
        {

            var rentals = await _context.Rentals 
                        .Where(rental => rental.BookId == bookId)
                        .Include(rental => rental.Book)
                        .Include(rental => rental.Renter)
                        .ThenInclude(renter => renter.Entity) 
                        .OrderBy(renter => renter.RentalDate)
                        .ToListAsync();

            var rentalDTOs = new List<RentalShowDTO>();

              foreach (var rental in rentals)
            {
                var bookAuthors = await _bookAuthorsService.GetBookAuthors(rental.Book.Id);
                var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
                var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

                rentalDTOs.Add(RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO));
            }

            return Ok(rentalDTOs);
        }

        // GET: api/Rentals/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rental>> GetRental(Guid id)
        {
            var rental = await _context.Rentals
                                       .Include(r => r.Book)
                                       .Include(r => r.Renter)
                                       .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
            {
                return NotFound();
            }

            return rental;
        }

        // PUT: api/Rentals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRental(Guid id, RentalCreateWithStartDateDTO rentalDTO)
        {
            Rental? currentRental = await _context.Rentals.FindAsync(id);
            DateTime today = DateTime.UtcNow;

            if (currentRental == null)
            {
                return BadRequest($"Rental with {id} doesn't exist");
            }
            else if (currentRental.ReturnDate < today)
            {
                return BadRequest("Cannot update an expired rental");
            }

            Book? book = await _bookService.GetRawBookOnId(rentalDTO.BookId);
            if (book == null)
            {
                return BadRequest("Book not found.");
            }

            if (rentalDTO.BookId != currentRental.BookId && await _rentalService.BookRentedBetweenDates(rentalDTO.BookId, rentalDTO.RentalDate, rentalDTO.ReturnDate)) {
                return Conflict("New book is not available during the selected times.");
            }


            Renter? renter = await _renterService.GetRawRenterOnId(rentalDTO.RenterId);
            if (renter == null)
            {
                return BadRequest("Renter not found");
            }

            if (rentalDTO.RentalDate >= rentalDTO.ReturnDate)
            {
                return BadRequest("Return date must be after the rental date.");
            }


            _context.Entry(currentRental).State = EntityState.Modified;

            currentRental.Book = book;
            currentRental.RenterId = rentalDTO.RenterId;
            currentRental.Renter = renter;
            currentRental.RentalDate = rentalDTO.RentalDate;
            currentRental.ReturnDate = rentalDTO.ReturnDate;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: api/Rentals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RentalShowDTO>> PostRental(RentalCreateWithDaysDTO rentalDTO)
        {

            Book? book = await _bookService.GetRawBookOnId(rentalDTO.BookId);
            if (book == null)
            {
                return BadRequest("Book not found.");
            }

            bool bookRented = await _context.Rentals.AnyAsync(rental => rental.BookId == rentalDTO.BookId && rental.RentalDate <= DateTime.UtcNow && rental.ReturnDate >= DateTime.UtcNow && rental.returned == false);

            if (bookRented)
            {
                return Conflict("This book is rented out and hence not currently available.");
            }

            IEnumerable<Author> bookAuthors = await _bookAuthorsService.GetBookAuthors(rentalDTO.BookId);

            Renter? renter = await _renterService.GetRawRenterOnId(rentalDTO.RenterId);
            if (renter == null)
            {
                return BadRequest("Renter not found");
            }


            Rental rental = new Rental
            {
                Book = book,
                RenterId = rentalDTO.RenterId,
                BookId = rentalDTO.BookId,
                Renter = renter,
                RentalDate = DateTime.UtcNow,
                ReturnDate = (DateTime.UtcNow.AddDays(rentalDTO.Days))
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, RentalShowDTO.RentalToRentalShowDTO(rental, BookShowDTO.BookToBookShowDTO(book, bookAuthors), RenterShowDTO.RenterToRenterShowDTO(renter)));
        }


        // POST: api/RentalsByDate
        [HttpPost("ByDate")] 
        public async Task<IActionResult> PostRentalByDate(RentalCreateWithStartDateDTO rentalDTO)
        {
            Book? book = await _bookService.GetRawBookOnId(rentalDTO.BookId);
            if (book == null)
            {
                return BadRequest("Book not found.");
            }

            if (await _rentalService.BookRentedBetweenDates(rentalDTO.BookId, rentalDTO.RentalDate, rentalDTO.ReturnDate))
            {
                return Conflict("New book is not available during the selected times.");
            }


            Renter? renter = await _renterService.GetRawRenterOnId(rentalDTO.RenterId);
            if (renter == null)
            {
                return BadRequest("Renter not found");
            }

            if (rentalDTO.RentalDate >= rentalDTO.ReturnDate)
            {
                return BadRequest("Return date must be after the rental date.");
            }

            Rental rental = new Rental
            {
                Book = book,
                RenterId = rentalDTO.RenterId,
                BookId = rentalDTO.BookId,
                Renter = renter,
                RentalDate = rentalDTO.RentalDate,
                ReturnDate = rentalDTO.ReturnDate
            };

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            IEnumerable<Author> bookAuthors = await _bookAuthorsService.GetBookAuthors(rentalDTO.BookId);

            return CreatedAtAction(nameof(GetRental), new { id = rental.Id }, RentalShowDTO.RentalToRentalShowDTO(rental, BookShowDTO.BookToBookShowDTO(book, bookAuthors), RenterShowDTO.RenterToRenterShowDTO(renter)));


        }

        // PATCH: api/Rentals/5  
        [HttpPatch("{id}")]
        public async Task<IActionResult> ReturnRental(Guid id, bool returned)
        {
            Rental? rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            if (returned)
            {
                rental.returned = true;
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }
        // DELETE: api/Rentals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRental(Guid id)
        {
            var rental = await _context.Rentals.FindAsync(id);
            if (rental == null)
            {
                return NotFound();
            }

            _context.Rentals.Remove(rental);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RentalExists(Guid id)
        {
            return _context.Rentals.Any(e => e.Id == id);
        }
    }
}
