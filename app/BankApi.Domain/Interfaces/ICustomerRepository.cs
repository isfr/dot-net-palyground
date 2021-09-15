using System.Threading.Tasks;

using BankApi.Domain.Entities;

namespace BankApi.Domain.Interfaces
{
    public interface ICustomerRepository
    {
        IUnitOfWork unitOfWork { get; }

        Task<Customer> GetAsync(int id);

        Task<int> InsertAsync(Customer customerToInsert);
    }
}
