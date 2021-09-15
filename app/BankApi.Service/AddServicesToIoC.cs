using Microsoft.Extensions.DependencyInjection;

namespace BankApi.Service
{
    public static class AddServicesToIoC
    {
        public static void AddServices(this IServiceCollection services)
        {
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddTransient<IAccountService, AccountService>();
        }
    }
}
