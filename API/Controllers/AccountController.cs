using System.Net;
using API.DTOs.AccountRoles;
using API.DTOs.Accounts;
using API.Services.Interfaces;
using API.Utilities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("account")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
    {
        var result = await _accountService.ResetPasswordAsync(resetPasswordRequestDto);

        if (result == -4)
        {
            return BadRequest(new MessageResponseVM(StatusCodes.Status400BadRequest,
                                                  HttpStatusCode.BadRequest.ToString(),
                                                  "Password Not Match"
                                                 )); // Password Not Match
        }
        
        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Account Not Found"
                                                 )); // Data Not Found
        }
        
        if (result == -1)
        {
            return BadRequest(new MessageResponseVM(StatusCodes.Status400BadRequest,
                                                  HttpStatusCode.BadRequest.ToString(),
                                                  "Otp Expired"
                                                 )); // Otp Expired
        }
        
        if (result == -2)
        {
            return BadRequest(new MessageResponseVM(StatusCodes.Status400BadRequest,
                                                  HttpStatusCode.BadRequest.ToString(),
                                                  "Otp Not Match"
                                                 )); // Otp Not Match
        }
        
        if (result == -3)
        {
            return BadRequest(new MessageResponseVM(StatusCodes.Status400BadRequest,
                                                  HttpStatusCode.BadRequest.ToString(),
                                                  "Otp Already Used"
                                                 )); // Otp Already Used
        }
        
        

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Password Reset"));
    }
    
    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtpAsync(string email)
    {
        var result = await _accountService.SendOtpAsync(email);

        if (result == null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Email Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new SingleResponseVM<OtpResponseDto>(StatusCodes.Status200OK,
                                                       HttpStatusCode.OK.ToString(),
                                                       "Otp Sent",
                                                       result));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync(LoginRequestDto loginRequestDto)
    {
        var result = await _accountService.LoginAsync(loginRequestDto);

        if (result == null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Email or Password Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new SingleResponseVM<LoginResponseDto>(StatusCodes.Status200OK,
                                                           HttpStatusCode.OK.ToString(),
                                                           "Login Success",
                                                           result));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerDto)
    {
        var result = await _accountService.RegisterAsync(registerDto);

        if (result == -1)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Role Not Found"
                                                 )); // Data Not Found
        }
        
        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Account Created"));
    }

    [HttpDelete("remove-role")]
    public async Task<IActionResult> RemoveRoleAsync(RemoveAccountRoleRequestDto removeAccountRoleRequestDto)
    {
        var result = await _accountService.RemoveRoleAsync(removeAccountRoleRequestDto);

        if (result == 0)
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Employee Not Found"
                                                 )); // Data Not Found

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Employee Deleted"));
    }

    [HttpPost("add-role")]
    public async Task<IActionResult> AddRoleAsync(AddAccountRoleRequestDto addAccountRoleRequestDto)
    {
        var result = await _accountService.AddAccountRoleAsync(addAccountRoleRequestDto);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Account Id Not Found"
                                                 )); // Data Not Found
        }

        if (result == -1)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Role Id Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Role Added"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var results = await _accountService.GetAllAsync();

        if (!results.Any())
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Data Account Not Found")); // Data Not Found
        }

        return Ok(new ListResponseVM<AccountResponseDto>(StatusCodes.Status200OK,
                                                         HttpStatusCode.OK.ToString(),
                                                         "Data Account Found",
                                                         results.ToList()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _accountService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Account Not Found")); // Data Not Found
        }

        return Ok(new SingleResponseVM<AccountResponseDto>(StatusCodes.Status200OK,
                                                           HttpStatusCode.OK.ToString(),
                                                           "Data Account Found",
                                                           result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(AccountRequestDto accountRequestDto)
    {
        var result = await _accountService.CreateAsync(accountRequestDto);

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Account Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, AccountRequestDto accountRequestDto)
    {
        var result = await _accountService.UpdateAsync(id, accountRequestDto);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Account Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Account Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _accountService.DeleteAsync(id);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Account Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Account Deleted"));
    }
}
