using API.DTOs.AccountRoles;
using API.DTOs.Accounts;

namespace API.Services.Interfaces;

public interface IAccountService
{
    Task<int> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    Task<int> SendOtpAsync(string email);
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
    Task<int> RegisterAsync(RegisterDto registerDto);
    Task<int> AddAccountRoleAsync(AddAccountRoleRequestDto addAccountRoleRequestDto);
    Task<int> RemoveRoleAsync(RemoveAccountRoleRequestDto removeAccountRoleRequestDto);
    Task<IEnumerable<AccountResponseDto>?> GetAllAsync();
    Task<AccountResponseDto?> GetByIdAsync(Guid id);
    Task<int> CreateAsync(AccountRequestDto accountRequestDto);
    Task<int> UpdateAsync(Guid id, AccountRequestDto accountRequestDto);
    Task<int> DeleteAsync(Guid id);
}
