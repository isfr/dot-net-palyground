using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Interfaces;

using Microsoft.EntityFrameworkCore;

namespace BankApi.Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly BankDbContext dbContext;

        public AccountRepository(BankDbContext dbContext)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public IUnitOfWork unitOfWork => this.dbContext;

        public async Task<Account> GetAsync(int id)
        {
            return await this.dbContext.Accounts.FirstOrDefaultAsync(ac => ac.Id == id);
        }

        public async Task<IEnumerable<Account>> GetByUserAsync(int ownerId)
        {
            return await this.dbContext.Accounts.Where(ac => ac.OwnerId == ownerId).ToListAsync();
        }

        public async Task<int> InsertAsync(Account accountToSave)
        {
            await this.dbContext.Accounts.AddAsync(accountToSave);
            return accountToSave.Id;
        }

        public async Task<IEnumerable<Transaction>> RetrieveTransactionByAccountAsync(int accountId)
        {
            return await this.dbContext.Transactions.Where(trx => trx.OriginAccountId == accountId || trx.DestinationAccountId == accountId).ToListAsync();
        }

        public async Task SaveTransactionAsync(Transaction transactionToSave)
        {
            await this.dbContext.Transactions.AddAsync(transactionToSave);
        }
    }
}
