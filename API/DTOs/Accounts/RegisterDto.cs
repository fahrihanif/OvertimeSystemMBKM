namespace API.DTOs.Accounts;

public record RegisterDto(
    string FirstName,
    string LastName,
    int Salary,
    string Email,
    string Position,
    string Department,
    Guid? ManagerId,
    string Password,
    string ConfirmPassword);
