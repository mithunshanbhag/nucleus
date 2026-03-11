namespace Nucleus.Repositories.Interfaces;

public interface IBlobGenericRepository
{
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);

    Task<string?> GetAsync(string blobName, CancellationToken cancellationToken = default);

    Task<string?> GetUriAsync(string blobName, CancellationToken cancellationToken = default);

    Task AddAsync(string blobName, string blobContent, CancellationToken cancellationToken = default);

    Task UpdateAsync(string blobName, string blobContent, CancellationToken cancellationToken = default);

    Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);
}