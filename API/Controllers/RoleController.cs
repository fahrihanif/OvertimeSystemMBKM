using System.Net;
using API.DTOs.Roles;
using API.Services.Interfaces;
using API.Utilities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("role")]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService employeeService)
    {
        _roleService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var results = await _roleService.GetAllAsync();

        if (!results.Any())
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Data Employee Not Found")); // Data Not Found
        }

        return Ok(new ListResponseVM<RoleResponseDto>(StatusCodes.Status200OK,
                                                      HttpStatusCode.OK.ToString(),
                                                      "Data Employee Found",
                                                      results.ToList()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _roleService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Employee Not Found")); // Data Not Found
        }

        return Ok(new SingleResponseVM<RoleResponseDto>(StatusCodes.Status200OK,
                                                        HttpStatusCode.OK.ToString(),
                                                        "Data Employee Found",
                                                        result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(RoleRequestDto roleRequestDto)
    {
        await _roleService.CreateAsync(roleRequestDto);

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Employee Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, RoleRequestDto roleRequestDto)
    {
        var result = await _roleService.UpdateAsync(id, roleRequestDto);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Employee Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Employee Updated"));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _roleService.DeleteAsync(id);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Employee Not Found"
                                                 )); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Employee Deleted"));
    }
}
