using Microsoft.AspNetCore.Mvc;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Application.UseCases.Orders.CreateOrder;
using NorthwindTraders.Application.UseCases.Orders.DeleteOrder;
using NorthwindTraders.Application.UseCases.Orders.GenerateOrderPdf;
using NorthwindTraders.Application.UseCases.Orders.GetOrderById;
using NorthwindTraders.Application.UseCases.Orders.GetOrders;
using NorthwindTraders.Application.UseCases.Orders.UpdateOrder;

namespace NorthwindTraders.Api.Controllers;

/// <summary>
/// HTTP entry point for the order workflow; all business decisions stay in application use cases.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(
    GetOrdersUseCase getOrdersUseCase,
    GetOrderByIdUseCase getOrderByIdUseCase,
    CreateOrderUseCase createOrderUseCase,
    UpdateOrderUseCase updateOrderUseCase,
    DeleteOrderUseCase deleteOrderUseCase,
    GenerateOrderPdfUseCase generateOrderPdfUseCase) : ControllerBase
{
    /// <summary>
    /// Returns the lightweight list used by the orders page.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var orders = await getOrdersUseCase.ExecuteAsync(cancellationToken);

        return Ok(orders);
    }

    /// <summary>
    /// Returns one order with line items and saved shipping validation metadata.
    /// </summary>
    [HttpGet("{orderId:int}")]
    public async Task<IActionResult> GetByIdAsync(int orderId, CancellationToken cancellationToken)
    {
        var order = await getOrderByIdUseCase.ExecuteAsync(orderId, cancellationToken);

        return Ok(order);
    }

    /// <summary>
    /// Creates an order through the application layer and exposes the created resource location.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await createOrderUseCase.ExecuteAsync(request, cancellationToken);

        return Created($"/api/orders/{order.OrderId}", order);
    }

    /// <summary>
    /// Updates the order header, shipping validation metadata, and product line items.
    /// </summary>
    [HttpPut("{orderId:int}")]
    public async Task<IActionResult> UpdateAsync(
        int orderId,
        UpdateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await updateOrderUseCase.ExecuteAsync(orderId, request, cancellationToken);

        return Ok(order);
    }

    /// <summary>
    /// Deletes an order and its details through the repository-backed use case.
    /// </summary>
    [HttpDelete("{orderId:int}")]
    public async Task<IActionResult> DeleteAsync(int orderId, CancellationToken cancellationToken)
    {
        await deleteOrderUseCase.ExecuteAsync(orderId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Generates the branded PDF that is downloaded from the order detail page.
    /// </summary>
    [HttpGet("{orderId:int}/report/pdf")]
    public async Task<IActionResult> GeneratePdfAsync(int orderId, CancellationToken cancellationToken)
    {
        var pdf = await generateOrderPdfUseCase.ExecuteAsync(orderId, cancellationToken);

        return File(pdf, "application/pdf", $"northwind-order-{orderId}.pdf");
    }
}
