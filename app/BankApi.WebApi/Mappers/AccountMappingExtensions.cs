using BankApi.Domain.Entities;
using BankApi.WebApi.Models;

namespace BankApi.WebApi.Mappers
{
    public static class AccountMappingExtensions
    {
        public static AccountDto ToDto(this Account account)
        {
            return new AccountDto()
            {
                AccountId = account.Id,
                Balance = account.Balance.ToString("C2"),
                CustomerId = account.OwnerId
            };
        }

        public static Account ToDomain(this NewAccountDto dto)
        {
            return new Account()
            {
                Balance = dto.Balance,
                OwnerId = dto.CustomerId
            };
        }
    }
}
