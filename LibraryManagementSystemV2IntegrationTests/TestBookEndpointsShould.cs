using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2IntegrationTests.Utilities;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using Xunit.Abstractions;
using FluentAssertions;
using System.Text;
using LibraryManagementSystemV2.DTOs.AuthorDTOs;
using LibraryManagementSystemV2.DTOs.EntityDTOs;


namespace LibraryManagementSystemV2IntegrationTests
{
    public class TestBookEndpointsShould : IClassFixture<IntegrationTestFactory<Program>>, IAsyncLifetime
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _outputHelper; 
        private readonly BookUtility _bookUtility;
        private readonly AuthorUtility _authorUtility;
        private readonly AuthorBookUtility _authorBookUtility;
        private readonly LibraryManagementContext _context;


        private readonly HttpClient _client;  

        private readonly String Base = "/api/TestBook";


        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        }; 

        public TestBookEndpointsShould(ITestOutputHelper outputHelper)
        {
            _factory = new IntegrationTestFactory<Program>();
            _outputHelper = outputHelper; 
            _bookUtility = new BookUtility();
            _authorUtility = new AuthorUtility();
            _authorBookUtility = new AuthorBookUtility();
            _client = _factory.CreateClient(); 
            var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            _context = scopedServices.GetRequiredService<LibraryManagementContext>();
        }

        public async Task InitializeAsync()
        {
            DatabaseUtilities.RefreshDatabase(_context); 
            await _bookUtility.SeedDataContext(_context);
            await _authorUtility.SeedDataContext(_context);
            await _authorBookUtility.SeedDataContext(_context);
        }

        public Task DisposeAsync()
        {
            return Task.CompletedTask;
        }


        [Fact]
        public async Task ReturnBookShowDTOs()
        {
            var response = await _client.GetAsync($"{Base}/GetBooks");
            var content = await response.Content.ReadAsStringAsync();
                var books = JsonSerializer.Deserialize<List<BookShowDTO>>(content, _jsonOptions);

                response.StatusCode.Should().Be(HttpStatusCode.OK); 
                books.Should().BeOfType<List<BookShowDTO>>();
        }

        [Fact]
        public async Task ReturnAllBooksInDatabase()
        {

            var response = await _client.GetAsync($"{Base}/GetBooks");
            var content = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookShowDTO>>(content, _jsonOptions);

            books.Should().NotBeNullOrEmpty();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            books?.Count.Should().Be(5);
        }

        [Fact]
        public async Task ReturnCorrectBooksFromDatabase()
        {

            var response = await _client.GetAsync($"{Base}/GetBooks");
            var content = await response.Content.ReadAsStringAsync();
            var books = JsonSerializer.Deserialize<List<BookShowDTO>>(content, _jsonOptions);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            _bookUtility.GetBookNames().ForEach(bookName => books.Should().Contain(b => b.Name == bookName));
        }

        [Fact]
        public async Task ReturnCorrectBookWhenProvidingId()
        {
            var bookIdToLookFor = 1; 
            var bookIndexInArray = bookIdToLookFor - 1; 

            var response = await _client.GetAsync($"{Base}/GetBook/{bookIdToLookFor}");
            var content = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            book.Should().BeEquivalentTo(BookUtility.testBooks[bookIndexInArray]);

        }

        [Fact]
        public async Task ReturnNotFoundWhenProvidingInvalidId()
        {
            var bookIdToLookFor = 100;

            var response = await _client.GetAsync($"{Base}/GetBook/{bookIdToLookFor}");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            content.Should().Be($"Book with ID {bookIdToLookFor} not found.");
        }

        [Fact]
        public async Task ReturnBadRequestWhenProvidingInvalidId()
        {
            var bookIdToLookFor = 0;

            var response = await _client.GetAsync($"{Base}/GetBook/{bookIdToLookFor}");
            var content = await response.Content.ReadAsStringAsync();

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            content.Should().Be("Id must be greater than 0");
        }

        [Fact]
        public async Task ReturnCorrectAuthorAlongsideBook()
        {
            var bookIdToLookFor = 1;
            var authorIndexInArray = bookIdToLookFor - 1;
            var amountOfAuthorsForBook = 1;

            var response = await _client.GetAsync($"{Base}/GetBook/{bookIdToLookFor}");
            var content = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            book.Authors.Count.Should().Be(amountOfAuthorsForBook); 
             
            var author = book.Authors.First(); 
            author.Id.Should().Be(AuthorUtility.testAuthors[authorIndexInArray].Id);
            author.FirstName.Should().Be(AuthorUtility.testAuthors[authorIndexInArray].Entity.FirstName);
            author.LastName.Should().Be(AuthorUtility.testAuthors[authorIndexInArray].Entity.LastName);
        }


        [Fact]
        public async Task ReturnMultipleCorrectAuthorsAlongsideBook()
        {
            var bookIdToLookFor = 5;
            var authorIndexInArray = bookIdToLookFor - 1;
            var amountOfAuthorsForBook = 2;
            var expectedAuthors = _authorBookUtility.authorBooks.Where(authorBook => authorBook.BookId == bookIdToLookFor); 

            var response = await _client.GetAsync($"{Base}/GetBook/{bookIdToLookFor}");
            var content = await response.Content.ReadAsStringAsync();
            var book = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            book?.Authors.Count.Should().Be(amountOfAuthorsForBook);

            book.Should().NotBeNull();

            foreach (var item in book.Authors)
            {
                expectedAuthors.Should().Contain(authorBook => authorBook.AuthorId == item.Id);
                expectedAuthors.Should().Contain(authorBook => authorBook.Author.Entity.FirstName == item.FirstName);
                expectedAuthors.Should().Contain(authorBook => authorBook.Author.Entity.LastName == item.LastName);
            }
        }

        [Fact]
        public async Task SuccessfullyCreateANewBookWithNoAuthors()
        {
            var newBook = _bookUtility.NewEmptyBookToCreate();
            var serializedBook = JsonSerializer.Serialize(newBook, _jsonOptions);

            var response = await _client.PostAsync($"{Base}/CreateBook", new StringContent(serializedBook, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();
             
            BookShowDTO newBookResponse = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            newBookResponse.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created); 
            newBookResponse?.Name.Should().Be(newBook.Name);
            newBookResponse?.Authors.Should().BeEmpty();
            newBookResponse?.Description.Should().Be(newBook.Description);
            newBookResponse?.Price.Should().Be(newBook.Price);

        }

        [Fact]
        public async Task SuccessfullyCreateANewBookWithOneAuthors()
        {
            ICollection<long> existingAuthors = [1];
            var amountOfAuthors = existingAuthors.Count;
            var authorToAdd = AuthorUtility.testAuthors.Where(author => existingAuthors.First() == author.Id).First();
            var newBook = _bookUtility.NewBookToCreate(existingAuthors);
            var serializedBook = JsonSerializer.Serialize(newBook, _jsonOptions);

            var response = await _client.PostAsync($"{Base}/CreateBook", new StringContent(serializedBook, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            BookShowDTO newBookResponse = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            newBookResponse.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            newBookResponse?.Name.Should().Be(newBook.Name);
            newBookResponse?.Authors.Count.Should().Be(amountOfAuthors); 
            var author = newBookResponse?.Authors.First();
            author.Should().NotBeNull(); 
            author?.FirstName.Should().Be(authorToAdd.Entity.FirstName);
            author?.LastName.Should().Be(authorToAdd.Entity.LastName);
            author?.Id.Should().Be(authorToAdd.Id);
            newBookResponse?.Description.Should().Be(newBook.Description);
            newBookResponse?.Price.Should().Be(newBook.Price);
        }


        [Fact]
        public async Task SuccessfullyCreateANewBookWithMultipleAuthors()
        {
            ICollection<long> existingAuthors = [1,3];
            var authorsToAdd = AuthorUtility.testAuthors.Where(author => existingAuthors.Contains(author.Id));
            var newBook = _bookUtility.NewBookToCreate(existingAuthors);
            var serializedBook = JsonSerializer.Serialize(newBook, _jsonOptions);

            var response = await _client.PostAsync($"{Base}/CreateBook", new StringContent(serializedBook, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            BookShowDTO newBookResponse = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);

            newBookResponse.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            newBookResponse?.Name.Should().Be(newBook.Name);
            newBookResponse?.Authors.Count.Should().Be(2);
            foreach (var authorAdded in newBookResponse.Authors)
            {
                authorsToAdd.Should().Contain(author => author.Id == authorAdded.Id);
                authorsToAdd.Should().Contain(author => author.Entity.FirstName == authorAdded.FirstName);
                authorsToAdd.Should().Contain(author => author.Entity.LastName == authorAdded.LastName);
            }
            newBookResponse?.Description.Should().Be(newBook.Description);
            newBookResponse?.Price.Should().Be(newBook.Price);
        }

        [Fact]
        public async Task SuccessfullyCreateNewAuthorAlongsideBook()
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

            var newBook = _bookUtility.NewBookToCreate(newAuthors);
            var serializedBook = JsonSerializer.Serialize(newBook, _jsonOptions);

            var response = await _client.PostAsync($"{Base}/CreateBook", new StringContent(serializedBook, Encoding.UTF8, "application/json"));
            var content = await response.Content.ReadAsStringAsync();

            BookShowDTO? newBookResponse = JsonSerializer.Deserialize<BookShowDTO>(content, _jsonOptions);
            var allAuthors = await _authorUtility.GetAll(_context);

            newBookResponse.Should().NotBeNull();
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            newBookResponse?.Name.Should().Be(newBook.Name);
            newBookResponse?.Authors.Count.Should().Be(2);
            foreach (var newAuthor in newAuthors)
            {
                allAuthors.Should().Contain(author => author.Entity.FirstName == newAuthor.Entity.FirstName && author.Entity.LastName == newAuthor.Entity.LastName);
            }
            newBookResponse?.Description.Should().Be(newBook.Description);
            newBookResponse?.Price.Should().Be(newBook.Price);
        }
    }
}
