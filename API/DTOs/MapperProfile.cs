using API.DTOs.AccountRoles;
using API.DTOs.Accounts;
using API.DTOs.Employees;
using API.DTOs.Overtimes;
using API.DTOs.Roles;
using API.Models;
using API.Utilities.Handlers;
using AutoMapper;

namespace API.DTOs;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        // For Employees
        CreateMap<EmployeeRequestDto, Employee>()
           .ForMember(dest => dest.JoinedDate,
                      opt => opt.MapFrom(src => DateTime.Now));
        
        CreateMap<RegisterDto, Employee>()
           .ForMember(dest => dest.JoinedDate,
                      opt => opt.MapFrom(src => DateTime.Now));

        CreateMap<Employee, EmployeeResponseDto>();

        // For Accounts
        CreateMap<RegisterDto, Account>()
           .ForMember(dest => dest.Password,
                      opt => opt.MapFrom(src => BCryptHandler.HashPassword(src.Password)))
           .ForMember(dest => dest.Otp,
                      opt => opt.MapFrom(src => 0))
           .ForMember(dest => dest.Expired,
                      opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(dest => dest.IsUsed,
                      opt => opt.MapFrom(src => true))
           .ForMember(dest => dest.IsActive,
                      opt => opt.MapFrom(src => true));
        
        CreateMap<AccountRequestDto, Account>()
           .ForMember(dest => dest.Password,
                      opt => opt.MapFrom(src => BCryptHandler.HashPassword(src.Password)))
           .ForMember(dest => dest.Otp,
                      opt => opt.MapFrom(src => 0))
           .ForMember(dest => dest.Expired,
                      opt => opt.MapFrom(src => DateTime.Now))
           .ForMember(dest => dest.IsUsed,
                      opt => opt.MapFrom(src => true))
           .ForMember(dest => dest.IsActive,
                      opt => opt.MapFrom(src => true));
        
        CreateMap<Account, AccountResponseDto>()
           .ForMember(dest => dest.Roles,
                      opt => opt.MapFrom(src => src.AccountRoles.Select(ar => ar.Role.Name)));

        // For AccountRoles
        CreateMap<AddAccountRoleRequestDto, AccountRole>();
        CreateMap<RemoveAccountRoleRequestDto, AccountRole>();
        CreateMap<AccountRole, AccountRoleResponseDto>();

        // For Roles
        CreateMap<RoleRequestDto, Role>();
        CreateMap<Role, RoleResponseDto>();

        // For Overtimes and OverTimeRequests
        CreateMap<OvertimeRequestDto, Overtime>()
           .ForMember(dest => dest.Id,
                      opt => opt.MapFrom(src => Guid.NewGuid()))
           .ForMember(dest => dest.Status,
                      opt => opt.MapFrom(src => "Requested"));
        CreateMap<Overtime, OvertimeRequest>()
           .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Reason))
           .ForMember(dest => dest.OvertimeId, opt => opt.MapFrom(src => src.Id))
           .ForMember(dest => dest.Timestamp, opt => opt.MapFrom(src => DateTime.Now));
        CreateMap<Overtime, OvertimeResponseDto>();
        CreateMap<Overtime, OvertimeDetailResponseDto>()
           .ForMember(dest => dest.Requests,
                      opt => opt.MapFrom(src =>
                                             src.OvertimeRequests.Select(or =>
                                                                             new
                                                                                 OvertimeRequestResponseDto(or.Timestamp,
                                                                                  or.Status, or.Comment))));
    }
}
