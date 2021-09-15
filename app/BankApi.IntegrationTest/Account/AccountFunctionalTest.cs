using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using BankApi.WebApi;
using BankApi.WebApi.Models;

using FluentAssertions;
using FluentAssertions.Execution;

using Newtonsoft.Json;

using Xunit;

namespace BankApi.IntegrationTest
{
    public class AccountFunctionalTest
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        private readonly CustomWebApplicationFactory<Startup>
            _factory;

        private const string BASE_URI_PATH = "https://localhost:8080/api/Account";

        public AccountFunctionalTest(CustomWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._client = factory.CreateClient();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetAccount_WithValidAccountId_Returns200AndTheAccountInfo(int accountId)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetAccount/{accountId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);
            var actualResponseContent = JsonConvert.DeserializeObject<AccountDto>(await actualResponse.Content.ReadAsStringAsync());

            // Assert
            using var scope = new AssertionScope();
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            actualResponseContent.Should().NotBeNull();
            actualResponseContent.AccountId.Should().Be(accountId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task GetCustomerEnpoint_WithNotValidAccountId_Returns404(int accountId)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetAccount/{accountId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateNewAccount_WithValidInput_Returns200AndTheNewAccountId()
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/CreateNewAccount");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent($"{{\"Balance\":\"12345\", \"CustomerId\":\"1\"}}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateNewAccount_WithNotValidInput_Returns400(decimal balance)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/CreateNewAccount");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent($"{{\"Balance\":\"{balance}\", \"CustomerId\":\"1\"}}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Transfer_WithValidInput_Returns202()
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/Transfer");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent($"{{\"OriginAccountId\":\"1\", \"DestinationAccountId\":\"2\", \"Amount\":\"5\"}}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Accepted);
        }

        [Fact]
        public async Task Transfer_WithNotValidInput_Returns400()
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/Transfer");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent($"{{\"OriginAccountId\":\"1\", \"DestinationAccountId\":\"-2\", \"Amount\":\"-5\"}}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(4, 1)]
        [InlineData(3, 3)]
        public async Task GetTransactionHistory_WithValidAccountId_Returns200AndTheListOfTransactions(int accountId, int expectedCount)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetTransactionHistory/{accountId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);
            var actualResponseContent = JsonConvert.DeserializeObject<IEnumerable<TransactionDto>>(await actualResponse.Content.ReadAsStringAsync());

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            actualResponseContent.Should().HaveCount(c => c == expectedCount);
        }

        [Fact]
        public async Task GetTransactionHistory_WithNotValidAccountId_Returns404()
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetTransactionHistory/{5}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
