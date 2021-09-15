using System.Collections.Generic;
using System.Threading.Tasks;

using BankApi.Domain.Entities;

namespace BankApi.Service
{
    public interface ICustomerService
    {
        /// <summary>
        /// Creates the new customer.
        /// </summary>
        /// <param name="customerName">Name of the customer.</param>
        Task<int> CreateNewCustomer(string customerName);

        /// <summary>
        /// Gets the customer information.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        Task<Customer> GetCustomerInfo(int customerId);

        /// <summary>
        /// Gets the customer accounts.
        /// </summary>
        /// <param name="customerId">The customer identifier.</param>
        Task<IEnumerable<Account>> GetCustomerAccounts(int customerId);
    }
}
