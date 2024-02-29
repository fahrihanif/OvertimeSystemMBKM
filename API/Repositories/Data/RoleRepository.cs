using API.Data;
using API.Models;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories.Data;

public class RoleRepository : GeneralRepository<Role>, IRoleRepository
{
    public RoleRepository(OvertimeSystemDbContext context) : base(context) { }
    
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Set<Role>().FirstOrDefaultAsync(r => r.Name.Contains(name));
    }
}
