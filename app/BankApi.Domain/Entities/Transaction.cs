using System;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Domain.Entities
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OriginAccountId { get; set; }

        [Required]
        public int DestinationAccountId { get; set; }

        [Required]
        public decimal TransactionAmount { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
