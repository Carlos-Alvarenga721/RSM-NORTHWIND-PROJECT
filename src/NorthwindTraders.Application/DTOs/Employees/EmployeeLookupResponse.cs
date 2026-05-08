namespace NorthwindTraders.Application.DTOs.Employees;

public sealed record EmployeeLookupResponse(
    int EmployeeId,
    string FullName,
    string? Title,
    string? City,
    string? Country);
