using AutoMapper;
using LibraryManagementSystemV2.CustomExceptions.Books;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.EntityDTOs;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Mappings;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2.Repositories;
using LibraryManagementSystemV2.Services;
using LibraryManagementSystemV2Tests.Fixtures;
using LibraryManagementSystemV2Tests.Mocks;
using Moq;
using System.Linq.Expressions;


namespace LibraryManagementSystemV2Tests
{
    public class TestBookServiceShould : TestBookServiceFixture
    {

        [Fact] 
        public async Task ReturnAllBooks()
        {
            var result = await _testedService.GetAllAsync();

            Assert.Equal(_testBooks.Count, result.Count());
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
            var bookNames = _testBooks.Select(testBook => testBook.Name).ToList();

            bookNames.ForEach(bookName => Assert.Contains(result, book => book.Name.Contains(bookName)));
        }

        [Fact]
        public async Task ReturnCorrectAuthorsForEachBook()
        {
            var result = await _testedService.GetAllAsync();
            result = result.ToList();

            foreach (var book in result)
            {
                var bookAuthors = _testAuthorBooks.Where(authorBook => authorBook.BookId == book.Id).Select(authorBook => authorBook.Author).ToList(); 

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
            Book relevantBook = _testBooks[bookIdToSearch];

            var result = await _testedService.GetByIdAsync(bookIdToSearch);

            Assert.True(result.Id == bookIdToSearch);
            Assert.Equal(result.Name, relevantBook.Name);
            Assert.Equal(result.Description, relevantBook.Description);
        }

        [Fact]
        public async Task ReturnCorrectBookAuthorsWhenFetchingById()
        {
            const int bookIdToSearch = 3;
            var expectedAuthors = _testAuthorBooks.Where(authorBook => authorBook.BookId == bookIdToSearch).Select(authorBook => authorBook.Author).ToList();

            var result = await _testedService.GetByIdAsync(bookIdToSearch);

            var bookAuthors = result.Authors;

            Assert.Equal(expectedAuthors.Count, bookAuthors.Count);
            foreach (var author in bookAuthors)
            {
                Assert.Contains(author.Id, expectedAuthors.Select(author => author.Id));
                Assert.Contains(author.FirstName, expectedAuthors.Select(author => author.Entity.FirstName));
                Assert.Contains(author.LastName, expectedAuthors.Select(author => author.Entity.LastName));
            }
        }


        [Fact]
        public async Task CallBookCreationOperationsWhenAddingBook()
        {
            var bookCreateDTO = NewEmptyBookToCreate();

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
            var newAuthors = new List<AuthorCreateDTO>
               {
                   new AuthorCreateDTO
                   {
                       Entity = new EntityCreateDTO { FirstName = "John", LastName = "Doe" }
                   },
                   new AuthorCreateDTO
                   {
                       Entity = new EntityCreateDTO { FirstName = "Jane", LastName = "Smith" }
                   }
               };
            var bookCreateDTO = NewBookToCreate(newAuthors);  


            var result = await _testedService.AddAsync(bookCreateDTO);


            newAuthors.ToList().ForEach(newAuthor =>
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

            var bookCreateDTO = NewBookToCreate([1,2]); 
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
                var bookUpdateDTO = NewUpdatedBookToCreate();

                _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(false);

                await Assert.ThrowsAsync<BookNotFoundException>(async () => await _testedService.UpdateAsync(1, bookUpdateDTO));
          }

        [Fact]
        public async Task UpdateBookProperties()
        {

            long bookIdToUpdate = 1;
            var bookUpdateDTO = NewUpdatedBookToCreate();
            var originalBookCreateDTO = NewEmptyBookToCreate();  
            var originalBook = BookCreateDTO.BookCreateDTOToBook(originalBookCreateDTO);    

            _genericMockAuthorBookRepositoy.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<AuthorBook, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<AuthorBook, object>>[]>())).ReturnsAsync(new List<AuthorBook>());
            _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(true);
            _mockBookRepository.Setup(m => m.GetByIdAsync(bookIdToUpdate)).ReturnsAsync(originalBook);


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
            var bookUpdateDTO = NewUpdatedBookToCreate([1]);
            var originalBookCreateDTO = NewEmptyBookToCreate();
            var originalBook = BookCreateDTO.BookCreateDTOToBook(originalBookCreateDTO);
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
            var bookUpdateDTO = NewUpdatedBookToCreate();
            var originalBookCreateDTO = NewEmptyBookToCreate();
            var originalBook = BookCreateDTO.BookCreateDTOToBook(originalBookCreateDTO);

            _genericMockAuthorBookRepositoy.Setup(m => m.GetAllAsync(It.IsAny<Expression<Func<AuthorBook, bool>>>(), It.IsAny<bool>(), It.IsAny<Expression<Func<AuthorBook, object>>[]>())).ReturnsAsync(new List<AuthorBook>());
            _mockBookRepository.Setup(m => m.Exists(It.IsAny<Expression<Func<Book, bool>>>())).Returns(true);
            _mockBookRepository.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(originalBook);

            await _testedService.UpdateAsync(bookIdToUpdate, bookUpdateDTO);

            bookUpdateDTO.NewAuthors?.ToList().ForEach(newAuthor =>
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
