using LocationService.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LocationService.Infrastructure.Persistence;

internal sealed class LocationWriteStore : ILocationWriteStore
{
    private readonly LocationDbContext _context;

    public LocationWriteStore(LocationDbContext context)
    {
        _context = context;
    }

    public async Task<TEntity?> FindAsync<TEntity>(
        object[] keyValues,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        return await _context.Set<TEntity>().FindAsync(keyValues, cancellationToken);
    }

    public async Task AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class
    {
        await _context.Set<TEntity>().AddAsync(entity, cancellationToken);
    }

    public void Update<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Update(entity);
    }

    public void Remove<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Set<TEntity>().Remove(entity);
    }
}
