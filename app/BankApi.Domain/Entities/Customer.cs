using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BankApi.Domain.Entities
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
