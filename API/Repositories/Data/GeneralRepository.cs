using API.Data;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories.Data;

public class GeneralRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly OvertimeSystemDbContext _context;

    public GeneralRepository(OvertimeSystemDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        var entity = await _context.Set<TEntity>().FindAsync(id);
        return entity;
    }

    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
    }

    public Task DeleteAsync(TEntity entity)
    {
        _context.Set<TEntity>().Remove(entity);
        return _context.SaveChangesAsync();
    }

    public Task ChangeTrackingAsync()
    {
        _context.ChangeTracker.Clear();
        return Task.CompletedTask;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }
}
