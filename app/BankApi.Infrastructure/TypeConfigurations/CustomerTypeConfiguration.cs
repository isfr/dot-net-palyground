using BankApi.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.TypeConfigurations
{
    class CustomerTypeConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.ToTable("Customers");
            builder.HasKey(ac => ac.Id);

            builder.HasMany(cus => cus.Accounts).WithOne(acc => acc.AccountOwner).HasForeignKey(ac => ac.OwnerId);
        }
    }
}
