using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using BankApi.Domain.Entities;
using BankApi.Domain.Exceptions;
using BankApi.Domain.Interfaces;

namespace BankApi.Service
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository accountRepository;

        private readonly ICustomerRepository customerRepository;

        public AccountService(IAccountRepository accountRepository, ICustomerRepository customerRepository)
        {
            this.accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
            this.customerRepository = customerRepository ?? throw new ArgumentNullException(nameof(customerRepository));
        }

        public async Task<int> CreateNewAccount(int ownerId, decimal initialAmount)
        {
            if (await this.customerRepository.GetAsync(ownerId) is null)
            {
                throw new BusinessException("The customer does not exist");
            }

            if (initialAmount <= 0)
            {
                throw new BusinessException("The amount to transfer must be greater than 0");
            }

            var accountToAdd = new Account()
            {
                OwnerId = ownerId,
                Balance = initialAmount
            };

            await this.accountRepository.InsertAsync(accountToAdd);

            await this.accountRepository.unitOfWork.CommitAsync();

            return accountToAdd.Id;
        }

        public async Task<Account> GetAccountInfo(int accountId)
        {
            return await this.accountRepository.GetAsync(accountId);
        }

        public async Task<IEnumerable<Transaction>> GetAccountTransferHistory(int accountId)
        {
            return await this.accountRepository.RetrieveTransactionByAccountAsync(accountId);
        }

        public async Task TransferAmount(int originAccountId, int destinationAccountId, decimal amountToTransfer)
        {
            if (originAccountId == destinationAccountId)
            {
                throw new BusinessException("The origin account can not be the same that destination account");
            }

            var originAccount = await this.accountRepository.GetAsync(originAccountId);
            var destinationAccount = await this.accountRepository.GetAsync(destinationAccountId);

            var (canTransfer, errorMessages) = this.CanTransfer(originAccount, destinationAccount, amountToTransfer);

            if (!canTransfer)
            {
                throw new BusinessException(errorMessages);
            }

            destinationAccount.Balance += amountToTransfer;
            originAccount.Balance -= amountToTransfer;

            await this.accountRepository.SaveTransactionAsync(new Transaction() { OriginAccountId = originAccountId, DestinationAccountId = destinationAccountId, TransactionAmount = amountToTransfer, Timestamp = DateTime.Now });

            await this.accountRepository.unitOfWork.CommitAsync();
        }

        private (bool, IEnumerable<string>) CanTransfer(Account originAccount, Account destinationAccount, decimal amountToTransfer)
        {
            var errorMessages = new List<string>();

            if (originAccount is null)
            {
                errorMessages.Add("The origin account does not exist");
            }

            if (destinationAccount is null)
            {
                errorMessages.Add("The destination account does not exist");
            }

            if (originAccount?.Balance < amountToTransfer)
            {
                errorMessages.Add("The origin account does not have enough funds to do the transfer");
            }

            if (amountToTransfer <= decimal.Zero)
            {
                errorMessages.Add("The amount to transfer must be greater than 0");
            }

            return (!errorMessages.Any(), errorMessages);
        }
    }
}
