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
    public class CustomerFunctionalTest
        : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        private readonly CustomWebApplicationFactory<Startup>
            _factory;

        private const string BASE_URI_PATH = "https://localhost:8080/api/Customer";

        public CustomerFunctionalTest(CustomWebApplicationFactory<Startup> factory)
        {
            this._factory = factory;
            this._client = factory.CreateClient();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetCustomerEnpoint_WithValidCustomerId_Returns200AndTheCustomerInfo(int customerId)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetCustomer/{customerId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);
            var actualResponseContent = JsonConvert.DeserializeObject<CustomerDto>(await actualResponse.Content.ReadAsStringAsync());

            // Assert
            using var scope = new AssertionScope();
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            actualResponseContent.Should().NotBeNull();
            actualResponseContent.CustomerId.Should().Be(customerId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task GetCustomerEnpoint_WithNotValidCustomerId_Returns404(int customerId)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetCustomer/{customerId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateNewCustomer_WithValidInput_Returns200AndTheNewCustomerId()
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/CreateNewCustomer");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent("{\"CustomerName\":\"John Doe\"}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("thisnameisverylargeandprobablyshouldnotbevalidtobeinsertedinthedatabaseihopesobutithinkthatitisstilltoosmall")]
        public async Task CreateNewCustomer_WithNotValidInput_Returns400(string customerName)
        {
            // Arrange
            var uriToTest = new System.Uri(BASE_URI_PATH + "/CreateNewCustomer");
            var anyHttpRequestMessages = new HttpRequestMessage()
            {
                RequestUri = uriToTest,
                Content = new StringContent($"{{\"CustomerName\":\"{customerName}\"}}", Encoding.UTF8, "application/json"),
                Method = HttpMethod.Post
            };

            // Act
            var actualResponse = await _client.SendAsync(anyHttpRequestMessages);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCustomerAccounts_WithValidCustomerId_Returns200AndTheListOfAccounts()
        {
            // Arrange
            var anyValidCustomerId = 1;
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetCustomerAccounts/{anyValidCustomerId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);
            var actualResponseContent = JsonConvert.DeserializeObject<IEnumerable<AccountDto>>(await actualResponse.Content.ReadAsStringAsync());

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            actualResponseContent.Should().HaveCount(c => c == 1);
        }

        [Fact]
        public async Task GetCustomerAccounts_WithNotValidCustomerId_Returns404()
        {
            // Arrange
            var anyNotValidCustomerId = 10;
            var uriToTest = new System.Uri(BASE_URI_PATH + $"/GetCustomerAccounts/{anyNotValidCustomerId}");

            // Act
            var actualResponse = await _client.GetAsync(uriToTest);

            // Assert
            actualResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
