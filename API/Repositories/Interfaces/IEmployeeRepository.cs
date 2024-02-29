using API.Models;

namespace API.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<string?> GetLastNikAsync();
    Task<Employee?> GetByEmailAsync(string email);
}
