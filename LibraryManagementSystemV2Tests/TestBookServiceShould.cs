using AutoMapper;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services;
using Moq;


namespace LibraryManagementSystemV2Tests 
{
    public class TestBookServiceShould
    {

        private readonly TestBookService _testedService;
        private readonly Mock<IGenericRepository<Book>> _mockBookRepository; 

        public TestBookServiceShould()
        {

            _mockBookRepository = new Mock<IGenericRepository<Book>>();
            _mockBookRepository.Setup(m => m.GetAllAsync(It.IsAny<bool>())).ReturnsAsync(GetDummyBooks());
            _mockBookRepository.Setup(m => m.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((long id) => GetDummyBooks().FirstOrDefault(b => b.Id == id)); 

            var mockAuthorBookRepository = new Mock<IAuthorBookRepository>();
            mockAuthorBookRepository.Setup(m => m.GetBookAuthors(It.IsAny<long>())).ReturnsAsync((long id) => GetDummyBookAuthors(id));

            var mockUnitOfWork = new Mock<IUnitOfWork>();  
            mockUnitOfWork.Setup(m => m.Repository<Book>()).Returns(_mockBookRepository.Object);
            mockUnitOfWork.Setup(m => m.AuthorBookRepository()).Returns(mockAuthorBookRepository.Object);


            var mappingProfile = new MappingProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(mappingProfile));
            var mapper = new Mapper(configuration);


            _testedService = new TestBookService(mockUnitOfWork.Object, mapper); 


        }

        public IEnumerable<AuthorBook> GetDummyBookAuthors(long id) {

            Dictionary<long, IEnumerable<AuthorBook>> authorsForBook = new Dictionary<long, IEnumerable<AuthorBook>>();

            var authors = new List<Author>
            {
                new Author { Id = 1, Entity = new Entity { Id = 1, FirstName = "John", LastName = "Doe" } },
                new Author { Id = 2, Entity = new Entity { Id = 2, FirstName = "Jane", LastName = "Smith" } },
                new Author { Id = 3, Entity = new Entity { Id = 3, FirstName = "Alice", LastName = "Johnson" } },
                new Author { Id = 4, Entity = new Entity { Id = 4, FirstName = "Bob", LastName = "Brown" } }
            };


            authorsForBook[1] = new List<AuthorBook>
            {
                new AuthorBook { AuthorId = 1, BookId = 0, Author = authors[0], Book = GetDummyBooks()[0] },
                new AuthorBook { AuthorId = 2, BookId = 0, Author = authors[1], Book = GetDummyBooks()[0] }
            };

            authorsForBook[2] = new List<AuthorBook>
            {
                new AuthorBook { AuthorId = 3, BookId = 1, Author = authors[2], Book = GetDummyBooks()[1] }
            };

            authorsForBook[3] = new List<AuthorBook>
            {
                new AuthorBook { AuthorId = 2, BookId = 2, Author = authors[1], Book = GetDummyBooks()[2] },
                new AuthorBook { AuthorId = 4, BookId = 2, Author = authors[3], Book = GetDummyBooks()[2] }
            };

            authorsForBook[4] = new List<AuthorBook>
            {
                new AuthorBook { AuthorId = 1, BookId = 3, Author = authors[0], Book = GetDummyBooks()[3] },
                new AuthorBook { AuthorId = 3, BookId = 3, Author = authors[2], Book = GetDummyBooks()[3] },
                new AuthorBook { AuthorId = 4, BookId = 3, Author = authors[3], Book = GetDummyBooks()[3] }
            };

            return authorsForBook.ContainsKey(id) ? authorsForBook[id] : Enumerable.Empty<AuthorBook>();
        } 
        public List<Book?> GetDummyBooks(bool tracked = true)
        {
            return new List<Book?>
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
                                }
                            };
        }

        [Fact] 
        public async Task ReturnAllBooks()
        {
            var result = await _testedService.GetAllAsync();

            Assert.Equal(4, result.Count());
        }


        [Fact]
        public async Task ReturnAllBooksAsDTOs()
        {
            var result = await _testedService.GetAllAsync();

            Assert.IsType<List<BookShowDTO>>(result);

        }

        [Fact]
        public async Task ReturnCorrectBooks()
        {
            var result = await _testedService.GetAllAsync();

            Assert.Contains(result, book => book.Name.Contains("The Great Gatsby"));
            Assert.Contains(result, book => book.Name.Contains("1984"));
            Assert.Contains(result, book => book.Name.Contains("To Kill a Mockingbird"));
            Assert.Contains(result, book => book.Name.Contains("Pride and Prejudice"));

        }

        [Fact]
        public async Task ReturnCorrectAuthorsForEachBook()
        {
            var result = await _testedService.GetAllAsync();
            result = result.ToList(); 
            
            foreach (var book in result)
            {
                var bookAuthors = GetDummyBookAuthors(book.Id).Select(authorBook => authorBook.Author).ToList() ;
                Assert.Equal(bookAuthors.Count, book.Authors.Count);
                foreach (var author in book.Authors)
                {
                    Assert.Contains(author.Id, bookAuthors.Select(author => author.Id));
                    Assert.Contains(author.FirstName, bookAuthors.Select(author => author.Entity.FirstName));
                    Assert.Contains(author.LastName, bookAuthors.Select(author => author.Entity.LastName));

                }
            }
        }

        [Fact]
        public async Task ReturnOneBookWhenFetchingById()
        {
            const int bookIdToSearch = 1;
            var result = await _testedService.GetByIdAsync(bookIdToSearch);

            Assert.NotNull(result);
            Assert.IsType<BookShowDTO>(result);
        }

        [Fact]
        public async Task ReturnCorrectBookWhenFetchingById()
        {
            const int bookIdToSearch = 1;
            var result = await _testedService.GetByIdAsync(bookIdToSearch);

            Assert.True(result.Id == bookIdToSearch);
            Assert.Equal(result.Name, GetDummyBooks()[bookIdToSearch].Name);
            Assert.Equal(result.Description, GetDummyBooks()[bookIdToSearch].Description);
        }

        [Fact]
        public async Task ReturnCorrectBookAuthorsWhenFetchingById()
        {
            const int bookIdToSearch = 3;
            var result = await _testedService.GetByIdAsync(bookIdToSearch);
            var bookAuthors = result.Authors;
            var expectedAuthors = GetDummyBookAuthors(bookIdToSearch); 

            Assert.Equal(expectedAuthors.Count(), bookAuthors.Count);
            foreach (var author in bookAuthors)
            {
                Assert.Contains(author.Id, expectedAuthors.Select(author => author.AuthorId));
                Assert.Contains(author.FirstName, expectedAuthors.Select(author => author.Author.Entity.FirstName));
                Assert.Contains(author.LastName, expectedAuthors.Select(author => author.Author.Entity.LastName));
            }
        }

    }
}
