namespace NorthwindTraders.Application.DTOs.Reports;

public sealed record ReportFilterRequest(
    int? Year,
    int? Month,
    int? Week,
    string? Region);
