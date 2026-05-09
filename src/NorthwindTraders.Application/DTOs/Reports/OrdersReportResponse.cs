namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record OrdersReportResponse(
    IReadOnlyList<OrdersOverTimePoint> OrdersOverTime,
    IReadOnlyList<ShipmentsByRegionPoint> ShipmentsByRegion,
    IReadOnlyList<ReportOrderRow> Orders)
{
    public static OrdersReportResponse Empty { get; } = new(
        Array.Empty<OrdersOverTimePoint>(),
        Array.Empty<ShipmentsByRegionPoint>(),
        Array.Empty<ReportOrderRow>());

    public OrdersReportResponse Normalize()
    {
        if (OrdersOverTime is not null && ShipmentsByRegion is not null && Orders is not null)
        {
            return this;
        }

        return new OrdersReportResponse(
            OrdersOverTime ?? Array.Empty<OrdersOverTimePoint>(),
            ShipmentsByRegion ?? Array.Empty<ShipmentsByRegionPoint>(),
            Orders ?? Array.Empty<ReportOrderRow>());
    }
}
