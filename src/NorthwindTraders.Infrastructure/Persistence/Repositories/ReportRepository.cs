using System.Globalization;
using Microsoft.EntityFrameworkCore;
using NorthwindTraders.Application.Abstractions.Persistence;
using NorthwindTraders.Application.DTOs.Reports;
using NorthwindTraders.Infrastructure.Persistence.DbContext;
using NorthwindTraders.Infrastructure.Persistence.Entities;

namespace NorthwindTraders.Infrastructure.Persistence.Repositories;

/// <summary>
/// Builds the reporting read model from Northwind orders, customers, employees, and line totals.
/// </summary>
public sealed class ReportRepository(NorthwindDbContext dbContext) : IReportRepository
{
    public async Task<OrdersReportResponse> GetOrdersReportAsync(
        ReportFilterRequest filters,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = dbContext.Orders
            .AsNoTracking()
            .Include(order => order.Customer)
            .Include(order => order.Employee)
            .Include(order => order.OrderDetails);

        query = ApplyDatabaseFilters(query, filters);

        var orders = await query
            .ToListAsync(cancellationToken);

        // ISO week filtering uses .NET calendar logic, so it is applied after the database query.
        var filteredOrders = ApplyInMemoryFilters(orders, filters)
            .OrderByDescending(order => order.OrderDate)
            .ThenByDescending(order => order.OrderId)
            .ToList();

        var rows = filteredOrders
            .Select(MapOrderRow)
            .ToList();

        var ordersOverTime = filteredOrders
            .Where(order => order.OrderDate.HasValue)
            .GroupBy(order => new
            {
                order.OrderDate!.Value.Year,
                order.OrderDate.Value.Month
            })
            .OrderBy(group => group.Key.Year)
            .ThenBy(group => group.Key.Month)
            .Select(group => new OrdersOverTimePoint(
                $"{CultureInfo.InvariantCulture.DateTimeFormat.GetAbbreviatedMonthName(group.Key.Month)} {group.Key.Year}",
                group.Count(),
                group.Sum(CalculateOrderTotal)))
            .ToList();

        var shipmentsByRegion = filteredOrders
            .GroupBy(order => string.IsNullOrWhiteSpace(order.ShipRegion) ? "Unassigned" : order.ShipRegion.Trim())
            .OrderByDescending(group => group.Count())
            .ThenBy(group => group.Key)
            .Select(group => new ShipmentsByRegionPoint(group.Key, group.Count()))
            .ToList();

        return new OrdersReportResponse(ordersOverTime, shipmentsByRegion, rows);
    }

    private static IQueryable<Order> ApplyDatabaseFilters(
        IQueryable<Order> orders,
        ReportFilterRequest filters)
    {
        var filteredOrders = orders;

        if (filters.Year.HasValue)
        {
            filteredOrders = filteredOrders.Where(order =>
                order.OrderDate.HasValue && order.OrderDate.Value.Year == filters.Year.Value);
        }

        if (filters.Month.HasValue)
        {
            filteredOrders = filteredOrders.Where(order =>
                order.OrderDate.HasValue && order.OrderDate.Value.Month == filters.Month.Value);
        }

        var region = filters.Region?.Trim();
        if (!string.IsNullOrWhiteSpace(region))
        {
            filteredOrders = filteredOrders.Where(order =>
                order.ShipRegion != null && order.ShipRegion.Contains(region));
        }

        return filteredOrders;
    }

    private static IEnumerable<Order> ApplyInMemoryFilters(
        IEnumerable<Order> orders,
        ReportFilterRequest filters)
    {
        var filteredOrders = orders;

        if (filters.Week.HasValue)
        {
            filteredOrders = filteredOrders.Where(order =>
                order.OrderDate.HasValue &&
                ISOWeek.GetWeekOfYear(order.OrderDate.Value) == filters.Week.Value);
        }

        return filteredOrders;
    }

    private static ReportOrderRow MapOrderRow(Order order)
    {
        return new ReportOrderRow(
            order.OrderId,
            order.Customer?.CompanyName,
            GetEmployeeName(order.Employee),
            order.OrderDate,
            order.ShippedDate,
            order.ShipRegion,
            order.ShipCountry,
            CalculateOrderTotal(order));
    }

    private static string? GetEmployeeName(Employee? employee)
    {
        return employee is null ? null : $"{employee.FirstName} {employee.LastName}";
    }

    private static decimal CalculateOrderTotal(Order order)
    {
        return order.OrderDetails.Sum(detail =>
            detail.UnitPrice * detail.Quantity * (1 - (decimal)detail.Discount)) + (order.Freight ?? 0);
    }
}
