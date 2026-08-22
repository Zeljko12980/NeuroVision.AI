using BuildingBlocks.Persistence;

namespace PatientService.Infrastructure.Persistence;

public class PatientRepository<TEntity, TId>
    : Repository<TEntity, TId, PatientDbContext>
    where TEntity : class
    where TId : notnull
{
    public PatientRepository(PatientDbContext context)
        : base(context)
    {
    }
}
