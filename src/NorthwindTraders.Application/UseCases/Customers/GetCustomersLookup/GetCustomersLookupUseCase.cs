using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Customers;

namespace NorthwindTraders.Application.UseCases.Customers.GetCustomersLookup;

public sealed class GetCustomersLookupUseCase(ICustomerRepository customerRepository)
{
    public Task<IReadOnlyList<CustomerLookupResponse>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        return customerRepository.GetCustomersLookupAsync(cancellationToken);
    }
}
