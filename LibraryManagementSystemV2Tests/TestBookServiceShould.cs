using AutoMapper;
using LibraryManagementSystemV2.CustomExceptions.Books;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.EntityDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2Tests.Mocks;
using Moq;
using System.Linq.Expressions;


namespace LibraryManagementSystemV2Tests 
{
    public class TestBookServiceShould
    {

        private readonly TestBookService _testedService;
        private readonly Mock<IGenericRepository<Book>> _mockBookRepository;  
        private readonly Mock<IGenericRepository<Author>> _mockAuthorRepository;
        private readonly Mock<IGenericRepository<Entity>> _mockEntityRepository;
        private readonly Mock<IGenericRepository<AuthorBook>> _genericMockAuthorBookRepositoy;

        public TestBookServiceShould()
        {

            _mockBookRepository = new Mock<IGenericRepository<Book>>(); 
            _mockBookRepository.Setup(m => m.GetAllAsync(It.IsAny<bool>())).ReturnsAsync(GetDummyBooks());
            _mockBookRepository.Setup(m => m.GetByIdAsync(It.IsAny<long>())).ReturnsAsync((long id) => GetDummyBooks().FirstOrDefault(b => b.Id == id));
            _mockBookRepository.Setup(m => m.SetEntryStateToModified(It.IsAny<Book>()));
            
            var mockAuthorBookRepository = new Mock<IAuthorBookRepository>();
            mockAuthorBookRepository.Setup(m => m.GetBookAuthors(It.IsAny<long>())).ReturnsAsync((long id) => GetDummyBookAuthors(id));


            _mockAuthorRepository = new Mock<IGenericRepository<Author>>();
            _mockAuthorRepository.Setup(m => m.GetAllAsync(
                It.IsAny<Expression<Func<Author, bool>>>(),
                It.IsAny<bool>(),
                It.IsAny<Expression<Func<Author, object>>[]>()
            )).ReturnsAsync(new List<Author> { });


            _mockEntityRepository = new Mock<IGenericRepository<Entity>>();
            _genericMockAuthorBookRepositoy = new Mock<IGenericRepository<AuthorBook>>();


            var mockUnitOfWork = new Mock<IUnitOfWork>();  
            mockUnitOfWork.Setup(m => m.Repository<Book>()).Returns(_mockBookRepository.Object);
            mockUnitOfWork.Setup(m => m.AuthorBookRepository()).Returns(mockAuthorBookRepository.Object);
            mockUnitOfWork.Setup(m => m.Repository<Author>()).Returns(_mockAuthorRepository.Object);
            mockUnitOfWork.Setup(m => m.Repository<Entity>()).Returns(_mockEntityRepository.Object);
            mockUnitOfWork.Setup(m => m.Repository<AuthorBook>()).Returns(_genericMockAuthorBookRepositoy.Object);
            mockUnitOfWork.Setup(m => m.StartTransactionAsync()).ReturnsAsync(new TransactionMock()); 


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
            var expectedAuthors = GetDummyBookAuthors(bookIdToSearch);

            var result = await _testedService.GetByIdAsync(bookIdToSearch);

            var bookAuthors = result.Authors;
            Assert.Equal(expectedAuthors.Count(), bookAuthors.Count);
            foreach (var author in bookAuthors)
            {
                Assert.Contains(author.Id, expectedAuthors.Select(author => author.AuthorId));
                Assert.Contains(author.FirstName, expectedAuthors.Select(author => author.Author.Entity.FirstName));
                Assert.Contains(author.LastName, expectedAuthors.Select(author => author.Author.Entity.LastName));
            }
        }


        [Fact]
        public async Task CallBookCreationOperationsWhenAddingBook()
        {
            var bookCreateDTO = new BookCreateDTO
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
                AuthorIDs = new List<long> {},
                NewAuthors = new List<AuthorCreateDTO> {}
            }; 

            var result = await _testedService.AddAsync(bookCreateDTO);


            _mockBookRepository.Verify(m => m.AddAsync(It.Is<Book>(book =>
                book.Name == bookCreateDTO.Name &&
                book.Description == bookCreateDTO.Description &&
                Math.Abs(book.Price - bookCreateDTO.Price) < 0.001 &&
                book.Quantity == bookCreateDTO.Quantity
            )), Times.Once);  

           
            _mockEntityRepository.Verify(m => m.AddAsync(It.IsAny<Entity>()), Times.Never);
            _mockAuthorRepository.Verify(m => m.AddAsync(It.IsAny<Author>()), Times.Never);
            _mockEntityRepository.Verify(m => m.AddAsync(It.IsAny<Entity>()), Times.Never);
        }

