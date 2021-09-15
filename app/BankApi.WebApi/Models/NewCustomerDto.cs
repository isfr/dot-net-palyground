using System.ComponentModel.DataAnnotations;

namespace BankApi.WebApi.Models
{
    public class NewCustomerDto
    {
        [Required]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "The field {0} length must be between {1} and {2}")]
        public string CustomerName { get; set; }
    }
}
