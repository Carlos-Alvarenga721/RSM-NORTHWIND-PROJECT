using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Orders;
using NorthwindTraders.Infrastructure.Persistence.DbContext;
using NorthwindTraders.Infrastructure.Persistence.Entities;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

/// <summary>
/// Maps between Northwind EF Core entities and the application order DTOs.
/// </summary>
public sealed class OrderRepository(NorthwindDbContext dbContext) : IOrderRepository
{
    public async Task<IReadOnlyList<OrderSummaryResponse>> GetOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.Employee)
            .Include(order => order.ShipViaNavigation)
            .Include(order => order.OrderDetails)
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.OrderId)
            .ToListAsync(cancellationToken);

        return orders.Select(MapSummary).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.Employee)
            .Include(order => order.ShipViaNavigation)
            .Include(order => order.ShippingValidation)
            .Include(order => order.OrderDetails)
                .ThenInclude(detail => detail.Product)
            .FirstOrDefaultAsync(order => order.OrderId == orderId, cancellationToken);

        return order is null ? null : MapOrder(order);
    }

    public async Task<CreatedOrderReference> AddAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = new Order();
        ApplyOrderValues(order, request);
        order.ShippingValidation = MapShippingValidation(request.ShippingValidation);

        await dbContext.Orders.AddAsync(order, cancellationToken);

        return new CreatedOrderReference(() => order.OrderId);
    }

    public Task AddDetailsAsync(
        int orderId,
        IReadOnlyList<OrderDetailRequest> details,
        CancellationToken cancellationToken = default)
    {
        var orderDetails = details.Select(detail => MapDetail(detail, orderId));

        dbContext.OrderDetails.AddRange(orderDetails);

        return Task.CompletedTask;
    }

    public async Task UpdateAsync(int orderId, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(order => order.ShippingValidation)
            .Include(order => order.OrderDetails)
            .FirstAsync(order => order.OrderId == orderId, cancellationToken);

        ApplyOrderValues(order, request);
        ApplyShippingValidation(order, request.ShippingValidation);

        // ProductId is part of the detail key, so updates preserve matching rows and replace only missing products.
        var requestedProductIds = request.Details
            .Select(detail => detail.ProductId)
            .ToHashSet();

        var removedDetails = order.OrderDetails
            .Where(detail => !requestedProductIds.Contains(detail.ProductId))
            .ToList();

        dbContext.OrderDetails.RemoveRange(removedDetails);

        var existingDetailsByProductId = order.OrderDetails
            .Except(removedDetails)
            .ToDictionary(detail => detail.ProductId);

        foreach (var detail in request.Details)
        {
            if (existingDetailsByProductId.TryGetValue(detail.ProductId, out var existingDetail))
            {
                existingDetail.UnitPrice = detail.UnitPrice;
                existingDetail.Quantity = detail.Quantity;
                existingDetail.Discount = detail.Discount;
            }
            else
            {
                order.OrderDetails.Add(MapDetail(detail, orderId));
            }
        }
    }

    public async Task DeleteAsync(int orderId, CancellationToken cancellationToken = default)
    {
        var order = await dbContext.Orders
            .Include(order => order.OrderDetails)
            .FirstAsync(order => order.OrderId == orderId, cancellationToken);

        dbContext.OrderDetails.RemoveRange(order.OrderDetails);
        dbContext.Orders.Remove(order);
    }

    public async Task<bool> ExistsAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Orders
            .AnyAsync(order => order.OrderId == orderId, cancellationToken);
    }

    private static void ApplyOrderValues(Order order, CreateOrderRequest request)
    {
        order.CustomerId = request.CustomerId;
        order.EmployeeId = request.EmployeeId;
        order.OrderDate = request.OrderDate;
        order.RequiredDate = request.RequiredDate;
        order.ShippedDate = request.ShippedDate;
        order.ShipVia = request.ShipVia;
        order.Freight = request.Freight ?? 0;
        order.ShipName = request.ShipName;
        order.ShipAddress = request.ShipAddress;
        order.ShipCity = request.ShipCity;
        order.ShipRegion = request.ShipRegion;
        order.ShipPostalCode = request.ShipPostalCode;
        order.ShipCountry = request.ShipCountry;
    }

    private static void ApplyOrderValues(Order order, UpdateOrderRequest request)
    {
        order.CustomerId = request.CustomerId;
        order.EmployeeId = request.EmployeeId;
        order.OrderDate = request.OrderDate;
        order.RequiredDate = request.RequiredDate;
        order.ShippedDate = request.ShippedDate;
        order.ShipVia = request.ShipVia;
        order.Freight = request.Freight ?? 0;
        order.ShipName = request.ShipName;
        order.ShipAddress = request.ShipAddress;
        order.ShipCity = request.ShipCity;
        order.ShipRegion = request.ShipRegion;
        order.ShipPostalCode = request.ShipPostalCode;
        order.ShipCountry = request.ShipCountry;
    }

    private static OrderDetail MapDetail(OrderDetailRequest detail)
    {
        return new OrderDetail
        {
            ProductId = detail.ProductId,
            UnitPrice = detail.UnitPrice,
            Quantity = detail.Quantity,
            Discount = detail.Discount
        };
    }

    private static OrderDetail MapDetail(OrderDetailRequest detail, int orderId)
    {
        var orderDetail = MapDetail(detail);
        orderDetail.OrderId = orderId;

        return orderDetail;
    }

    private static OrderSummaryResponse MapSummary(Order order)
    {
        var freight = order.Freight ?? 0;

        return new OrderSummaryResponse(
            order.OrderId,
            order.CustomerId?.Trim() ?? string.Empty,
            order.Customer?.CompanyName,
            order.EmployeeId,
            GetEmployeeName(order.Employee),
            order.OrderDate,
            order.RequiredDate,
            order.ShippedDate,
            order.ShipVia,
            order.ShipViaNavigation?.CompanyName,
            freight,
            order.ShipCity,
            order.ShipRegion,
            order.ShipCountry,
            order.OrderDetails.Count,
            order.OrderDetails.Sum(CalculateLineTotal) + freight);
    }

    private static OrderResponse MapOrder(Order order)
    {
        var freight = order.Freight ?? 0;

        // Responses sort line items by product name for a stable detail page and PDF output.
        var details = order.OrderDetails
            .OrderBy(detail => detail.Product.ProductName)
            .Select(detail => new OrderDetailResponse(
                detail.ProductId,
                detail.Product.ProductName,
                detail.UnitPrice,
                detail.Quantity,
                detail.Discount,
                CalculateLineTotal(detail)))
            .ToList();

        return new OrderResponse(
            order.OrderId,
            order.CustomerId?.Trim() ?? string.Empty,
            order.Customer?.CompanyName,
            order.EmployeeId,
            GetEmployeeName(order.Employee),
            order.OrderDate,
            order.RequiredDate,
            order.ShippedDate,
            order.ShipVia,
            order.ShipViaNavigation?.CompanyName,
            freight,
            order.ShipName,
            order.ShipAddress,
            order.ShipCity,
            order.ShipRegion,
            order.ShipPostalCode,
            order.ShipCountry,
            MapShippingValidation(order.ShippingValidation),
            details.Sum(detail => detail.LineTotal) + freight,
            details);
    }

    private static void ApplyShippingValidation(
        Order order,
        OrderShippingValidationRequest? validationRequest)
    {
        if (validationRequest is null)
        {
            order.ShippingValidation = null;
            return;
        }

        if (order.ShippingValidation is null)
        {
            order.ShippingValidation = MapShippingValidation(validationRequest);
            return;
        }

        UpdateShippingValidation(order.ShippingValidation, validationRequest);
    }

    private static OrderShippingValidation? MapShippingValidation(OrderShippingValidationRequest? validationRequest)
    {
        if (validationRequest is null)
        {
            return null;
        }

        var validation = new OrderShippingValidation();
        UpdateShippingValidation(validation, validationRequest);

        return validation;
    }

    private static void UpdateShippingValidation(
        OrderShippingValidation validation,
        OrderShippingValidationRequest validationRequest)
    {
        validation.OriginalAddress = validationRequest.OriginalAddress;
        validation.FormattedAddress = validationRequest.FormattedAddress;
        validation.Latitude = validationRequest.Latitude;
        validation.Longitude = validationRequest.Longitude;
        validation.ValidationStatus = validationRequest.ValidationStatus;
        validation.GooglePlaceId = validationRequest.GooglePlaceId;
        validation.ValidationMessage = validationRequest.ValidationMessage;
        validation.ValidationGranularity = validationRequest.ValidationGranularity;
        validation.GeocodeGranularity = validationRequest.GeocodeGranularity;
        validation.ValidatedAtUtc = DateTime.UtcNow;
    }

    private static OrderShippingValidationResponse? MapShippingValidation(
        OrderShippingValidation? validation)
    {
        return validation is null
            ? null
            : new OrderShippingValidationResponse(
                validation.OriginalAddress,
                validation.FormattedAddress,
                validation.Latitude,
                validation.Longitude,
                validation.ValidationStatus,
                validation.GooglePlaceId,
                validation.ValidationMessage,
                validation.ValidationGranularity,
                validation.GeocodeGranularity,
                validation.ValidatedAtUtc);
    }

    private static string? GetEmployeeName(Employee? employee)
    {
        return employee is null ? null : $"{employee.FirstName} {employee.LastName}";
    }

    private static decimal CalculateLineTotal(OrderDetail detail)
    {
        return detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount);
    }
}
