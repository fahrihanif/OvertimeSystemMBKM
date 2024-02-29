namespace API.DTOs.Employees;

public record EmployeeRequestDto(
    string FirstName,
    string LastName,
    int Salary,
    string Email,
    string Position,
    string Department,
    Guid? ManagerId);
