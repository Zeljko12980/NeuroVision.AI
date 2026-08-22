using BuildingBlocks.Persistence;

namespace DoctorService.Infrastructure.Persistence;

public class DoctorRepository<TEntity, TId>
    : Repository<TEntity, TId, DoctorDbContext>
    where TEntity : class
    where TId : notnull
{
    public DoctorRepository(DoctorDbContext context)
        : base(context)
    {
    }
}
