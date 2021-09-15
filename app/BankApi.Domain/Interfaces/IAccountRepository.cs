using System.Collections.Generic;
using System.Threading.Tasks;

using BankApi.Domain.Entities;

namespace BankApi.Domain.Interfaces
{
    public interface IAccountRepository
    {
        IUnitOfWork unitOfWork { get; }

        Task<Account> GetAsync(int id);

        Task<IEnumerable<Account>> GetByUserAsync(int ownerId);

        Task<int> InsertAsync(Account accountToSave);

        Task SaveTransactionAsync(Transaction transactionToSave);

        Task<IEnumerable<Transaction>> RetrieveTransactionByAccountAsync(int accountId);
    }
}
