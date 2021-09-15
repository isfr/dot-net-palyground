using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Interfaces;
using BankApi.Infrastructure.TypeConfigurations;

using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure
{
    public class BankDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Account> Accounts { get; set; }

        public DbSet<Transaction> Transactions { get; set; }

        public BankDbContext(DbContextOptions<BankDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AccountTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionTypeConfiguration());
        }

        public void Commit()
        {
            this.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await this.SaveChangesAsync();
        }
    }
}
