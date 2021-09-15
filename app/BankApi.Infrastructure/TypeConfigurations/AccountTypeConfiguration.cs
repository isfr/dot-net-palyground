using BankApi.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BankApi.Infrastructure.TypeConfigurations
{
    public class AccountTypeConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.ToTable("Accounts");
            builder.HasKey(ac => ac.Id);

            builder.HasOne(ac => ac.AccountOwner).WithMany(cus => cus.Accounts).HasForeignKey(ac => ac.OwnerId);
        }
    }
}
