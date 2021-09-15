using BankApi.Domain.Interfaces;
using BankApi.Infrastructure.Repositories;

using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Infrastructure
{
    public static class AddRepositoriesToIoC
    {
        public static void AddEntityFrameworkRepositories(this IServiceCollection services)
        {
            services.AddTransient<ICustomerRepository, CustomerRepository>();
            services.AddTransient<IAccountRepository, AccountRepository>();
        }
    }
}
