using System;

using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.UnitTest.Helpers
{
    public class DbContextTestFactory
    {
        private readonly DbContextOptions<BankDbContext> options;

        public DbContextTestFactory()
        {
            this.options = new DbContextOptionsBuilder<BankDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        }

        public BankDbContext GetDbCotext()
        {
            var context = new BankDbContext(this.options);
            context.Accounts.AddRange(SampleRepositoryData.GetAccounts());
            context.Customers.AddRange(SampleRepositoryData.GetCustomers());
            context.Transactions.AddRange(SampleRepositoryData.GetTransactions());
            context.SaveChanges();
            return context;
        }
    }
}
