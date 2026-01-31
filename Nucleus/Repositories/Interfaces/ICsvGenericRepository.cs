namespace Nucleus.Repositories.Interfaces;

public interface ICsvGenericRepository
{
    Task<IReadOnlyList<TCsvRecord>> GetAllRecordsAsync<TCsvRecord>(Stream stream, CancellationToken cancellationToken = default) where TCsvRecord : class;

    Task<Stream> WriteAllRecordsAsync<TCsvRecord>(IReadOnlyList<TCsvRecord> records, CancellationToken cancellationToken = default) where TCsvRecord : class;

    Task<Stream> WriteAllRecordsAsync(IReadOnlyList<dynamic> records, CancellationToken cancellationToken = default);
}
