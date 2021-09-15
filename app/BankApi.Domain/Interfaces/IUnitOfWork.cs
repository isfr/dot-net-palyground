using System.Threading.Tasks;

namespace BankApi.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        void Commit();

        Task CommitAsync();
    }
}
