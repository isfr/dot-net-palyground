using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Exceptions;
using BankApi.Domain.Interfaces;

namespace BankApi.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository customerRepository;

        private readonly IAccountRepository accountRepository;

        public CustomerService(ICustomerRepository customerRepository, IAccountRepository accountRepository)
        {
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        }

        public async Task<int> CreateNewCustomer(string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName))
            {
                throw new BusinessException($"The parameter {nameof(customerName)} is mandatory");
            }

            var customerToInsert = new Customer() { Name = customerName.Trim() };

            await this.customerRepository.InsertAsync(customerToInsert);

            await this.customerRepository.unitOfWork.CommitAsync();

            return customerToInsert.Id;
        }

        public async Task<IEnumerable<Account>> GetCustomerAccounts(int customerId)
        {
            return await this.accountRepository.GetByUserAsync(customerId);
        }

        public async Task<Customer> GetCustomerInfo(int customerId)
        {
            return await this.customerRepository.GetAsync(customerId);
        }
    }
}
