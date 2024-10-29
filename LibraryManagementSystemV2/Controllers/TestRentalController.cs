using LibraryManagementSystemV2.CustomExceptions.Books;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using Serilog;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using LibraryManagementSystemV2.DTOs.LibraryStatisticsDTOs;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.DTOs.RentalDTOs;

namespace LibraryManagementSystemV2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestRentalController : ControllerBase
    {

        private readonly ITestRentalMapping _service;

        public TestRentalController(ITestRentalMapping service)
        {
            _service = service;
        }

        [HttpGet("GetRentals")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<BookShowDTO>>> GetAllRentals()
        {
            Log.Information("Getting all rentals.");
            var rentals = await _service.GetAllAsync();
            return Ok(rentals);
        }


        // GET: api/Rentals/ForBook
        [HttpGet("OverdueRentals")]
        public async Task<ActionResult<IEnumerable<OverdueShowDTO>>> GetOverdueBooks()
        {
            Log.Information("Getting all overdue rentals");
            var overdueRentals = await _service.GetOverdueRentals();
            return Ok(overdueRentals);
        }

        [HttpGet("UserWithMostBooksOnLoan")]
        public async Task<ActionResult<RenterWithRentalsShowDTO>> GetRenterWithMostRentals()
        {
            Log.Information("Getting user with most rentals");
            RenterWithRentalsShowDTO maxRenter = await _service.GetRenterWithMostRentals();
            return Ok(maxRenter);
        }

        [HttpGet("GetRentalsForUser")]
        public async Task<ActionResult<RenterWithRentalsShowDTO>> GetRentalsForUser(long renterID)
        {
            Log.Information("Getting user with most rentals");
            RenterWithRentalsNoRenterShowDTO renterWithRentals = await _service.GetRenterWithRentals(renterID);
            return Ok(renterWithRentals);
        }
    }
}
