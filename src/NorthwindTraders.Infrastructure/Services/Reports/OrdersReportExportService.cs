using System.Globalization;
using ClosedXML.Excel;
using NorthwindTraders.Application.Abstractions.Services;
using NorthwindTraders.Application.DTOs.Reports;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace NorthwindTraders.Infrastructure.Services.Reports;

public sealed class OrdersReportExportService : IOrdersReportExportService
{
    public byte[] GenerateExcel(OrdersReportResponse report, ReportFilterRequest filters)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Orders Report");

        worksheet.Cell(1, 1).Value = "Northwind Traders Orders Report";
        worksheet.Cell(1, 1).Style.Font.Bold = true;
        worksheet.Cell(1, 1).Style.Font.FontSize = 16;
        worksheet.Range(1, 1, 1, 8).Merge();

        worksheet.Cell(3, 1).Value = "Filters";
        worksheet.Cell(3, 1).Style.Font.Bold = true;
        worksheet.Cell(4, 1).Value = "Year";
        worksheet.Cell(4, 2).Value = filters.Year?.ToString(CultureInfo.InvariantCulture) ?? "All";
        worksheet.Cell(5, 1).Value = "Month";
        worksheet.Cell(5, 2).Value = filters.Month?.ToString(CultureInfo.InvariantCulture) ?? "All";
        worksheet.Cell(6, 1).Value = "Week";
        worksheet.Cell(6, 2).Value = filters.Week?.ToString(CultureInfo.InvariantCulture) ?? "All";
        worksheet.Cell(7, 1).Value = "Region";
        worksheet.Cell(7, 2).Value = string.IsNullOrWhiteSpace(filters.Region) ? "All" : filters.Region;

        var headerRow = 9;
        var headers = new[]
        {
            "Order ID",
            "Customer",
            "Employee",
            "Order Date",
            "Shipped Date",
            "Region",
            "Ship Country",
            "Order Total"
        };

        for (var index = 0; index < headers.Length; index++)
        {
            worksheet.Cell(headerRow, index + 1).Value = headers[index];
            worksheet.Cell(headerRow, index + 1).Style.Font.Bold = true;
            worksheet.Cell(headerRow, index + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#28527A");
            worksheet.Cell(headerRow, index + 1).Style.Font.FontColor = XLColor.White;
        }

        var currentRow = headerRow + 1;
        foreach (var order in report.Orders)
        {
            worksheet.Cell(currentRow, 1).Value = order.OrderId;
            worksheet.Cell(currentRow, 2).Value = order.CustomerName ?? string.Empty;
            worksheet.Cell(currentRow, 3).Value = order.EmployeeName ?? string.Empty;
            worksheet.Cell(currentRow, 4).Value = order.OrderDate;
            worksheet.Cell(currentRow, 5).Value = order.ShippedDate;
            worksheet.Cell(currentRow, 6).Value = order.ShipRegion ?? string.Empty;
            worksheet.Cell(currentRow, 7).Value = order.ShipCountry ?? string.Empty;
            worksheet.Cell(currentRow, 8).Value = order.OrderTotal;
            currentRow++;
        }

        worksheet.Column(4).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Column(5).Style.DateFormat.Format = "yyyy-mm-dd";
        worksheet.Column(8).Style.NumberFormat.Format = "$#,##0.00";
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        return stream.ToArray();
    }

    public byte[] GeneratePdf(OrdersReportResponse report, ReportFilterRequest filters)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(document =>
        {
            document.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(32);
                page.DefaultTextStyle(text => text.FontSize(9));

                page.Header().Element(header => ComposeHeader(header, filters));
                page.Content().Element(content => ComposeContent(content, report));
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Northwind Traders Orders Report - Page ");
                        text.CurrentPageNumber();
                        text.Span(" of ");
                        text.TotalPages();
                    });
            });
        }).GeneratePdf();
    }

    private static void ComposeHeader(IContainer container, ReportFilterRequest filters)
    {
        container.Column(column =>
        {
            column.Item().Text("Northwind Traders Orders Report")
                .FontSize(18)
                .Bold()
                .FontColor(Colors.Blue.Darken3);
            column.Item().PaddingTop(4).Text(GetFilterSummary(filters)).FontSize(9);
            column.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
        });
    }

    private static void ComposeContent(IContainer container, OrdersReportResponse report)
    {
        container.PaddingTop(12).Column(column =>
        {
            column.Spacing(12);
            column.Item().Text($"Orders: {report.Orders.Count} | Regions: {report.ShipmentsByRegion.Count} | Total: {FormatCurrency(report.Orders.Sum(order => order.OrderTotal))}")
                .SemiBold();
            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(55);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                AddHeaderCell(table, "Order ID");
                AddHeaderCell(table, "Customer");
                AddHeaderCell(table, "Employee");
                AddHeaderCell(table, "Order Date");
                AddHeaderCell(table, "Shipped Date");
                AddHeaderCell(table, "Region");
                AddHeaderCell(table, "Country");
                AddHeaderCell(table, "Total");

                foreach (var order in report.Orders)
                {
                    AddBodyCell(table, order.OrderId.ToString(CultureInfo.InvariantCulture));
                    AddBodyCell(table, order.CustomerName ?? string.Empty);
                    AddBodyCell(table, order.EmployeeName ?? string.Empty);
                    AddBodyCell(table, FormatDate(order.OrderDate));
                    AddBodyCell(table, FormatDate(order.ShippedDate));
                    AddBodyCell(table, order.ShipRegion ?? string.Empty);
                    AddBodyCell(table, order.ShipCountry ?? string.Empty);
                    AddBodyCell(table, FormatCurrency(order.OrderTotal));
                }
            });
        });
    }

    private static string GetFilterSummary(ReportFilterRequest filters)
    {
        return $"Year: {filters.Year?.ToString(CultureInfo.InvariantCulture) ?? "All"} | " +
            $"Month: {filters.Month?.ToString(CultureInfo.InvariantCulture) ?? "All"} | " +
            $"Week: {filters.Week?.ToString(CultureInfo.InvariantCulture) ?? "All"} | " +
            $"Region: {(string.IsNullOrWhiteSpace(filters.Region) ? "All" : filters.Region)}";
    }

    private static void AddHeaderCell(TableDescriptor table, string value)
    {
        table.Cell()
            .Background(Colors.Blue.Darken3)
            .Padding(4)
            .Text(value)
            .FontColor(Colors.White)
            .SemiBold();
    }

    private static void AddBodyCell(TableDescriptor table, string value)
    {
        table.Cell()
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(4)
            .Text(value);
    }

    private static string FormatDate(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : "Not set";
    }

    private static string FormatCurrency(decimal value)
    {
        return value.ToString("C", CultureInfo.GetCultureInfo("en-US"));
    }
}
