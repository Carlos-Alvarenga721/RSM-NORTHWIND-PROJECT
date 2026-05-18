namespace NorthwindTraders.Application.DTOs.Employees;

/// <summary>
/// Small employee projection used by dropdowns and other lookup UI.
/// </summary>
public sealed record EmployeeLookupResponse(
    int EmployeeId,
    string FullName,
    string? Title,
    string? City,
    string? Country);
