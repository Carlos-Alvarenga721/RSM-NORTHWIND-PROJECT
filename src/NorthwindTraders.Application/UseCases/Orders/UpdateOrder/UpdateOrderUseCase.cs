using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.Common.Exceptions;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.UseCases.Orders.UpdateOrder;

public sealed class UpdateOrderUseCase(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateOrderRequest> validator)
{
    public async Task<OrderResponse> ExecuteAsync(int orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
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
