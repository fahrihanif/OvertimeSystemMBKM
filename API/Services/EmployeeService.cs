using API.DTOs.Employees;
using API.Models;
using API.Repositories.Interfaces;
using API.Services.Interfaces;
using API.Utilities.Handlers;
using AutoMapper;

namespace API.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public EmployeeService(IEmployeeRepository employeeRepository, IMapper mapper)
    {
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeResponseDto>?> GetAllAsync()
    {
        var data = await _employeeRepository.GetAllAsync();

        var dataMap = _mapper.Map<IEnumerable<EmployeeResponseDto>>(data);

        return dataMap; // success
    }

    public async Task<EmployeeResponseDto?> GetByIdAsync(Guid id)
    {
        var data = await _employeeRepository.GetByIdAsync(id);

        var dataMap = _mapper.Map<EmployeeResponseDto>(data);

        return dataMap; // success
    }

    public async Task<int> CreateAsync(EmployeeRequestDto employeeRequestDto)
    {
        var getLastNik = await _employeeRepository.GetLastNikAsync();
        var employee = _mapper.Map<Employee>(employeeRequestDto);
        employee.Nik = GenerateHandler.Nik(getLastNik);
        await _employeeRepository.CreateAsync(employee);

        return 1; // success
    }

    public async Task<int> UpdateAsync(Guid id, EmployeeRequestDto employeeRequestDto)
    {
        var data = await _employeeRepository.GetByIdAsync(id);
        await _employeeRepository.ChangeTrackingAsync();
        if (data == null) return 0; // not found

        var employee = _mapper.Map<Employee>(employeeRequestDto);

        employee.Id = id;
        employee.Nik = data.Nik;
        employee.JoinedDate = data.JoinedDate;
        await _employeeRepository.UpdateAsync(employee);

        return 1; // success
    }

    public async Task<int> DeleteAsync(Guid id)
    {
        var data = await _employeeRepository.GetByIdAsync(id);

        if (data == null) return 0; // not found

        await _employeeRepository.DeleteAsync(data);

        return 1; // success
    }
}
