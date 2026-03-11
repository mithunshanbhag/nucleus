namespace Nucleus.Repositories.Interfaces;

/// <remarks>
///     The type parameter constraint of 'class' is only required because of this open GitHub issue:
///     - https://github.com/dotnet/runtime/issues/41749
///     Else we could have just used interface types for the type parameter.
/// </remarks>
public interface ICosmosGenericRepository<TEntity> where TEntity : class
{
    Task<IEnumerable<TEntity>> QueryAsync(string querySpec, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> QueryAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> ListAsync(string? filterClause = null, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(string partitionKey, string id, CancellationToken cancellationToken = default);

    Task<TEntity?> GetAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetValuesAsync(QueryDefinition querySpec, CancellationToken cancellationToken = default);

    Task AddAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    Task AddAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    Task UpsertAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    Task UpsertAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(string partitionKey, string id, CancellationToken cancellationToken = default);

    Task DeleteAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default);
}