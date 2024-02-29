using API.Data;
using API.DTOs.AccountRoles;
using API.DTOs.Accounts;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities.Handlers;
using AutoMapper;

namespace API.Services;

public class AccountService : IAccountService
{
    private readonly IAccountRepository _accountRepository;
    private readonly IAccountRoleRepository _accountRoleRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;
    private readonly IEmailHandler _emailHandler;

    public AccountService(IAccountRepository accountRepository, IMapper mapper,
                          IAccountRoleRepository accountRoleRepository, IRoleRepository roleRepository,
                          IEmployeeRepository employeeRepository, IEmailHandler emailHandler)
    {
        _accountRepository = accountRepository;
        _mapper = mapper;
        _accountRoleRepository = accountRoleRepository;
        _roleRepository = roleRepository;
        _employeeRepository = employeeRepository;
        _emailHandler = emailHandler;
    }

    public async Task<int> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
    {
        if(resetPasswordRequestDto.Password != resetPasswordRequestDto.ConfirmPassword) return -4; // password not match
        
        var employee = await _employeeRepository.GetByEmailAsync(resetPasswordRequestDto.Email);
        if (employee == null) return 0; // not found
        
        var account = await _accountRepository.GetByIdAsync(employee.Id);
        if (account == null) return 0; // not found
        
        if (account.Expired < DateTime.Now) return -1; // otp expired
        if (account.Otp != resetPasswordRequestDto.Otp) return -2; // otp not match
        if (account.IsUsed) return -3; // otp already used
        
        account.Password = BCryptHandler.HashPassword(resetPasswordRequestDto.Password);
        account.IsUsed = true;
        
        await _accountRepository.UpdateAsync(account);
        
        return 1; // success
    }

    public async Task<OtpResponseDto?> SendOtpAsync(string email)
    {
        var employee = await _employeeRepository.GetByEmailAsync(email);
        if (employee == null) return null; // not found
        
        var account = await _accountRepository.GetByIdAsync(employee.Id);
        if (account == null) return null; // not found
        
        var otp = new Random().Next(00000, 99999);
        account.Otp = otp;
        account.IsUsed = false;
        account.Expired = DateTime.Now.AddMinutes(5);
        
        await _accountRepository.UpdateAsync(account);

        var message = $"<p>Hi {string.Concat(employee.FirstName, " ", employee.LastName)},</p>\n" +
                      $"<p>Your OTP is: <span style=\"font-weight: bold;\">{otp}</span>. " +
                      $"Expires in 5 minutes.</p>\n" +
                      $"<p>**Security:** Never share OTP, be cautious of requests, change password regularly.</p>\n\n" +
                      $"<p>Sincerely,</p>\n  <p>The Overtime System Team</p>";
        
        var emailMap = new EmailDto(email, "[Reset Password] - MBKM 6", message);
        await _emailHandler.SendEmailAsync(emailMap);

        return new OtpResponseDto(otp);
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var employee = await _employeeRepository.GetByEmailAsync(loginRequestDto.Email);

        if (employee == null) return null; // not found

        var account = await _accountRepository.GetByIdAsync(employee.Id);

        if (account == null) return null; // not found

        var isPasswordValid = BCryptHandler.VerifyPassword(loginRequestDto.Password, account.Password);
        if (!isPasswordValid) return null; // not found

        return new LoginResponseDto("isinya nanti token jwt");
    }

    public async Task<int> RegisterAsync(RegisterDto registerDto)
    {
        await using var transaction = await _accountRepository.BeginTransactionAsync();
        try
        {
            var employee = _mapper.Map<Employee>(registerDto);
            employee.Nik = GenerateHandler.Nik(await _employeeRepository.GetLastNikAsync());

            var employeeResult = await _employeeRepository.CreateAsync(employee);

            var account = _mapper.Map<Account>(registerDto);
            account.Id = employeeResult.Id;

            var accountResult = await _accountRepository.CreateAsync(account);

            var role = await _roleRepository.GetByNameAsync("employee");

            if (role == null) return -1; // Role not found 

            var accountRole = new AccountRole {
                AccountId = accountResult.Id,
                RoleId = role.Id
            };

            await _accountRoleRepository.CreateAsync(accountRole);
            await transaction.CommitAsync();
            return 1; // success
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<int> AddAccountRoleAsync(AddAccountRoleRequestDto addAccountRoleRequestDto)
    {
        var account = await _accountRepository.GetByIdAsync(addAccountRoleRequestDto.AccountId);

        if (account == null) return 0; // Account not found

        var role = await _roleRepository.GetByIdAsync(addAccountRoleRequestDto.RoleId);

        if (role == null) return -1; // Account not found

        var accountRole = _mapper.Map<AccountRole>(addAccountRoleRequestDto);

        await _accountRoleRepository.CreateAsync(accountRole);

        return 1; // success
    }

    public async Task<int> RemoveRoleAsync(RemoveAccountRoleRequestDto removeAccountRoleRequestDto)
    {
        var accountRole =
            await _accountRoleRepository.GetDataByAccountIdAndRoleAsync(removeAccountRoleRequestDto.AccountId,
                                                                        removeAccountRoleRequestDto.RoleId);

        if (accountRole == null) return 0; // Account or Role not found

        await _accountRoleRepository.DeleteAsync(accountRole);

        return 1; // success
    }

    public async Task<IEnumerable<AccountResponseDto>?> GetAllAsync()
    {
        var data = await _accountRepository.GetAllAsync();

        var dataMap = _mapper.Map<IEnumerable<AccountResponseDto>>(data);

        return dataMap; // success
    }

    public async Task<AccountResponseDto?> GetByIdAsync(Guid id)
    {
        var account = await _accountRepository.GetByIdAsync(id);

        if (account == null) return null; // not found

        var dataMap = _mapper.Map<AccountResponseDto>(account);

        return dataMap; // success
    }

    public async Task<int> CreateAsync(AccountRequestDto accountRequestDto)
    {
        var account = _mapper.Map<Account>(accountRequestDto);

        await _accountRepository.CreateAsync(account);

        return 1; // success
    }

    public async Task<int> UpdateAsync(Guid id, AccountRequestDto accountRequestDto)
    {
        var data = await _accountRepository.GetByIdAsync(id);
        await _accountRepository.ChangeTrackingAsync();
        if (data == null) return 0; // not found

        var account = _mapper.Map<Account>(accountRequestDto);

        account.Id = id;
        await _accountRepository.UpdateAsync(account);

        return 1; // success
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var data = await _accountRepository.GetByIdAsync(id);

        if (data == null) return 0; // not found

        await _accountRepository.DeleteAsync(data);

        return 1; // success
    }
}
