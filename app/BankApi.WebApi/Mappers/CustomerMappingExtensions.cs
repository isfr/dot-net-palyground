using BankApi.Domain.Entities;
using BankApi.WebApi.Models;

namespace BankApi.WebApi.Mappers
{
    public static class CustomerMappingExtensions
    {
        public static CustomerDto ToDto(this Customer account)
        {
            return new CustomerDto()
            {
                 CustomerId = account.Id,
                 CustomerName = account.Name
            };
        }

        public static Customer ToDomain(this CustomerDto dto)
        {
            return new Customer()
            {
                Id = dto.CustomerId,
                Name = dto.CustomerName
            };
        }
    }
}
