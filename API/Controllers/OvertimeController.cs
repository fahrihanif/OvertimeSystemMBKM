using System.Net;
using API.DTOs.Overtimes;
using API.Services.Interfaces;
using API.Utilities.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("overtime")]
public class OvertimeController : ControllerBase
{
    private readonly IOvertimeService _overtimeService;

    public OvertimeController(IOvertimeService overtimeService)
    {
        _overtimeService = overtimeService;
    }

    [HttpGet("download-document/{id}")]
    public async Task<IActionResult> DownloadOvertimeDocumentAsync(Guid id)
    {
        var result = await _overtimeService.DownloadDocumentAsync(id);

        if (result.FileDownloadName == "0")
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Overtime Not Found")); // Data Not Found
        }

        if (result.FileDownloadName == "-1")
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Document Not Found")); // Data Not Found
        }

        return File(result.Document, result.ContentType, result.FileDownloadName);
    }

    [HttpPost("approval")]
    public async Task<IActionResult> ApprovalOvertimeAsync(OvertimeChangeRequestDto overtimeChangeRequestDto)
    {
        var result = await _overtimeService.ChangeRequestStatusAsync(overtimeChangeRequestDto);
        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Overtime Not Found")); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Overtime Approved"));
    }

    [HttpPost("request")]
    public async Task<IActionResult> RequestOvertimeAsync(IFormFile? document,
                                                          [FromForm] OvertimeRequestDto overtimeRequestDto)
    {
        var result = await _overtimeService.RequestOvertimeAsync(document, overtimeRequestDto);

        if (result == -1)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Document Not Found")); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Overtime Requested"));
    }

    [HttpGet("details/{accountId}")]
    public async Task<IActionResult> GetAllDetailAsync(Guid accountId)
    {
        var result = await _overtimeService.GetDetailsAsync(accountId);

        if (!result.Any())
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Never Request Overtime")); // Data Not Found
        }

        return Ok(new ListResponseVM<OvertimeDetailResponseDto>(StatusCodes.Status200OK,
                                                                HttpStatusCode.OK.ToString(),
                                                                "Data Overtime Found",
                                                                result.ToList()));
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> GetDetailByOvertimeIdAsync(Guid id)
    {
        var result = await _overtimeService.GetDetailByOvertimeIdAsync(id);

        if (result is null)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Overtime Not Found")); // Data Not Found
        }

        return Ok(new SingleResponseVM<OvertimeDetailResponseDto>(StatusCodes.Status200OK,
                                                                  HttpStatusCode.OK.ToString(),
                                                                  "Data Overtime Found",
                                                                  result));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await _overtimeService.GetAllAsync();

        if (!result.Any())
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Data Overtime Not Found"
                                                 )); // Data Not Found
        }
        
        return Ok(new ListResponseVM<OvertimeDetailResponseDto>(StatusCodes.Status200OK,
                                                                HttpStatusCode.OK.ToString(),
                                                                "Data Overtime Found",
                                                                result.ToList()));
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _overtimeService.DeleteAsync(id);

        if (result == 0)
        {
            return NotFound(new MessageResponseVM(StatusCodes.Status404NotFound,
                                                  HttpStatusCode.NotFound.ToString(),
                                                  "Id Overtime Not Found")); // Data Not Found
        }

        return Ok(new MessageResponseVM(StatusCodes.Status200OK,
                                        HttpStatusCode.OK.ToString(),
                                        "Overtime Deleted"));
    }
}
