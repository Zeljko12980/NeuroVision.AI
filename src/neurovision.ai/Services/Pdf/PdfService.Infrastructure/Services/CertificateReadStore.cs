using BuildingBlocks.Dapper;
using PdfService.Application.Common.Interfaces;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Services;

public sealed class CertificateReadStore : ICertificateReadStore
{
    private readonly ISqlQueryExecutor _sql;

    public CertificateReadStore(ISqlQueryExecutor sql)
    {
        _sql = sql;
    }

    public async Task<(IReadOnlyList<Certificate> Items, long TotalCount)> GetPagedAsync(
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            PageSize = pageSize,
            Offset = pageIndex * pageSize
        };

        var totalCount = await _sql.QuerySingleAsync<int>(
            """
            SELECT COUNT(*)
            FROM "Certificates";
            """,
            parameters);

        var rows = await _sql.QueryAsync<CertificateRow>(
            """
            SELECT *
            FROM "Certificates"
            LIMIT @PageSize
            OFFSET @Offset;
            """,
            parameters);

        return (rows.Select(row => row.ToDomain()).ToList(), totalCount);
    }

    private sealed class CertificateRow
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Thumbprint { get; set; } = string.Empty;
        public string SerialNumber { get; set; } = string.Empty;
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public bool IsDefault { get; set; }
        public string ProtectedPassword { get; set; } = string.Empty;

        public Certificate ToDomain() =>
            Certificate.Restore(
                Id,
                Name,
                Subject,
                Issuer,
                Thumbprint,
                SerialNumber,
                ValidFrom,
                ValidTo,
                FileName,
                FilePath,
                ProtectedPassword,
                IsDefault);
    }
}
