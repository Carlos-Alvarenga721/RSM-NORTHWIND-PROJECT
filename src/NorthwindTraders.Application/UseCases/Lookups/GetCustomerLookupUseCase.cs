using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Customers;

namespace NorthwindTraders.Application.UseCases.Lookups;

public sealed class GetCustomerLookupUseCase(ICustomerRepository customerRepository)
{
    public Task<IReadOnlyList<CustomerLookupDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return customerRepository.GetLookupAsync(cancellationToken);
    }
}
