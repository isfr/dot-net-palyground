using System;

namespace BankApi.WebApi.Models
{
    public class TransactionDto
    {
        public int OriginAccountId { get; set; }

        public int DestinationAccountId { get; set; }

        public decimal AmountTransfered { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
