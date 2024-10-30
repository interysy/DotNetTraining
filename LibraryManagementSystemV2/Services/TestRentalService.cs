using AutoMapper;
using LibraryManagementSystemV2.CustomExceptions.Renters;
using LibraryManagementSystemV2.DTOs.LibraryStatisticsDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.DTOs.RentalDTOs;
using LibraryManagementSystemV2.DTOs.RenterDTOs;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories.Interfaces;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using LibraryManagementSystemV2.Services.GenericServices;
using LibraryManagementSystemV2.Services.Interfaces;
using LibraryManagementSystemV2.Types;

namespace LibraryManagementSystemV2.Services
{
    public class TestRentalService : GenericService<Rental, RentalShowDTO, RentalCreateWithDaysDTO, RentalCreateWithDateDTO>, ITestRentalMapping
    {
        public TestRentalService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
        }

        public new async Task<IEnumerable<RentalShowDTO>> GetAllAsync()
        {
            List<Rental> rentals = await _unitOfWork.RentalRepository().GetAllAsync(false);

            var rentalDTOs = new List<RentalShowDTO>();

            foreach (Rental? rental in rentals)
            {
                IEnumerable<AuthorBook> bookAuthors = await _unitOfWork.AuthorBookRepository().GetBookAuthors(rental.BookId);
                IEnumerable<Author> authors = bookAuthors.Select(authorBook => authorBook.Author); 

                var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, authors);
                var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

                rentalDTOs.Add(RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO));
            }
            return rentalDTOs;
        }

        public async Task<IEnumerable<RentalShowDTO>> GetOverdueRentals()  
        {
            var rentals = await _unitOfWork.RentalRepository().GetOverdueBooksAsync();


            var overdueShowDTOs = new List<OverdueShowDTO>();

            foreach (var rental in rentals)
            {
                IEnumerable<AuthorBook> bookAuthors = await _unitOfWork.AuthorBookRepository().GetBookAuthors(rental.BookId);
                IEnumerable<Author> authors = bookAuthors.Select(authorBook => authorBook.Author); 

                var bookDTO = BookShowDTO.BookToBookShowDTO(rental.Book, authors);
                var renterDTO = RenterShowDTO.RenterToRenterShowDTO(rental.Renter);

                RentalShowDTO showDTO = RentalShowDTO.RentalToRentalShowDTO(rental, bookDTO, renterDTO);

                overdueShowDTOs.Add(OverdueShowDTO.RentalToOverdueShowDTO(showDTO));
            }

            return overdueShowDTOs;

        }

        public async Task<RenterWithRentalsShowDTO> GetRenterWithMostRentals()
        {
            RenterWithCount? rentalWithCount = await _unitOfWork.RentalRepository().GetRenterWithMostRentalsAsync();

            if (rentalWithCount is null)
            {
                throw new RenterNotFoundException("Cannot find renter with the most rentals.");
            }

            Renter? renter = await _unitOfWork.Repository<Renter>().GetByIdAsync(rentalWithCount.RenterId, renter => renter.Entity);
            if (renter is null) { throw new RenterNotFoundException("Cannot find renter with the most rentals."); } 

            RenterShowDTO renterShow = RenterShowDTO.RenterToRenterShowDTO(renter);
            IEnumerable<Rental> rentals = await _unitOfWork.Repository<Rental>().GetAllAsync(rental => rental.RenterId == rentalWithCount.RenterId, true, rental => rental.Book);

            var rentalShows = new List<RentalShowDTO>();
            foreach (Rental rental in rentals)
            {
                IEnumerable<Author> bookAuthors = (await _unitOfWork.AuthorBookRepository().GetBookAuthors(rental.BookId)).Select(authorBook => authorBook.Author);
                BookShowDTO bookShow = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
                rentalShows.Add(RentalShowDTO.RentalToRentalShowDTO(rental, bookShow, renterShow));
            }

            return RenterWithRentalsShowDTO.RenterWithRentalsToRenterWithRentalsShowDTO(renter, rentalWithCount.Count, rentalShows);
        }

        public async Task<RenterWithRentalsNoRenterShowDTO> GetRenterWithRentals(long renterId)  
        {
            Renter? renter = await _unitOfWork.Repository<Renter>().GetByIdAsync(renterId, renter => renter.Entity);


            if (renter is null)
            {
                throw new RenterNotFoundException("Cannot find renter with the most rentals.");
            }

            RenterShowDTO renterShow = RenterShowDTO.RenterToRenterShowDTO(renter);
            IEnumerable<Rental> rentals = await _unitOfWork.Repository<Rental>().GetAllAsync(rental => rental.RenterId == renterId, true, rental => rental.Book);

            var rentalShows = new List<RentalShowWithNoRenterDTO>();
            foreach (Rental rental in rentals)
            {
                IEnumerable<Author> bookAuthors = (await _unitOfWork.AuthorBookRepository().GetBookAuthors(rental.BookId)).Select(authorBook => authorBook.Author);
                BookShowDTO bookShow = BookShowDTO.BookToBookShowDTO(rental.Book, bookAuthors);
                rentalShows.Add(RentalShowWithNoRenterDTO.RentalToRentalShowWithNoRenterDTO(rental, bookShow));
            }

            return RenterWithRentalsNoRenterShowDTO.RenterWithRentalsToRenterWithRentalsShowDTO(renter, rentalShows.Count, rentalShows);

        } 
    }
}
