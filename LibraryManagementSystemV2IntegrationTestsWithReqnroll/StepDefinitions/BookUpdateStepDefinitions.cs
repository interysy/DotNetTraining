using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using LibraryManagementSystemV2.Contexts;
using LibraryManagementSystemV2.DTOs.NewFolder1;
using LibraryManagementSystemV2.Models;
using LibraryManagementSystemV2IntegrationTests;
using LibraryManagementSystemV2IntegrationTests.Utilities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll;
using Xunit.Abstractions;

namespace LibraryManagementSystemV2IntegrationTestsWithReqnroll.StepDefinitions
{
    [Binding]
    public class BookUpdateStepDefinitions : IClassFixture<IntegrationTestFactory<Program>>
    {

        private BookCreateDTO? _bookToUpdate;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
        private readonly BookUtility _bookUtility;
        private readonly AuthorUtility _authorUtility;
        private readonly AuthorBookUtility _authorBookUtility;
        private readonly LibraryManagementContext _context; 
        private readonly IntegrationTestFactory<Program> _factory;
        private HttpClient _client;
         
        private string _responseContent;
        private HttpResponseMessage _response;
        private readonly String Base = "/api/TestBook";
        private readonly ITestOutputHelper _outputHelper;
        long _bookToUpdateId; 


        public BookUpdateStepDefinitions(ITestOutputHelper outputHelper)
        {
            _factory = new IntegrationTestFactory<Program>(); 



            _bookUtility = new BookUtility(); 
            _authorUtility = new AuthorUtility();
            _authorBookUtility = new AuthorBookUtility(); 


            _client = _factory.CreateClient();
            var scope = _factory.Services.CreateScope();
            var scopedServices = scope.ServiceProvider;
            _context = scopedServices.GetRequiredService<LibraryManagementContext>();
            DatabaseUtilities.RefreshDatabase(_context); 

            _outputHelper = outputHelper;
        }

        [Given("the user wants to update book {long} with no authors and provides {string}, {string}, {float}, {int}")]
        public async Task GivenTheUserWantsToUpdateBookWithNoAuthorsAndProvides(long id, string name, string description, float price, int quantity)
        {
            await _bookUtility.SeedData(_context);
            _bookToUpdateId = id;

            _bookToUpdate = new()
            {
                Name = name,
                Description = description,
                Price = price,
                Quantity = quantity,
                AuthorIDs = new List<long>(), 
                NewAuthors = []
            };

            _outputHelper.WriteLine(_context.Books.Count().ToString());
        }

        [When("the book is updated")]
        public async Task WhenTheBookIsUpdated()
        {
            var serializedBook = JsonSerializer.Serialize(_bookToUpdate, _jsonOptions);

            _response = await _client.PutAsync($"{Base}/UpdateBook?id={_bookToUpdateId}", new StringContent(serializedBook, Encoding.UTF8, "application/json"));
            _responseContent = await _response.Content.ReadAsStringAsync();

            //Book? resultantBook = await BookUtility.GetAdded(_context, _bookToUpdateId);

            //_outputHelper.WriteLine(resultantBook.Name);
            //_outputHelper.WriteLine(resultantBook.Description);
            //_outputHelper.WriteLine(resultantBook.Price.ToString());
            //_outputHelper.WriteLine(resultantBook.Quantity.ToString());


        }

        [Then("the book properties should be updated in the database")]
        public async Task ThenTheBookPropertiesShouldBeUpdatedInTheDatabase()
        {


            //System.Threading.Thread.Sleep(5000);
            Assert.Equal(HttpStatusCode.NoContent, _response.StatusCode);

            //Book? resultantBook = await BookUtility.GetAdded(_context, _bookToUpdateId);


            //var scope = _factory.Services.CreateScope();
            //var scopedServices = scope.ServiceProvider;
            //var context = scopedServices.GetRequiredService<LibraryManagementContext>(); 

            Book? resultantBook = await _context.Books.FindAsync(_bookToUpdateId);

            _outputHelper.WriteLine(_context.Books.ToList().Count.ToString());

            _outputHelper.WriteLine(resultantBook.Name);
            _outputHelper.WriteLine(resultantBook.Description);
            _outputHelper.WriteLine(resultantBook.Price.ToString());
            _outputHelper.WriteLine(resultantBook.Quantity.ToString());
             


            Assert.NotNull(resultantBook);
            Assert.Equal(_bookToUpdate.Name, resultantBook.Name);
            Assert.Equal(_bookToUpdate.Description, resultantBook.Description);
            Assert.Equal(_bookToUpdate.Price, resultantBook.Price);
            Assert.Equal(_bookToUpdate.Quantity, resultantBook.Quantity); 
        }
    }
}
