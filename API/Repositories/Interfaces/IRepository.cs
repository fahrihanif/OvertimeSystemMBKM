using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories.Interfaces;

public interface IRepository<TEntity>
{
    Task<IEnumerable<TEntity>> GetAllAsync();
    Task<TEntity?> GetByIdAsync(Guid id);
    Task<TEntity> CreateAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(TEntity entity);
    Task ChangeTrackingAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
