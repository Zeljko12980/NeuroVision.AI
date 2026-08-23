namespace PatientService.Application.Common.Interfaces;

public interface IPatientWriteStore
{
    Task<TEntity?> FindAsync<TEntity>(
        object[] keyValues,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    Task AddAsync<TEntity>(
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : class;

    void Update<TEntity>(TEntity entity) where TEntity : class;

    void Remove<TEntity>(TEntity entity) where TEntity : class;
}
