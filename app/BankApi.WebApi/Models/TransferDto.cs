using System.ComponentModel.DataAnnotations;

using BankApi.WebApi.Attributes;

namespace BankApi.WebApi.Models
{
    public class TransferDto
    {
        [Required]
        public int OriginAccountId { get; set; }

        [Required]
        public int DestinationAccountId { get; set; }

        [Required]
        [RangeCustom(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Amount { get; set; }
    }
}