        [Fact]
        public async Task CallAuthorCreationOperationsWhenCreatingAuthorsAlongsideBook()
        {
            var bookCreateDTO = new BookCreateDTO
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
                AuthorIDs = new List<long> {},
                NewAuthors = new List<AuthorCreateDTO>
               {
                   new AuthorCreateDTO
                   {
                       Entity = new EntityCreateDTO { FirstName = "John", LastName = "Doe" }
                   },
                   new AuthorCreateDTO
                   {
                       Entity = new EntityCreateDTO { FirstName = "Jane", LastName = "Smith" }
                   }
               }
            };


            var result = await _testedService.AddAsync(bookCreateDTO);


            bookCreateDTO.NewAuthors.ToList().ForEach(newAuthor =>
            {

                _mockEntityRepository.Verify(m => m.AddAsync(It.Is<Entity>(e =>
                                         e.FirstName == newAuthor.Entity.FirstName &&
                                         e.LastName == newAuthor.Entity.LastName
                )), Times.Once); 

                _mockAuthorRepository.Verify(m => m.AddAsync(It.Is<Author>(a =>
                                          a.Entity.FirstName == newAuthor.Entity.FirstName &&
                                          a.Entity.LastName == newAuthor.Entity.LastName
                )), Times.Once);

                _genericMockAuthorBookRepositoy.Verify(m => m.AddAsync(It.Is<AuthorBook>(authorBook =>
                    authorBook.Author.Entity.FirstName == newAuthor.Entity.FirstName &&
                    authorBook.Author.Entity.LastName == newAuthor.Entity.LastName &&
                    authorBook.Book.Name == bookCreateDTO.Name &&
                    authorBook.Book.Description == bookCreateDTO.Description &&
                    Math.Abs(authorBook.Book.Price - bookCreateDTO.Price) < 0.01f &&
                    authorBook.Book.Quantity == bookCreateDTO.Quantity
                )), Times.Once);



            });
        }

        [Fact]
        public async Task CreateAuthorBookLinkBetweenNewBookAndExistingAuthors() 
        {

            var bookCreateDTO = new BookCreateDTO
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
                AuthorIDs = new List<long> { 1,2 },
                NewAuthors = new List<AuthorCreateDTO>{}
            };
            var authors = new List<Author> { new Author { Id = 1, Entity = new Entity { Id = 1, FirstName = "John", LastName = "Doe" } }, new Author { Id = 2, Entity = new Entity { Id = 2, FirstName = "Jane", LastName = "Smith" } } };

            _mockAuthorRepository.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<Author, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<Author, object>>[]>())).ReturnsAsync(authors);


            var result = await _testedService.AddAsync(bookCreateDTO);


            authors.ForEach(author =>
            {

                _genericMockAuthorBookRepositoy.Verify(m => m.AddAsync(It.Is<AuthorBook>(authorBook =>
                    authorBook.Author.Entity.FirstName == author.Entity.FirstName &&
                    authorBook.Author.Entity.LastName == author.Entity.LastName &&
                    authorBook.Book.Name == bookCreateDTO.Name &&
                    authorBook.Book.Description == bookCreateDTO.Description &&
                    Math.Abs(authorBook.Book.Price - bookCreateDTO.Price) < 0.01f &&
                    authorBook.Book.Quantity == bookCreateDTO.Quantity
                )), Times.Once);



            });

            _mockEntityRepository.Verify(m => m.AddAsync(It.IsAny<Entity>()), Times.Never);
            _mockAuthorRepository.Verify(m => m.AddAsync(It.IsAny<Author>()), Times.Never);
        } 



          [Fact] 
          public async Task NotUpdateIfBookDoesNotExist()
            {
                var bookUpdateDTO = new BookUpdateDTO
                {
                    Name = "Sample Book",
                    Description = "A sample book description.",
                    Price = 19.99f,
                    Quantity = 10,
                    AuthorIDs = new List<long> { },
                    NewAuthors = new List<AuthorCreateDTO> { }
                };

                _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(false);

                await Assert.ThrowsAsync<BookNotFoundException>(async () => await _testedService.UpdateAsync(1, bookUpdateDTO));

      
          }

        [Fact]
        public async Task UpdateBookProperties()
        {

            var bookIdToUpdate = 1;
            var bookUpdateDTO = new BookUpdateDTO
            {
                Name = "Updated Sample Book",
                Description = "An updated sample book description.",
                Price = 100.00f,
                Quantity = 1,
                AuthorIDs = new List<long> {},
                NewAuthors = new List<AuthorCreateDTO> { }
            }; 

            var originalBook = new Book 
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
            };

            _genericMockAuthorBookRepositoy.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<AuthorBook, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<AuthorBook, object>>[]>())).ReturnsAsync(new List<AuthorBook>());

            _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(true);
            _mockBookRepository.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(originalBook);


            await _testedService.UpdateAsync(bookIdToUpdate, bookUpdateDTO);

            _mockBookRepository.Verify(m => m.SetEntryStateToModified(It.Is<Book>(book =>
                book.Name == bookUpdateDTO.Name &&
                book.Description == bookUpdateDTO.Description &&
                Math.Abs(book.Price - bookUpdateDTO.Price) < 0.01f &&
                book.Quantity == bookUpdateDTO.Quantity
            )), Times.Once); 

            
        }


        [Fact]
        public async Task UpdateBookAuthorThatExists()
        {

            var bookIdToUpdate = 1;
            var bookUpdateDTO = new BookUpdateDTO
            {
                Name = "Updated Sample Book",
                Description = "An updated sample book description.",
                Price = 100.00f,
                Quantity = 1,
                AuthorIDs = new List<long> { 1 },
                NewAuthors = new List<AuthorCreateDTO> { }
            };

            var originalBook = new Book
            {
                Name = "Sample Book",
                Description = "A sample book description.",
                Price = 19.99f,
                Quantity = 10,
            };

            var author = new Author { Id = 1, Entity = new Entity { Id = 1, FirstName = "John", LastName = "Doe" } };

            _genericMockAuthorBookRepositoy.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<AuthorBook, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<AuthorBook, object>>[]>())).ReturnsAsync(new List<AuthorBook>());

            _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(true);
            _mockBookRepository.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(originalBook);

            _mockAuthorRepository.Setup(m => m.GetByIdAsync(bookUpdateDTO.AuthorIDs.First())).ReturnsAsync(author);

            await _testedService.UpdateAsync(bookIdToUpdate, bookUpdateDTO);

            _genericMockAuthorBookRepositoy.Verify(m => m.AddAsync(It.Is<AuthorBook>(authorBook =>
                    authorBook.Author.Entity.FirstName == author.Entity.FirstName &&
                    authorBook.Author.Entity.LastName == author.Entity.LastName &&
                    authorBook.Book.Name == bookUpdateDTO.Name &&
                    authorBook.Book.Description == bookUpdateDTO.Description &&
                    Math.Abs(authorBook.Book.Price - bookUpdateDTO.Price) < 0.01f &&
                    authorBook.Book.Quantity == bookUpdateDTO.Quantity
                )), Times.Once);
        }


        [Fact]
        public async Task UpdateBookAuthorThatDoesNotExist()
        {

            var bookIdToUpdate = 1;
            var bookUpdateDTO = new BookUpdateDTO
            {
                Name = "Updated Sample Book",
                Description = "An updated sample book description.",
                Price = 100.00f,
                Quantity = 1,
                AuthorIDs = new List<long> {},
                NewAuthors = new List<AuthorCreateDTO>
                   {
                       new AuthorCreateDTO
                       {
                           Entity = new EntityCreateDTO { FirstName = "John", LastName = "Doe" }
                       },
                       new AuthorCreateDTO
                       {
                           Entity = new EntityCreateDTO { FirstName = "Jane", LastName = "Smith" }
                       }
                   }
            };

            var originalBook = new Book
            {
                Name = bookUpdateDTO.Name,
                Description = bookUpdateDTO.Description,
                Price = bookUpdateDTO.Price,
                Quantity = bookUpdateDTO.Quantity
            };


            _genericMockAuthorBookRepositoy.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<AuthorBook, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<AuthorBook, object>>[]>())).ReturnsAsync(new List<AuthorBook>());

            _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(true);
            _mockBookRepository.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(originalBook);

            await _testedService.UpdateAsync(bookIdToUpdate, bookUpdateDTO);

            bookUpdateDTO.NewAuthors.ToList().ForEach(newAuthor =>
            {

                _mockEntityRepository.Verify(m => m.AddAsync(It.Is<Entity>(e =>
                                         e.FirstName == newAuthor.Entity.FirstName &&
                                         e.LastName == newAuthor.Entity.LastName
                )), Times.Once);

                _mockAuthorRepository.Verify(m => m.AddAsync(It.Is<Author>(a =>
                                          a.Entity.FirstName == newAuthor.Entity.FirstName &&
                                          a.Entity.LastName == newAuthor.Entity.LastName
                )), Times.Once);

                _genericMockAuthorBookRepositoy.Verify(m => m.AddAsync(It.Is<AuthorBook>(authorBook =>
                    authorBook.Author.Entity.FirstName == newAuthor.Entity.FirstName &&
                    authorBook.Author.Entity.LastName == newAuthor.Entity.LastName &&
                    authorBook.Book.Name == bookUpdateDTO.Name &&
                    authorBook.Book.Description == bookUpdateDTO.Description &&
                    Math.Abs(authorBook.Book.Price - bookUpdateDTO.Price) < 0.01f &&
                    authorBook.Book.Quantity == bookUpdateDTO.Quantity
                )), Times.Once);



            });
        }
    }
}
