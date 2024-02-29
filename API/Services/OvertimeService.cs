using API.DTOs.Overtimes;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities.Handlers;
using AutoMapper;

namespace API.Services;

public class OvertimeService : IOvertimeService
{
    private readonly IMapper _mapper;
    private readonly IOvertimeRepository _overtimeRepository;
    private readonly IOvertimeRequestRepository _overtimeRequestRepository;

    public OvertimeService(IOvertimeRepository overtimeRepository, IMapper mapper,
                           IOvertimeRequestRepository overtimeRequestRepository)
    {
        _overtimeRepository = overtimeRepository;
        _mapper = mapper;
        _overtimeRequestRepository = overtimeRequestRepository;
    }

    public async Task<OvertimeDownloadResponseDto> DownloadDocumentAsync(Guid overtimeId)
    {
        var overtime = await _overtimeRepository.GetByIdAsync(overtimeId);

        if (overtime is null)
            return new OvertimeDownloadResponseDto(BitConverter.GetBytes(0),
                                                   "0",
                                                   "0"); // id not found

        if (string.IsNullOrEmpty(overtime.Document))
            return new OvertimeDownloadResponseDto(BitConverter.GetBytes(-1),
                                                   "-1",
                                                   "-1"); // document not found

        byte[] document;
        try
        {
            document = await DocumentHandler.Download(overtime.Document);
        }
        catch
        {
            throw new Exception("File not exist in server");
        }

        return new OvertimeDownloadResponseDto(document, "application/octet-stream",
                                               Path.GetFileName(overtime.Document)); // success
    }

    public Task<int> ChangeRequestStatusAsync(OvertimeChangeRequestDto overtimeChangeRequestDto)
    {
        try
        {
            // TODO: Implement ChangeRequestStatusAsync
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException?.Message ?? ex.Message,
                              Console.ForegroundColor = ConsoleColor.Red);

            throw; // error
        }
    }

    public async Task<OvertimeDetailResponseDto> GetDetailByOvertimeIdAsync(Guid overtimeId)
    {
        var data = await _overtimeRepository.GetByIdAsync(overtimeId);

        var dataMap = _mapper.Map<OvertimeDetailResponseDto>(data);

        return dataMap; // success
    }

    public async Task<IEnumerable<OvertimeDetailResponseDto>?> GetDetailsAsync(Guid accountId)
    {
        var data = await _overtimeRepository.GetAllAsync();

        data = data.Where(x => x.OvertimeRequests.Any(or => or.AccountId == accountId));

        var dataMap = _mapper.Map<IEnumerable<OvertimeDetailResponseDto>>(data);

        return dataMap; // success
    }

    public async Task<int> RequestOvertimeAsync(IFormFile? document, OvertimeRequestDto overtimeRequestDto)
    {
        if (document is null) return -1; // file not found
        if (document.Length == 0) return -1; // file not found
        
        await using var transaction = await _overtimeRepository.BeginTransactionAsync();

        try
        {
            var overtime = _mapper.Map<Overtime>(overtimeRequestDto);
            var upload = await DocumentHandler.Upload(document, overtime.Id);
            overtime.Document = upload;
            var data = await _overtimeRepository.CreateAsync(overtime);

            var overtimeRequset = _mapper.Map<OvertimeRequest>(data);
            overtimeRequset.AccountId = overtimeRequestDto.AccountId;
            await _overtimeRequestRepository.CreateAsync(overtimeRequset);

            await transaction.CommitAsync();
            return 1; // success
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<IEnumerable<OvertimeDetailResponseDto>?> GetAllAsync()
    {
        var data = await _overtimeRepository.GetAllAsync();

        var dataMap = _mapper.Map<IEnumerable<OvertimeDetailResponseDto>>(data);

        return dataMap; // success
    }

    public async Task<Overtime?> GetByIdAsync(Guid id)
    {
        var data = await _overtimeRepository.GetByIdAsync(id);

        return data; // success
    }

    public async Task<int> CreateAsync(Overtime overtime)
    {
        await _overtimeRepository.CreateAsync(overtime);

        return 1; // success
    }

    public async Task<int> UpdateAsync(Guid id, Overtime overtime)
    {
        var data = await _overtimeRepository.GetByIdAsync(id);
        await _overtimeRepository.ChangeTrackingAsync();
        if (data == null) return 0; // not found

        overtime.Id = id;
        await _overtimeRepository.UpdateAsync(overtime);

        return 1; // success
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var data = await _overtimeRepository.GetByIdAsync(id);

        if (data == null) return 0; // not found

        await _overtimeRepository.DeleteAsync(data);

        return 1; // success
    }
}
