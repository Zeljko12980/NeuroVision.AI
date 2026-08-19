using BuildingBlocks.Persistence;

namespace LocationService.Infrastructure.Persistence
{
    public class LocationRepository<TEntity, TId>
     : Repository<TEntity, TId, LocationDbContext>
     where TEntity : class
     where TId : notnull
    {
        public LocationRepository(LocationDbContext context)
            : base(context)
        {
        }
    }
}
