using API.DTOs.Overtimes;
using API.Models;

namespace API.Services.Interfaces;

public interface IOvertimeService
{
    Task<OvertimeDownloadResponseDto> DownloadDocumentAsync(Guid overtimeId);
    Task<int> ChangeRequestStatusAsync(OvertimeChangeRequestDto overtimeChangeRequestDto);
    Task<OvertimeDetailResponseDto> GetDetailByOvertimeIdAsync(Guid overtimeId);
    Task<IEnumerable<OvertimeDetailResponseDto>?> GetDetailsAsync(Guid accountId);
    Task<int> RequestOvertimeAsync(IFormFile? document, OvertimeRequestDto overtimeRequestDto);
    Task<IEnumerable<OvertimeDetailResponseDto>?> GetAllAsync();
    Task<Overtime?> GetByIdAsync(Guid id);
    Task<int> CreateAsync(Overtime overtime);
    Task<int> UpdateAsync(Guid id, Overtime overtime);
    Task<int> DeleteAsync(Guid id);
}
