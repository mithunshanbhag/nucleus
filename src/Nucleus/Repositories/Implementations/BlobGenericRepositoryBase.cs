namespace Nucleus.Repositories.Implementations;

/// <summary>
///     Provides shared Azure Blob Storage repository operations for text blobs.
/// </summary>
public abstract class BlobGenericRepositoryBase : IBlobGenericRepository
{
    protected readonly BlobContainerClient ContainerClient;
    protected readonly BlobServiceClient StorageAccount;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BlobGenericRepositoryBase" /> class.
    /// </summary>
    /// <param name="storageAccount">The blob service client used to access storage resources.</param>
    /// <param name="containerName">The name of the blob container used by the repository.</param>
    protected BlobGenericRepositoryBase(BlobServiceClient storageAccount, string containerName)
    {
        StorageAccount = storageAccount;
        ContainerClient = StorageAccount.GetBlobContainerClient(containerName);
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string blobName, CancellationToken cancellationToken = default)
    {
        bool containerExists = await ContainerClient.ExistsAsync(cancellationToken);

        if (!containerExists) return false;

        // fetch blob
        var blobClient = ContainerClient.GetBlobClient(blobName);

        bool blobExists = await blobClient.ExistsAsync(cancellationToken);

        return blobExists;
    }

    /// <inheritdoc />
    public async Task<string?> GetAsync(string blobName, CancellationToken cancellationToken = default)
    {
        bool containerExists = await ContainerClient.ExistsAsync(cancellationToken);

        if (!containerExists) return null;

        // fetch blob
        var blobClient = ContainerClient.GetBlobClient(blobName);

        bool blobExists = await blobClient.ExistsAsync(cancellationToken);

        if (!blobExists) return null;

        // download blob contents
        var blobDownloadResponse = await blobClient.DownloadContentAsync(cancellationToken);

        return blobDownloadResponse?.Value?.Content?.ToString();
    }

    /// <inheritdoc />
    public async Task<string?> GetUriAsync(string blobName, CancellationToken cancellationToken = default)
    {
        bool containerExists = await ContainerClient.ExistsAsync(cancellationToken);

        if (!containerExists) return null;

        // Hack alert: setting the access level explicitly to 'blob' because the Azure functions output binding
        // which was originally used to create the container sets it by default to 'private'.
        await ContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob, cancellationToken: cancellationToken);

        // fetch blob
        var blobClient = ContainerClient.GetBlobClient(blobName);

        bool blobExists = await blobClient.ExistsAsync(cancellationToken);

        return blobExists ? blobClient.Uri?.ToString() : null;
    }

    /// <inheritdoc />
    public async Task AddAsync(string blobName, string blobContent, CancellationToken cancellationToken = default)
    {
        var blobClient = ContainerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(new BinaryData(blobContent ?? string.Empty), cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateAsync(string blobName, string blobContent, CancellationToken cancellationToken = default)
    {
        var blobClient = ContainerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(new BinaryData(blobContent), true, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string blobName, CancellationToken cancellationToken = default)
    {
        var blobClient = ContainerClient.GetBlobClient(blobName);

        await blobClient.DeleteAsync(cancellationToken: cancellationToken);
    }
}