using BuildingBlocks.Persistence;
using PdfService.Infrastructure.Data;

namespace PdfService.Infrastructure.Repositories;

public sealed class PdfRepository<TEntity, TId>
    : Repository<TEntity,TId, PdfDbContext>
    where TEntity : class
    where TId : notnull
{
    public PdfRepository(PdfDbContext context)
        : base(context)
    {
    }
}