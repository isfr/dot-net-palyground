using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Exceptions;
using BankApi.Domain.Interfaces;

using FluentAssertions;

using Moq;

using Xunit;

namespace BankApi.Service.UnitTest
{
    public class AccountServiceTest
    {
        private readonly IAccountService subjectUnderTest;

        private readonly Mock<ICustomerRepository> customerRepositoryMock;

        private readonly Mock<IAccountRepository> accountRepositoryMock;

        private readonly Mock<IUnitOfWork> unitOfWorkMock;

        private Random rand;

        public AccountServiceTest()
        {
            this.unitOfWorkMock = new Mock<IUnitOfWork>();
            this.customerRepositoryMock = new Mock<ICustomerRepository>();
            this.accountRepositoryMock = new Mock<IAccountRepository>();

            this.customerRepositoryMock.Setup(_ => _.unitOfWork).Returns(this.unitOfWorkMock.Object);
            this.accountRepositoryMock.Setup(_ => _.unitOfWork).Returns(this.unitOfWorkMock.Object);

            this.subjectUnderTest = new AccountService(this.accountRepositoryMock.Object, this.customerRepositoryMock.Object);

            this.rand = new Random(DateTime.Now.Millisecond);
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(99.999)]
        [InlineData(100000000000)]
        public async Task CreateNewAccount_WithValidData_InsertsNewAccount(decimal anyAmount)
        {
            // Arrange
            var anyValidOwnerId = this.rand.Next(int.MaxValue);
            this.customerRepositoryMock.Setup(_ => _.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Customer() { Id = anyValidOwnerId });

            // Act
            var actualResult = await this.subjectUnderTest.CreateNewAccount(anyValidOwnerId, anyAmount);

            // Assert
            this.accountRepositoryMock.Verify(_ => _.InsertAsync(It.IsAny<Account>()), Times.Once);
            this.unitOfWorkMock.Verify(_ => _.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateNewAccount_WithInvalidCustomerId_ThrowsException()
        {
            // Arrange
            var anyInvalidOwnerId = this.rand.Next(int.MaxValue);
            var anyAmount = this.rand.Next(int.MaxValue);

            // Act
            Func<Task<int>> act = () => this.subjectUnderTest.CreateNewAccount(anyInvalidOwnerId, anyAmount);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("The customer does not exist");
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task CreateNewAccount_WithInvalidAmount_ThrowsException(decimal anyInvalidAmount)
        {
            // Arrange
            var anyValidOwnerId = this.rand.Next(int.MaxValue);
            this.customerRepositoryMock.Setup(_ => _.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Customer() { Id = anyValidOwnerId });

            // Act
            Func<Task<int>> act = () => this.subjectUnderTest.CreateNewAccount(anyValidOwnerId, anyInvalidAmount);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("The amount to transfer must be greater than 0");
        }

        [Fact]
        public async Task GetAccountInfo_WithValidAccount_ReturnsTheAccountInfo()
        {
            // Arrange
            var anyValidAccountId = this.rand.Next(int.MaxValue);
            var anyAccountReturned = new Account() { Id = anyValidAccountId, OwnerId = this.rand.Next(int.MaxValue), Balance = this.rand.Next(int.MaxValue) };
            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(anyAccountReturned);

            // Act
            var actualRestult = await this.subjectUnderTest.GetAccountInfo(anyValidAccountId);

            // Assert
            actualRestult.Should().BeEquivalentTo(anyAccountReturned);
        }

        [Fact]
        public async Task GetAccountInfo_WithInvalidAccount_ReturnsNull()
        {
            // Arrange
            var anyValidAccountId = this.rand.Next(int.MaxValue);

            // Act
            var actualRestult = await this.subjectUnderTest.GetAccountInfo(anyValidAccountId);

            // Assert
            actualRestult.Should().BeNull();
        }

        [Fact]
        public async Task GetAccountTransferHistory_WithValidAccount_ReturnsTransactionList()
        {
            // Arrange
            var anyValidAccountId = this.rand.Next(1, 100);
            this.accountRepositoryMock.Setup(_ => _.RetrieveTransactionByAccountAsync(It.IsAny<int>()))
                .ReturnsAsync((int accountId) => {
                    return new List<Transaction>()
                    {
                        new Transaction() { Id = this.rand.Next(int.MaxValue), OriginAccountId = anyValidAccountId, DestinationAccountId = this.rand.Next(100, int.MaxValue), TransactionAmount = this.rand.Next(int.MaxValue) },
                        new Transaction() { Id = this.rand.Next(int.MaxValue), OriginAccountId = this.rand.Next(100, int.MaxValue), DestinationAccountId = anyValidAccountId, TransactionAmount = this.rand.Next(int.MaxValue) },
                        new Transaction() { Id = this.rand.Next(int.MaxValue), OriginAccountId = this.rand.Next(100, int.MaxValue), DestinationAccountId = anyValidAccountId, TransactionAmount = this.rand.Next(int.MaxValue) },
                        new Transaction() { Id = this.rand.Next(int.MaxValue), OriginAccountId = this.rand.Next(100, int.MaxValue), DestinationAccountId = anyValidAccountId * -1, TransactionAmount = this.rand.Next(int.MaxValue) },
                    }.Where(t => t.OriginAccountId == accountId || t.DestinationAccountId == accountId); });

            // Act
            var actualRestult = await this.subjectUnderTest.GetAccountTransferHistory(anyValidAccountId);

            // Assert
            actualRestult.Should().HaveCount(c => c == 3);
        }

        [Fact]
        public async Task GetAccountTransferHistory_WithInvalidAccount_ReturnsEmptyList()
        {
            // Arrange
            var anyValidAccountId = this.rand.Next(int.MaxValue);

            // Act
            var actualRestult = await this.subjectUnderTest.GetAccountTransferHistory(anyValidAccountId);

            // Assert
            actualRestult.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0.01)]
        [InlineData(100)]
        [InlineData(99.999)]
        [InlineData(100000000000)]
        public async Task TransferAmount_WithValidData_DoTransferAndSaveTransaction(decimal amountToTransfer)
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidOriginAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidOriginAccountId, Balance = amountToTransfer + 1 });
            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidDestinationAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidDestinationAccountId, Balance = amountToTransfer + 1 });

            // Act
            await this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, amountToTransfer);

            // Assert
            this.accountRepositoryMock.Verify(_ => _.SaveTransactionAsync(It.IsAny<Transaction>()), Times.Once);
            this.unitOfWorkMock.Verify(_ => _.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task TransferAmount_WithSameOriginAndDestination_ThrowsException()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = anyValidOriginAccountId;
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Contains("The origin account can not be the same that destination account"));
        }

        [Fact]
        public async Task TransferAmount_WithNotValidOriginAccount_ThrowsException()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidDestinationAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidDestinationAccountId, Balance = anyAmountToTransfer });

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Contains("The origin account does not exist"));
        }

        [Fact]
        public async Task TransferAmount_WithNotValidDestinationAccount_ThrowsException()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidOriginAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidDestinationAccountId, Balance = anyAmountToTransfer });

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Contains("The destination account does not exist"));
        }

        [Fact]
        public async Task TransferAmount_WithNotValidOriginAccountBalance_ThrowsException()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidOriginAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidOriginAccountId, Balance = anyAmountToTransfer - 1 });
            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidDestinationAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidDestinationAccountId, Balance = anyAmountToTransfer });

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Contains("The origin account does not have enough funds to do the transfer"));
        }

        [Fact]
        public async Task TransferAmount_WithNotValidAmountToTransfer_ThrowsException()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidOriginAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidOriginAccountId, Balance = anyAmountToTransfer });
            this.accountRepositoryMock.Setup(_ => _.GetAsync(It.Is((int input) => input == anyValidDestinationAccountId)))
                .ReturnsAsync(new Account() { Id = anyValidDestinationAccountId, Balance = anyAmountToTransfer });

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer - anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Contains("The amount to transfer must be greater than 0"));
        }

        [Fact]
        public async Task TransferAmount_WithNotValidInputData_ThrowsExceptionWithMultipleMessages()
        {
            // Arrange
            var anyValidOriginAccountId = this.rand.Next(int.MaxValue);
            var anyValidDestinationAccountId = this.rand.Next(int.MaxValue);
            var anyAmountToTransfer = this.rand.Next(int.MaxValue);

            // Act
            Func<Task> act = () => this.subjectUnderTest.TransferAmount(anyValidOriginAccountId, anyValidDestinationAccountId, anyAmountToTransfer - anyAmountToTransfer);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .Where(bex => bex.Messages.Count() == 3);
        }
    }
}
