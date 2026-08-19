using BuildingBlocks.Dapper;
using PdfService.Application.Common.Interfaces;
using PdfService.Domain.Entities;

namespace PdfService.Infrastructure.Services;

public sealed class PdfTemplateReadStore : IPdfTemplateReadStore
{
    private readonly ISqlQueryExecutor _sql;

    public PdfTemplateReadStore(ISqlQueryExecutor sql)
    {
        _sql = sql;
    }

    public Task<Guid?> GetIdByCodeAsync(string code, CancellationToken cancellationToken = default) =>
        _sql.QuerySingleAsync<Guid?>(
            """
            SELECT public.get_pdf_template_id_by_code(@Code);
            """,
            new { Code = code });

    public async Task<PdfTemplate?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var row = await _sql.QuerySingleAsync<PdfTemplateRow>(
            """
            SELECT *
            FROM "pdf_templates"
            WHERE "Code" = @Code
            LIMIT 1;
            """,
            new { Code = code });

        return row?.ToDomain();
    }

    public async Task LoadFieldsAsync(PdfTemplate template, CancellationToken cancellationToken = default)
    {
        var rows = await _sql.QueryAsync<PdfTemplateFieldRow>(
            """
            SELECT *
            FROM "pdf_template_fields"
            WHERE "PdfTemplateId" = @Id;
            """,
            new { template.Id });

        template.Fields.Clear();
        foreach (var field in rows.Select(row => row.ToDomain()))
            template.Fields.Add(field);
    }

    public async Task<(IReadOnlyList<PdfTemplate> Items, long TotalCount)> GetPagedAsync(
        string? code,
        int pageIndex,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var parameters = new
        {
            Code = code,
            PageSize = pageSize,
            Offset = pageIndex * pageSize
        };

        var totalCount = await _sql.QuerySingleAsync<int>(
            """
            SELECT COUNT(*)
            FROM "pdf_templates"
            WHERE (@Code IS NULL OR "Code" ILIKE '%' || @Code || '%');
            """,
            parameters);

        var rows = await _sql.QueryAsync<PdfTemplateRow>(
            """
            SELECT *
            FROM "pdf_templates"
            WHERE (@Code IS NULL OR "Code" ILIKE '%' || @Code || '%')
            ORDER BY "CreatedAt" DESC
            LIMIT @PageSize
            OFFSET @Offset;
            """,
            parameters);

        return (rows.Select(row => row.ToDomain()).ToList(), totalCount);
    }

    public async Task<(IReadOnlyList<PdfTemplate> Items, long TotalCount)> GetActiveAsync(
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
            FROM "pdf_templates"
            WHERE "IsActive" = true;
            """,
            parameters);

        var rows = await _sql.QueryAsync<PdfTemplateRow>(
            """
            SELECT *
            FROM "pdf_templates"
            WHERE "IsActive" = true
            ORDER BY "CreatedAt" DESC
            LIMIT @PageSize
            OFFSET @Offset;
            """,
            parameters);

        return (rows.Select(row => row.ToDomain()).ToList(), totalCount);
    }

    private sealed class PdfTemplateRow
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string HtmlContent { get; set; } = string.Empty;
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool RequiresSignature { get; set; }
        public int SignaturePage { get; set; }

        public PdfTemplate ToDomain() =>
            PdfTemplate.Restore(
                Id,
                Code,
                Name,
                HtmlContent,
                Version,
                IsActive,
                CreatedAt,
                UpdatedAt,
                RequiresSignature,
                SignaturePage);
    }

    private sealed class PdfTemplateFieldRow
    {
        public Guid Id { get; set; }
        public Guid PdfTemplateId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int Page { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        public PdfTemplateField ToDomain() =>
            PdfTemplateField.Restore(Id, PdfTemplateId, Name, Type, Page, X, Y, Width, Height);
    }
}
