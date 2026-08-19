namespace LocationService.Application.Common.Interfaces;

public interface ILocationReadStore<TResponse>
{
    Task<TResponse?> GetByKeyAsync(
        object parameters,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TResponse>> GetPagedAsync(
        object parameters,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        object? parameters = null,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        object parameters,
        CancellationToken cancellationToken = default);
}
