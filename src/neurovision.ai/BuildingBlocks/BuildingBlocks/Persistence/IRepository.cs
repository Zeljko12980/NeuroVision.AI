namespace BuildingBlocks.Persistence;

public interface IRepository<TEntity, TId>
    where TEntity : class
    where TId : notnull
{
    Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default);

    void Update(TEntity entity);

    void Delete(TEntity entity);
}