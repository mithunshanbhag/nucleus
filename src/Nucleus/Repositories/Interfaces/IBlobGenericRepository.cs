namespace Nucleus.Repositories.Interfaces;

/// <summary>
///     Defines repository operations for reading and writing text blobs in Azure Blob Storage.
/// </summary>
public interface IBlobGenericRepository
{
    /// <summary>
    ///     Determines whether a blob exists in the configured container.
    /// </summary>
    /// <param name="blobName">The name of the blob to look up.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns><see langword="true" /> when the blob exists; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets the text content of a blob.
    /// </summary>
    /// <param name="blobName">The name of the blob to read.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The blob content when the blob exists; otherwise, <see langword="null" />.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<string?> GetAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets a public URI for a blob when it exists.
    /// </summary>
    /// <param name="blobName">The name of the blob whose URI should be returned.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The blob URI when the blob exists; otherwise, <see langword="null" />.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<string?> GetUriAsync(string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Uploads text content as a blob.
    /// </summary>
    /// <param name="blobName">The name of the blob to create.</param>
    /// <param name="blobContent">The text content to upload.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the blob has been uploaded.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task AddAsync(string blobName, string blobContent, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Uploads text content and overwrites any existing blob with the same name.
    /// </summary>
    /// <param name="blobName">The name of the blob to update.</param>
    /// <param name="blobContent">The replacement text content.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the blob has been updated.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task UpdateAsync(string blobName, string blobContent, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes a blob from the configured container.
    /// </summary>
    /// <param name="blobName">The name of the blob to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the delete request has finished.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task DeleteAsync(string blobName, CancellationToken cancellationToken = default);
}