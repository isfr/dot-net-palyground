using System.Collections.Generic;
using System.Threading.Tasks;

using BankApi.Domain.Entities;

namespace BankApi.Service
{
    public interface IAccountService
    {
        /// <summary>
        /// Creates the new account.
        /// </summary>
        /// <param name="ownerId">The owner identifier.</param>
        /// <param name="initialAmount">The initial amount.</param>
        Task<int> CreateNewAccount(int ownerId, decimal initialAmount);

        /// <summary>
        /// Transfers the amount from an origin account to the destination account.
        /// </summary>
        /// <param name="originAccountId">The origin account identifier.</param>
        /// <param name="destinationAccountId">The destination account identifier.</param>
        /// <param name="amountToTransfer">The amount to transfer.</param>
        Task TransferAmount(int originAccountId, int destinationAccountId, decimal amountToTransfer);

        /// <summary>
        /// Gets the account information.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        Task<Account> GetAccountInfo(int accountId);

        /// <summary>
        /// Gets the account transfer history.
        /// </summary>
        /// <param name="accountId">The account identifier.</param>
        Task<IEnumerable<Transaction>> GetAccountTransferHistory(int accountId);
    }
}
