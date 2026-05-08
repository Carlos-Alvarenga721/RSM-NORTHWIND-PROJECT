using FluentValidation;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;

namespace NorthwindTraders.Application.UseCases.Orders.CreateOrder;

public sealed class CreateOrderUseCase(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateOrderRequest> validator)
{
    public async Task<OrderResponse> ExecuteAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        await validator.ValidateAndThrowAsync(request, cancellationToken);

        var createdOrder = await orderRepository.AddAsync(request, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return await orderRepository.GetByIdAsync(createdOrder.OrderId, cancellationToken)
            ?? throw new InvalidOperationException($"Created order {createdOrder.OrderId} could not be loaded.");
    }
}
