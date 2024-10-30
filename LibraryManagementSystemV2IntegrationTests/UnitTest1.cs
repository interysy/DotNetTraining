using LibraryManagementSystemV2IntegrationTests;
using LibraryManagementSystemV2IntegrationTests.Utilities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Xunit.Abstractions;


namespace LibraryManagementSystemV2IntegrationTests
{
    public class TestBookServiceControllerTests : IClassFixture<IntegrationTestFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly ITestOutputHelper _outputHelper;

        public TestBookServiceControllerTests(ITestOutputHelper outputHelper)
        {
            _factory = new IntegrationTestFactory<Program>();
            _outputHelper = outputHelper;


        }

        [Fact]
        public async Task TestGetBooks()
        {
            //CreateDatabaseUtilities.CreateAndSeedDatabase(_factory);
            // Arrange
            var client = _factory.CreateClient();
            var response = await client.GetAsync("/api/TestBook/GetBooks");
            var content = await response.Content.ReadAsStringAsync();
            //var books = JsonSerializer.Deserialize<List<BookShowDTO>>(content);

            _outputHelper.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //Assert.Equal(2, books.Count);
        }
    }
}
