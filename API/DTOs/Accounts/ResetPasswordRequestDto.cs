namespace API.DTOs.Accounts;

public record ResetPasswordRequestDto(string Email, int Otp, string Password, string ConfirmPassword);
