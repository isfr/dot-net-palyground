using System.Threading.Tasks;

using BankApi.Domain.Interfaces;
using BankApi.Infrastructure.Repositories;
using BankApi.Infrastructure.UnitTest.Helpers;

using FluentAssertions;
using FluentAssertions.Execution;

using Xunit;

namespace BankApi.Infrastructure.UnitTest
{
    public class AccountRepositoryTest
    {
        private readonly DbContextTestFactory dbContextTestFactory;


        public AccountRepositoryTest()
        {
            this.dbContextTestFactory = new DbContextTestFactory();
        }

        private IAccountRepository GetRepository()
        {
            return new AccountRepository(this.dbContextTestFactory.GetDbCotext());
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetAsync_WithValidInput_ReturnsAccount(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetAsync(accountId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Id.Should().Be(accountId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task GetAsync_WithNotValidInput_ReturnsNull(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetAsync(accountId);

            // Assert
            actualResult.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetByUserAsync_WithValidInput_ReturnsAccount(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetByUserAsync(accountId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Should().ContainSingle(_ => _.Id == accountId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task GetByUserAsync_WithNotValidInput_ReturnsEmptyList(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.GetByUserAsync(accountId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task RetrieveTransactionByAccountAsync_WithValidInput_ReturnsAccount(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.RetrieveTransactionByAccountAsync(accountId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Should().Contain(_ => _.OriginAccountId == accountId || _.DestinationAccountId == accountId);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(3000)]
        public async Task RetrieveTransactionByAccountAsync_WithInvalidInput_ReturnsEmptyList(int accountId)
        {
            // Arrange
            var subjectUnderTest = this.GetRepository();

            // Act
            var actualResult = await subjectUnderTest.RetrieveTransactionByAccountAsync(accountId);

            // Assert
            using var scope = new AssertionScope();
            actualResult.Should().NotBeNull();
            actualResult.Should().BeEmpty();
        }
    }
}
