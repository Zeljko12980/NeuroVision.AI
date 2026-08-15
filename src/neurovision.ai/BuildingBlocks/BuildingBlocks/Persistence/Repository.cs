using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Persistence;

public class Repository<TEntity, TId, TContext>
    : IRepository<TEntity, TId>
    where TEntity : class
    where TId : notnull
    where TContext : DbContext
{
    protected readonly TContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(TContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }


    public async Task<TEntity?> GetByIdAsync(
        TId id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync(
            new object[] { id },
            cancellationToken);
    }


    public async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(
            entity,
            cancellationToken);
    }


    public void Update(TEntity entity)
    {
        DbSet.Update(entity);
    }


    public void Delete(TEntity entity)
    {
        DbSet.Remove(entity);
    }
}