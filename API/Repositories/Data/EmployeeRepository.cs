using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Data;

public class EmployeeRepository : GeneralRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(OvertimeSystemDbContext context) : base(context) { }

    public async Task<string?> GetLastNikAsync()
    {
        return _context.Set<Employee>().ToList().Select(e => e.Nik).LastOrDefault();
    }

    public async Task<Employee?> GetByEmailAsync(string email)
    {
        return await _context.Set<Employee>().FirstOrDefaultAsync(e => e.Email == email);
    }
}
