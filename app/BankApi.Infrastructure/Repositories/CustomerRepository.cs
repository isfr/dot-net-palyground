using System;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BankDbContext dbContext;

        public CustomerRepository(BankDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IUnitOfWork unitOfWork => this.dbContext;

        public async Task<Customer> GetAsync(int id)
        {
            return await this.dbContext.Customers.FirstOrDefaultAsync(cus => cus.Id == id);
        }

        public async Task<int> InsertAsync(Customer customerToInsert)
        {
            await this.dbContext.Customers.AddAsync(customerToInsert);
            return customerToInsert.Id;
        }
    }
}
