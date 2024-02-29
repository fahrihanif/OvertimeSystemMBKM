namespace API.DTOs.Accounts;

public record LoginRequestDto(
    string Email,
    string Password
    );
