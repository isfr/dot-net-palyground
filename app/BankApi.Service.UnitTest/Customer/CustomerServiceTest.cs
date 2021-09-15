using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Exceptions;
using BankApi.Domain.Interfaces;

using FluentAssertions;

using Moq;

using Xunit;

namespace BankApi.Service.UnitTest
{
    public class CustomerServiceTest
    {
        private readonly ICustomerService subjectUnderTest;

        private readonly Mock<ICustomerRepository> customerRepositoryMock;

        private readonly Mock<IAccountRepository> accountRepositoryMock;

        private readonly Mock<IUnitOfWork> unitOfWorkMock;

        private Random rand;

        public CustomerServiceTest()
        {
            this.unitOfWorkMock = new Mock<IUnitOfWork>();
            this.customerRepositoryMock = new Mock<ICustomerRepository>();
            this.accountRepositoryMock = new Mock<IAccountRepository>();

            this.customerRepositoryMock.Setup(_ => _.unitOfWork).Returns(this.unitOfWorkMock.Object);
            this.accountRepositoryMock.Setup(_ => _.unitOfWork).Returns(this.unitOfWorkMock.Object);

            this.subjectUnderTest = new CustomerService(this.customerRepositoryMock.Object, this.accountRepositoryMock.Object);

            this.rand = new Random(DateTime.Now.Millisecond);
        }

        [Fact]
        public async Task CreateNewCustomer_WithNullOrEmptyName_ThrowsException()
        {
            // Arange
            var anyInvalidName = this.rand.Next(2) == 0 ? string.Empty : null;

            // Act
            Func<Task<int>> act = () => subjectUnderTest.CreateNewCustomer(anyInvalidName);

            // Assert
            await act.Should().ThrowAsync<BusinessException>()
                .WithMessage("The parameter customerName is mandatory");
        }

        [Fact]
        public async Task CreateNewCustomer_WithValidName_InsertsTheCustomer()
        {
            // Arange
            var anyInvalidName = "aasdasdqwe sdasdasdasd";

            // Act
            var _ = await this.subjectUnderTest.CreateNewCustomer(anyInvalidName);

            // Assert
            this.customerRepositoryMock.Verify(_ => _.InsertAsync(It.IsAny<Customer>()), Times.Once);
            this.unitOfWorkMock.Verify(_ => _.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCustomerAccounts_WithValidId_ResturnsAccounts()
        {
            // Arange
            var anyValidCustomerId = this.rand.Next(int.MaxValue);
            this.accountRepositoryMock.Setup(_ => _.GetByUserAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<Account>()
                {
                    new Account() { OwnerId = anyValidCustomerId, Id = this.rand.Next(int.MaxValue), Balance = this.rand.Next(int.MaxValue) },
                    new Account() { OwnerId = anyValidCustomerId, Id = this.rand.Next(int.MaxValue), Balance = this.rand.Next(int.MaxValue) },
                    new Account() { OwnerId = anyValidCustomerId, Id = this.rand.Next(int.MaxValue), Balance = this.rand.Next(int.MaxValue) }
                });

            // Act
            var actualResult = await this.subjectUnderTest.GetCustomerAccounts(anyValidCustomerId);

            // Assert
            actualResult.Should().HaveCount(c => c == 3);
        }

        [Fact]
        public async Task GetCustomerAccounts_WithValidIdThereIsNoData_ReturnsEmptyList()
        {
            // Arange
            var anyInvalidCustomerId = this.rand.Next(int.MaxValue);

            // Act
            var actualResult = await this.subjectUnderTest.GetCustomerAccounts(anyInvalidCustomerId);

            // Assert
            actualResult.Should().BeEmpty();
        }

        [Fact]
        public async Task GetCustomerInfo_WithValidId_ResturnsCustomerInfo()
        {
            // Arange
            var anyValidCustomerId = this.rand.Next(int.MaxValue);
            var anyCustomerName = "anyValidCustomer Name";
            this.customerRepositoryMock.Setup(_ => _.GetAsync(It.IsAny<int>()))
                .ReturnsAsync(new Customer() { Id = anyValidCustomerId, Name = anyCustomerName });

            // Act
            var actualResult = await this.subjectUnderTest.GetCustomerInfo(anyValidCustomerId);

            // Assert
            actualResult.Should().BeEquivalentTo(new Customer() { Id = anyValidCustomerId, Name = anyCustomerName });
        }

        [Fact]
        public async Task GetCustomerInfo_WithValidIdThereIsNoData_ReturnsEmptyList()
        {
            // Arange
            var anyInvalidCustomerId = this.rand.Next(int.MaxValue);

            // Act
            var actualResult = await this.subjectUnderTest.GetCustomerInfo(anyInvalidCustomerId);

            // Assert
            actualResult.Should().BeNull();
        }
    }
}
