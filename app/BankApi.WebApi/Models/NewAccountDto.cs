using System.ComponentModel.DataAnnotations;

using BankApi.WebApi.Attributes;

namespace BankApi.WebApi.Models
{
    public class NewAccountDto
    {
        [Required]
        [RangeCustom(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [Required]
        public int CustomerId { get; set; }
    }
}
