using System.Globalization;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.Orders;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NorthwindTraders.Infrastructure.Services.Reports;

public sealed class QuestPdfOrderReportService : IOrderPdfReportService
{
    public byte[] GenerateOrderDetailsPdf(OrderResponse order)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(36);
                page.DefaultTextStyle(text => text.FontSize(10));

                page.Header().Element(header => ComposeHeader(header, order));
                page.Content().Element(content => ComposeContent(content, order));
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Northwind Traders Order Report - Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, OrderResponse order)
    {
        container.Column(column =>
        {
            column.Item().Text("Northwind Traders")
                .FontSize(20)
                .Bold()
                .FontColor(Colors.Blue.Darken3);
            column.Item().Text($"Order {order.OrderId}")
                .FontSize(14)
                .SemiBold();
            column.Item().PaddingTop(4).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeContent(IContainer container, OrderResponse order)
    {
        container.PaddingTop(16).Column(column =>
        {
            column.Spacing(16);
            column.Item().Element(section => ComposeOrderInformation(section, order));
            column.Item().Element(section => ComposeShippingAddress(section, order));
            column.Item().Element(section => ComposeLineItems(section, order));
            column.Item().AlignRight().Text($"Order Total: {FormatCurrency(order.OrderTotal)}")
                .FontSize(14)
                .Bold();
        });
    }

    private static void ComposeOrderInformation(IContainer container, OrderResponse order)
    {
        container.Column(column =>
        {
            column.Item().Text("Order Information").FontSize(13).Bold();
            column.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddInfoRow(table, "Customer", order.CustomerName ?? order.CustomerId);
                AddInfoRow(table, "Employee", order.EmployeeName ?? "Not assigned");
                AddInfoRow(table, "Order Date", FormatDate(order.OrderDate));
                AddInfoRow(table, "Required Date", FormatDate(order.RequiredDate));
                AddInfoRow(table, "Shipped Date", FormatDate(order.ShippedDate));
                AddInfoRow(table, "Shipper", order.ShipperName ?? "Not assigned");
                AddInfoRow(table, "Freight", FormatCurrency(order.Freight));
            });
        });
    }

    private static void ComposeShippingAddress(IContainer container, OrderResponse order)
    {
        var address = string.Join(", ", new[]
        {
            order.ShipName,
            order.ShipAddress,
            order.ShipCity,
            order.ShipRegion,
            order.ShipPostalCode,
            order.ShipCountry
        }.Where(value => !string.IsNullOrWhiteSpace(value)));

        container.Column(column =>
        {
            column.Item().Text("Shipping Address").FontSize(13).Bold();
            column.Item().PaddingTop(6).Text(string.IsNullOrWhiteSpace(address) ? "Not set" : address);
            column.Item().PaddingTop(6).Text($"Validated Address: {order.ShippingValidation?.FormattedAddress ?? "Not available"}");
            column.Item().Text($"Validation Status: {order.ShippingValidation?.ValidationStatus ?? "Not available"}");
            column.Item().Text($"Coordinates: {FormatCoordinates(order.ShippingValidation)}");
        });
    }

    private static void ComposeLineItems(IContainer container, OrderResponse order)
    {
        container.Column(column =>
        {
            column.Item().Text("Product Line Items").FontSize(13).Bold();
            column.Item().PaddingTop(6).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddHeaderCell(table, "Product");
                AddHeaderCell(table, "Quantity");
                AddHeaderCell(table, "Price");
                AddHeaderCell(table, "Discount");
                AddHeaderCell(table, "Total");

                foreach (var detail in order.Details)
                {
                    AddBodyCell(table, detail.ProductName);
                    AddBodyCell(table, detail.Quantity.ToString());
                    AddBodyCell(table, FormatCurrency(detail.UnitPrice));
                    AddBodyCell(table, $"{detail.Discount:P0}");
                    AddBodyCell(table, FormatCurrency(detail.LineTotal));
                }
            });
        });
    }

    private static void AddInfoRow(TableDescriptor table, string label, string value)
    {
        table.Cell().PaddingVertical(3).Text(label).SemiBold();
        table.Cell().PaddingVertical(3).Text(value);
    }

    private static void AddHeaderCell(TableDescriptor table, string value)
    {
        table.Cell()
            .Background(Colors.Blue.Darken3)
            .Padding(5)
            .Text(value)
            .FontColor(Colors.White)
            .SemiBold();
    }

    private static void AddBodyCell(TableDescriptor table, string value)
    {
        table.Cell()
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .Text(value);
    }

    private static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("yyyy-MM-dd") : "Not set";
    }

    private static string FormatCurrency(decimal value)
    {
        return value.ToString("C", CultureInfo.GetCultureInfo("en-US"));
    }

    private static string FormatCoordinates(OrderShippingValidationResponse? validation)
    {
        return validation?.Latitude is null || validation.Longitude is null
            ? "Not available"
            : string.Create(
                CultureInfo.InvariantCulture,
                $"{validation.Latitude.Value:F6}, {validation.Longitude.Value:F6}");
    }
}
