using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.UseCases.Orders.CreateOrder;
using NorthwindTraders.Application.UseCases.Orders.DeleteOrder;
using NorthwindTraders.Application.UseCases.Orders.GetOrderById;
using NorthwindTraders.Application.UseCases.Orders.GetOrders;
using NorthwindTraders.Application.UseCases.Orders.UpdateOrder;

namespace NorthwindTraders.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(
    GetOrdersUseCase getOrdersUseCase,
    GetOrderByIdUseCase getOrderByIdUseCase,
    CreateOrderUseCase createOrderUseCase,
    UpdateOrderUseCase updateOrderUseCase,
    DeleteOrderUseCase deleteOrderUseCase) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var orders = await getOrdersUseCase.ExecuteAsync(cancellationToken);

        return Ok(orders);
    }

    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetByIdAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await getOrderByIdUseCase.ExecuteAsync(orderId, cancellationToken);

        return Ok(order);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await createOrderUseCase.ExecuteAsync(request, cancellationToken);

        return Created($"/api/orders/{order.OrderId}", order);
    }

    [HttpPut("{orderId:int}")]
    public async Task<IActionResult> UpdateAsync(
        int orderId,
        UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await updateOrderUseCase.ExecuteAsync(orderId, request, cancellationToken);

        return Ok(order);
    }

    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> DeleteAsync(int orderId, CancellationToken cancellationToken)
    {
        await deleteOrderUseCase.ExecuteAsync(orderId, cancellationToken);

        return NoContent();
    }
}
