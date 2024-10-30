using AutoMapper;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2Tests.Mocks;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystemV2Tests.Fixtures
{
    public class TestBookServiceFixture
    {
        protected readonly TestBookService _testedService;
        protected readonly Mock<IGenericRepository<Book>> _mockBookRepository;
        protected readonly Mock<IGenericRepository<Author>> _mockAuthorRepository;
        protected readonly Mock<IGenericRepository<Entity>> _mockEntityRepository;
        protected readonly Mock<IGenericRepository<AuthorBook>> _genericMockAuthorBookRepositoy;
        protected readonly Mock<IUnitOfWork> _mockUnitOfWork;
        protected readonly Mock<IAuthorBookRepository> _mockAuthorBookRepository;

        protected readonly List<Author> _testAuthors =
        [
             new Author { Id = 0, Entity = new Entity { Id = 1, FirstName = "F. Scott", LastName = "Fitzgerald" } },
             new Author { Id = 1, Entity = new Entity { Id = 2, FirstName = "George", LastName = "Orwell" } },
             new Author { Id = 2, Entity = new Entity { Id = 3, FirstName = "Harper", LastName = "Lee" } },
             new Author { Id = 4, Entity = new Entity { Id = 4, FirstName = "Terry", LastName = "Pratchett" } },
             new Author { Id = 5, Entity = new Entity { Id = 5, FirstName = "Neil", LastName = "Gaiman" } },
             new Author { Id = 3, Entity = new Entity { Id = 6, FirstName = "Jane", LastName = "Austen" } }
        ];

        protected List<AuthorBook> _testAuthorBooks;

        protected readonly List<Book> _testBooks = new List<Book>
                        {
                                new Book
                                {
                                    Id = 0,
                                    Name = "The Great Gatsby",
                                    Description = "A novel written by American author F. Scott Fitzgerald.",
                                    Price = 10.99f,
                                    Quantity = 5
                                },
                                new Book
                                {
                                    Id = 1,
                                    Name = "1984",
                                    Description = "A dystopian social science fiction novel and cautionary tale by the English writer George Orwell.",
                                    Price = 8.99f,
                                    Quantity = 10
                                },
                                new Book
                                {
                                    Id = 2,
                                    Name = "To Kill a Mockingbird",
                                    Description = "A novel by Harper Lee published in 1960.",
                                    Price = 12.99f,
                                    Quantity = 7
                                },
                                new Book
                                {
                                    Id = 3,
                                    Name = "Pride and Prejudice",
                                    Description = "A romantic novel of manners written by Jane Austen.",
                                    Price = 9.99f,
                                    Quantity = 3
                                },
                                new Book
                                {
                                    Id = 4,
                                    Name = "Good Omens",
                                    Description = "A novel by Terry Pratchett and Neil Gaiman.",
                                    Price = 11.99f,
                                    Quantity = 2
                                }
                            };


        public TestBookServiceFixture()
        {
            InitialiseTestAuthorBooks();

            _mockBookRepository = new Mock<IGenericRepository<Book>>();
            InitialiseBookRepositoryMock();

            _mockAuthorBookRepository = new Mock<IAuthorBookRepository>();
            InitialiseAuthorBookRepositoryMock();


            _mockAuthorRepository = new Mock<IGenericRepository<Author>>();
            InitialiseAuthorRepositoryMock();


            _mockEntityRepository = new Mock<IGenericRepository<Entity>>();
            _genericMockAuthorBookRepositoy = new Mock<IGenericRepository<AuthorBook>>();


            _mockUnitOfWork = new Mock<IUnitOfWork>();
            InitialiseUnitOfWorkMock();


            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);


            _testedService = new TestBookService(_mockUnitOfWork.Object, mapper);

        }

        private void InitialiseBookRepositoryMock()
        {

            _mockBookRepository.Setup(m => m.GetAllAsync(It.IsAny<bool>())).ReturnsAsync(_testBooks.Cast<Book?>().ToList());
            _mockBookRepository.Setup(m => m.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((long id) => _testBooks.FirstOrDefault(testBook => testBook is not null && testBook.Id == id, null));
            _mockBookRepository.Setup(m => m.SetEntryStateToModified(It.IsAny<Book>()));
        }

        private void InitialiseAuthorRepositoryMock()
        {
            _mockAuthorRepository.Setup(m => m.GetAllAsync(
              It.IsAny<Expression<Func<Author, bool>>>(),
              It.IsAny<bool>(),
              It.IsAny<Expression<Func<Author, object>>[]>()
            )).ReturnsAsync(new List<Author> { });
        }

        private void InitialiseAuthorBookRepositoryMock()
        {
            _mockAuthorBookRepository.Setup(m => m.GetBookAuthors(It.IsAny<long>())).ReturnsAsync((long id) => _testAuthorBooks.Where(authorBook => authorBook.BookId == id));
        }

        private void InitialiseUnitOfWorkMock()
        {

            _mockUnitOfWork.Setup(m => m.Repository<Book>()).Returns(_mockBookRepository.Object);
            _mockUnitOfWork.Setup(m => m.AuthorBookRepository()).Returns(_mockAuthorBookRepository.Object);
            _mockUnitOfWork.Setup(m => m.Repository<Author>()).Returns(_mockAuthorRepository.Object);
            _mockUnitOfWork.Setup(m => m.Repository<Entity>()).Returns(_mockEntityRepository.Object);
            _mockUnitOfWork.Setup(m => m.Repository<AuthorBook>()).Returns(_genericMockAuthorBookRepositoy.Object);
            _mockUnitOfWork.Setup(m => m.StartTransactionAsync()).ReturnsAsync(new TransactionMock());
        }

        private void InitialiseTestAuthorBooks()
        {
            _testAuthorBooks = [
                 new AuthorBook { AuthorId = 0, BookId = 0, Author = _testAuthors[0], Book = _testBooks[0] },
                 new AuthorBook { AuthorId = 1, BookId = 1, Author = _testAuthors[1], Book = _testBooks[1] },
                 new AuthorBook { AuthorId = 2, BookId = 2, Author = _testAuthors[2], Book = _testBooks[2] },
                 new AuthorBook { AuthorId = 3, BookId = 3, Author = _testAuthors[3], Book = _testBooks[3] },
                 new AuthorBook { AuthorId = 4, BookId = 4, Author = _testAuthors[4], Book = _testBooks[4] },
                 new AuthorBook { AuthorId = 5, BookId = 4, Author = _testAuthors[5], Book = _testBooks[4] }
            ];
        }

        protected BookCreateDTO NewEmptyBookToCreate()
        {
            return new BookCreateDTO
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
                AuthorIDs = new List<long> { },
                NewAuthors = new List<AuthorCreateDTO> { }
            };
        }

        protected BookUpdateDTO NewUpdatedBookToCreate()
        {
            return new BookUpdateDTO
            {
                Name = "Updated Sample Book",
                Description = "An updated sample book description.",
                Price = 100.00f,
                Quantity = 1,
                AuthorIDs = new List<long> { },
                NewAuthors = new List<AuthorCreateDTO> { }
            };
        }

        protected BookUpdateDTO NewUpdatedBookToCreate(ICollection<long> authorIds)
        {
            var newBook = NewUpdatedBookToCreate();
            newBook.AuthorIDs = authorIds;
            return newBook;
        }

        protected BookCreateDTO NewBookToCreate(ICollection<AuthorCreateDTO> newAuthors)
        {
            var newBook = NewEmptyBookToCreate();
            newBook.NewAuthors = newAuthors;
            return newBook;
        }

        protected BookCreateDTO NewBookToCreate(ICollection<long> existingAuthors)
        {

            var newBook = NewEmptyBookToCreate();
            newBook.AuthorIDs = existingAuthors;
            return newBook;
        }

        public void Dispose()
        {
            // ... clean up test data from the database ...
        }
    }

}
