using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.LibraryStatisticsDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using LibraryManagementSystemV2.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryStatisticsController : ControllerBase
    {

        private readonly SQLiteContext _context;
        private readonly IBookAuthorService _bookAuthorsService;
        private readonly IRentalService _rentalService; 

        public LibraryStatisticsController(SQLiteContext context, IBookAuthorService bookAuthorsService, IRentalService rentalService)
        {
            _context = context;
            _bookAuthorsService = bookAuthorsService;
            _rentalService = rentalService;
        }

        // GET: api/Rentals/ForBook
        [HttpGet("OverdueBooks")]
        public async Task<ActionResult<IEnumerable<OverdueShowDTO>>> GetOverdueBooks()
        {

            var rentals = await _context.Rentals
                               .Where(rental => !rental.returned && rental.ReturnDate < DateTime.UtcNow)
                               .Include(rental => rental.Book)
                               .Include(rental => rental.Renter)
                               .ThenInclude(renter => renter.Entity)
                               .OrderBy(renter => renter.ReturnDate)
                               .ToListAsync();


            var overdueShowDTOs = new List<OverdueShowDTO>(); 

            foreach (var rental in rentals)
            {
                var bookAuthors = await _bookAuthorsService.GetBookAuthors(rental.Book.Id);
                var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
                var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

                RentalShowDTO showDTO = RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO);

                overdueShowDTOs.Add(OverdueShowDTO.RentalToOverdueShowDTO(showDTO));
            }

            return Ok(overdueShowDTOs);

        }


        //[HttpGet("BooksOnLoanForUser")]
        //public async Task<ActionResult<IEnumerable<OverdueShowDTO>>> GetBooksOnLoadPerUser(long userId)
        //{

        //    var rentals = await _rentalService.CurrentlyTakenOutBooks()
        //                       .Include(rental => rental.Book)   
        //                       .Include(rental => rental.Renter)
        //                       .ThenInclude(renter => renter.Entity)
        //                       .OrderBy(renter => renter.ReturnDate)
        //                       .ToListAsync();


        //    var overdueShowDTOs = new List<OverdueShowDTO>();

        //    foreach (var rental in rentals)
        //    {
        //        var bookAuthors = await _bookAuthorsService.GetBookAuthors(rental.Book.Id);
        //        var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
        //        var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

        //        RentalShowDTO showDTO = RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO);

        //        overdueShowDTOs.Add(OverdueShowDTO.RentalToOverdueShowDTO(showDTO));
        //    }

        //    return Ok(overdueShowDTOs);

        //}
    }
}
