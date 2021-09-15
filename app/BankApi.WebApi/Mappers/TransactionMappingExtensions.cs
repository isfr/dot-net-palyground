using BankApi.Domain.Entities;
using BankApi.WebApi.Models;

namespace BankApi.WebApi.Mappers
{
    public static class TransactionMappingExtensions
    {
        public static TransactionDto ToDto(this Transaction trx)
        {
            return new TransactionDto()
            {
                OriginAccountId = trx.OriginAccountId,
                DestinationAccountId = trx.DestinationAccountId,
                AmountTransfered = trx.TransactionAmount,
                Timestamp = trx.Timestamp
            };
        }

        public static Transaction ToDomain(this TransactionDto dto)
        {
            return new Transaction()
            {
                OriginAccountId = dto.OriginAccountId,
                DestinationAccountId = dto.DestinationAccountId,
                TransactionAmount = dto.AmountTransfered,
                Timestamp = dto.Timestamp
            };
        }
    }
}
