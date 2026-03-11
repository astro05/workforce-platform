using Microsoft.EntityFrameworkCore;
using WorkforceAPI.Domain.Repositories;

namespace WorkforceAPI.Infrastructure.Persistence.MsSqlServer;

public class GenericRepository<T> : IRepository<T> where T : class
{
    private readonly WorkforceDbContext _ctx;
    private readonly DbSet<T> _set;

    public GenericRepository(WorkforceDbContext ctx)
    {
        _ctx = ctx;
        _set = ctx.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync([id], ct);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.ToListAsync(ct);

    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        return entity;
    }

    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _set.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}