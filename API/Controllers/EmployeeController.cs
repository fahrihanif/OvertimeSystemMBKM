using System.Net;
using API.DTOs.Employees;
using API.Services.Interfaces;
using API.Utilities.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize(Roles = "employee")]
[ApiController]
[Route("employee")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeeController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var results = await _employeeService.GetAllAsync();

        if (!results.Any())
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Data Employee Not Found")); // Data Not Found
        }

        return Ok(new ListResponseVM<EmployeeResponseDto>(StatusCodes.Status200OK,
                                                          HttpStatusCode.OK.ToString(),
                                                          "Data Employee Found",
                                                          results.ToList()));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
    {
        var result = await _employeeService.GetByIdAsync(id);

        if (result is null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Employee Not Found")); // Data Not Found
        }

        return Ok(new SingleResponseVM<EmployeeResponseDto>(StatusCodes.Status200OK,
                                                            HttpStatusCode.OK.ToString(),
                                                            "Data Employee Found",
                                                            result));
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(EmployeeRequestDto employeeRequestDto)
    {
        await _employeeService.CreateAsync(employeeRequestDto);

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Employee Created"));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(Guid id, EmployeeRequestDto employeeRequestDto)
    {
        var result = await _employeeService.UpdateAsync(id, employeeRequestDto);

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
        var result = await _employeeService.DeleteAsync(id);

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
