namespace NorthwindTraders.Application.DTOs.Employees;

public sealed record EmployeeLookupDto(
    int EmployeeId,
    string FullName,
    string? Title,
    string? City,
    string? Country);
