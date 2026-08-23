using PatientService.Application.Common.Interfaces;

namespace PatientService.Infrastructure.Persistence;

internal sealed class PatientWriteStore : IPatientWriteStore
{
    private readonly PatientDbContext _context;

    public PatientWriteStore(PatientDbContext context)
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
