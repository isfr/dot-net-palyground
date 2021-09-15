using System.ComponentModel.DataAnnotations;

namespace BankApi.Domain.Entities
{
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public int OwnerId { get; set; }

        public virtual Customer AccountOwner { get; set; }
    }
}
