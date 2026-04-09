namespace Nucleus.Repositories.Implementations;

/// <summary>
///     Provides shared CSV repository operations backed by <see cref="CsvHelper" />.
/// </summary>
/// <param name="csvConfig">The CSV configuration used for reading and writing records.</param>
public abstract class CsvGenericRepositoryBase(CsvConfiguration csvConfig) : ICsvGenericRepository
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<TCsvRecord>> GetAllRecordsAsync<TCsvRecord>(Stream stream, CancellationToken cancellationToken = default) where TCsvRecord : class
    {
        if (stream == null || stream.Length == 0) return [];

        // Copy the stream to a new memory stream to ensure it can be read multiple times
        using var memoryStream = new MemoryStream();

        await stream.CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0; // Reset the position to the beginning of the stream
        memoryStream.Seek(0, SeekOrigin.Begin);

        //
        using var reader = new StreamReader(memoryStream);

        using var csv = new CsvReader(reader, csvConfig);

        var records = await csv.GetRecordsAsync<TCsvRecord>(cancellationToken).ToListAsync(cancellationToken);

        return records;
    }

    /// <inheritdoc />
    public async Task<Stream> WriteAllRecordsAsync<TCsvRecord>(IReadOnlyList<TCsvRecord> records, CancellationToken cancellationToken = default) where TCsvRecord : class
    {
        return await NewMethodAsync(records, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Stream> WriteAllRecordsAsync(IReadOnlyList<dynamic> records, CancellationToken cancellationToken = default)
    {
        return await NewMethodAsync(records, cancellationToken);
    }

    private async Task<Stream> NewMethodAsync(IReadOnlyList<dynamic> records, CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await using var writer = new StreamWriter(memoryStream);
        await using var csv = new CsvWriter(writer, csvConfig);

        await csv.WriteRecordsAsync(records, cancellationToken);
        await csv.FlushAsync(); // Ensure all data is written

        memoryStream.Position = 0; // Reset position before copying

        // Copy the stream to a new memory stream to ensure it can be read multiple times
        var stream = new MemoryStream();

        await memoryStream.CopyToAsync(stream, cancellationToken);

        stream.Position = 0; // Reset the position to the beginning of the stream
        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
