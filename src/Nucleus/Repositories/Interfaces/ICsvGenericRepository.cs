namespace Nucleus.Repositories.Interfaces;

/// <summary>
///     Defines repository operations for reading and writing CSV data.
/// </summary>
public interface ICsvGenericRepository
{
    /// <summary>
    ///     Reads all records from a CSV stream and maps them to the requested record type.
    /// </summary>
    /// <typeparam name="TCsvRecord">The record type to deserialize from the CSV content.</typeparam>
    /// <param name="stream">The input stream that contains CSV content.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The parsed CSV records, or an empty list when the stream is empty.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IReadOnlyList<TCsvRecord>> GetAllRecordsAsync<TCsvRecord>(Stream stream, CancellationToken cancellationToken = default) where TCsvRecord : class;

    /// <summary>
    ///     Writes strongly typed records to a CSV stream.
    /// </summary>
    /// <typeparam name="TCsvRecord">The record type to serialize to CSV.</typeparam>
    /// <param name="records">The records to serialize.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A readable stream positioned at the beginning of the generated CSV content.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<Stream> WriteAllRecordsAsync<TCsvRecord>(IReadOnlyList<TCsvRecord> records, CancellationToken cancellationToken = default) where TCsvRecord : class;

    /// <summary>
    ///     Writes dynamic records to a CSV stream.
    /// </summary>
    /// <param name="records">The records to serialize.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A readable stream positioned at the beginning of the generated CSV content.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<Stream> WriteAllRecordsAsync(IReadOnlyList<dynamic> records, CancellationToken cancellationToken = default);
}