using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Common.Exceptions;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.UseCases.Orders.UpdateOrder;

/// <summary>
/// Updates an existing order after confirming the route id points to a persisted Northwind order.
/// </summary>
public sealed class UpdateOrderUseCase(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateOrderRequest> validator)
{
    public async Task<OrderResponse> ExecuteAsync(int orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        // Existence is checked first so missing orders are reported as 404 instead of validation or EF failures.
        if (!await orderRepository.ExistsAsync(orderId, cancellationToken))
        {
            throw new NotFoundException($"Order {orderId} was not found.");
        }

        await validator.ValidateAndThrowAsync(request, cancellationToken);

        await orderRepository.UpdateAsync(orderId, request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await orderRepository.GetByIdAsync(orderId, cancellationToken)
            ?? throw new NotFoundException($"Order {orderId} was not found.");
    }
}
