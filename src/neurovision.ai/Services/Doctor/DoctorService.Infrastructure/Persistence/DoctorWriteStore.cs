using DoctorService.Application.Common.Interfaces;

namespace DoctorService.Infrastructure.Persistence;

internal sealed class DoctorWriteStore : IDoctorWriteStore
{
    private readonly DoctorDbContext _context;

    public DoctorWriteStore(DoctorDbContext context)
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
