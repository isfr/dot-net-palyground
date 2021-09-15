using System.Threading.Tasks;

using BankApi.Domain.Interfaces;
using BankApi.Infrastructure.Repositories;
using BankApi.Infrastructure.UnitTest.Helpers;

using FluentAssertions;
using FluentAssertions.Execution;

using Xunit;

namespace BankApi.Infrastructure.UnitTest
{
    public class CustomerRepositoryTest
    {
        private readonly DbContextTestFactory dbContextTestFactory;

        public CustomerRepositoryTest()
        {
            this.dbContextTestFactory = new DbContextTestFactory();
        }

        private ICustomerRepository GetRepository()
        {
            return new CustomerRepository(this.dbContextTestFactory.GetDbCotext());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetAsync_WithValidInput_ReturnsCustomer(int cutomerId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetAsync(cutomerId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Id.Should().Be(cutomerId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task GetAsync_WithNotValidInput_ReturnsNull(int cutomerId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetAsync(cutomerId);

            // Assert
            actualResult.Should().BeNull();
        }
    }
}
