namespace DoctorService.Application.Common.Interfaces;

public interface ISequenceStore
{
    Task<int> NextAsync(
        string table,
        string sequenceColumn,
        CancellationToken cancellationToken,
        params (string Column, object Value)[] scope);
}
