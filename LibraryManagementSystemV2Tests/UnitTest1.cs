using AutoMapper;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.Controllers;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2.Services.GenericServiceMappings;
using LibraryManagementSystemV2Tests.Mocks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Data.SqlTypes;

namespace LibraryManagementSystemV2Tests 
{
    public class TestBookApiShould
    {
        //private readonly Mock<DbSet<Book>> _mockBookDbSet;
        //private readonly Mock<DbContext> _mockContext;
        private readonly IGenericRepository<Book> _bookRepository;
        private readonly TestBookController _bookController;

        public TestBookApiShould()
        {

            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);  

            var mockRepository = new Mock<IGenericRepository<Book>>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockRepository.Setup(m => m.GetAllAsync(It.IsAny<bool>())).ReturnsAsync(GetDummyBooks());
            mockUnitOfWork.Setup(m => m.Repository<Book>()).Returns(mockRepository.Object);
            var service = new TestBookService(mockUnitOfWork.Object, mapper);
            _bookController = new TestBookController(service);

        }

        public List<Book> GetDummyBooks(bool tracked = true)
        {
            return new List<Book>
                            {
                                new Book
                                {
                                    Id = 1,
                                    Name = "The Great Gatsby",
                                    Description = "A novel written by American author F. Scott Fitzgerald.",
                                    Price = 10.99f,
                                    Quantity = 5
                                },
                                new Book
                                {
                                    Id = 2,
                                    Name = "1984",
                                    Description = "A dystopian social science fiction novel and cautionary tale by the English writer George Orwell.",
                                    Price = 8.99f,
                                    Quantity = 10
                                },
                                new Book
                                {
                                    Id = 3,
                                    Name = "To Kill a Mockingbird",
                                    Description = "A novel by Harper Lee published in 1960.",
                                    Price = 12.99f,
                                    Quantity = 7
                                },
                                new Book
                                {
                                    Id = 4,
                                    Name = "Pride and Prejudice",
                                    Description = "A romantic novel of manners written by Jane Austen.",
                                    Price = 9.99f,
                                    Quantity = 3
                                }
                            };
        }

        [Fact]
        public async Task ReturnAllBooks()
        {
            var result = await _bookController.GetAllBooks();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var books = Assert.IsType<List<BookShowDTO>>(okResult.Value);

            Assert.Equal(4, books.Count);
        }

        [Fact]
        public async Task ReturnAllBooksAsDTOs()
        {
            var result = await _bookController.GetAllBooks();

            var okResult = Assert.IsType<OkObjectResult>(result.Result); 
            var books = Assert.IsType<List<BookShowDTO>>(okResult.Value); 

        }

        [Fact]
        public async Task ReturnCorrectBooks()  
        {
            var result = await _bookController.GetAllBooks();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var books = Assert.IsType<List<BookShowDTO>>(okResult.Value);

            Assert.Contains(books, book => book.Name.Contains("The Great Gatsby"));
            Assert.Contains(books, book => book.Name.Contains("1984"));
            Assert.Contains(books, book => book.Name.Contains("To Kill a Mockingbird"));
            Assert.Contains(books , book => book.Name.Contains("Pride and Prejudice"));

        }

    }
}
