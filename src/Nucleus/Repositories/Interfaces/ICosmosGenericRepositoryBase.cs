namespace Nucleus.Repositories.Interfaces;

/// <summary>
///     Defines generic repository operations for a Cosmos DB container.
/// </summary>
/// <typeparam name="TEntity">The persistence model stored in the container.</typeparam>
/// <remarks>
///     The type parameter constraint of 'class' is only required because of this open GitHub issue:
///     - https://github.com/dotnet/runtime/issues/41749
///     Else we could have just used interface types for the type parameter.
/// </remarks>
public interface ICosmosGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    ///     Executes a Cosmos DB SQL query string across partitions and returns the matching entities.
    /// </summary>
    /// <param name="querySpec">The Cosmos DB SQL query text to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> QueryAcrossPartitionsAsync(string querySpec, CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> QueryAcrossPartitionsAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a Cosmos DB SQL query string against a single partition and returns the matching entities.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the query.</param>
    /// <param name="querySpec">The Cosmos DB SQL query text to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> QueryByPartitionAsync(string partitionKey, string querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a Cosmos DB SQL query string against a single partition and returns the matching entities.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the query.</param>
    /// <param name="querySpec">The Cosmos DB SQL query text to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> QueryByPartitionAsync(PartitionKey partitionKey, string querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a Cosmos DB query definition against a single partition and returns the matching entities.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the query.</param>
    /// <param name="queryDefinition">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> QueryByPartitionAsync(string partitionKey, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a Cosmos DB query definition against a single partition and returns the matching entities.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the query.</param>
    /// <param name="queryDefinition">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> QueryByPartitionAsync(PartitionKey partitionKey, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<TEntity>> ListAcrossPartitionsAsync(string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists entities in a single partition, optionally applying a SQL filter clause.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the query.</param>
    /// <param name="filterClause">An optional SQL filter clause appended to the base query.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> ListByPartitionAsync(string partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Lists entities in a single partition, optionally applying a SQL filter clause.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the query.</param>
    /// <param name="filterClause">An optional SQL filter clause appended to the base query.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The entities returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TEntity>> ListByPartitionAsync(PartitionKey partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an entity by its partition key value and identifier.
    /// </summary>
    /// <param name="partitionKey">The partition key value of the entity to retrieve.</param>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The matching entity, or <see langword="null" /> when it does not exist.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TEntity?> GetAsync(string partitionKey, string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets an entity by its partition key and identifier.
    /// </summary>
    /// <param name="partitionKey">The partition key of the entity to retrieve.</param>
    /// <param name="id">The identifier of the entity to retrieve.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The matching entity, or <see langword="null" /> when it does not exist.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TEntity?> GetAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TValue>> GetScalarValuesAcrossPartitionsAsync<TValue>(QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a scalar query in a single partition and materializes the returned values as
    ///     <typeparamref name="TValue" />.
    /// </summary>
    /// <typeparam name="TValue">The scalar value type returned by the query.</typeparam>
    /// <param name="partitionKey">The partition key value that scopes the query.</param>
    /// <param name="querySpec">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The scalar values returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TValue>> GetScalarValuesByPartitionAsync<TValue>(string partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a scalar query in a single partition and materializes the returned values as
    ///     <typeparamref name="TValue" />.
    /// </summary>
    /// <typeparam name="TValue">The scalar value type returned by the query.</typeparam>
    /// <param name="partitionKey">The partition key that scopes the query.</param>
    /// <param name="querySpec">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The scalar values returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<TValue>> GetScalarValuesByPartitionAsync<TValue>(PartitionKey partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetValuesAcrossPartitionsAsync(QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a query in a single partition that returns scalar string values.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the query.</param>
    /// <param name="querySpec">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The string values returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<string>> GetValuesByPartitionAsync(string partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes a query in a single partition that returns scalar string values.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the query.</param>
    /// <param name="querySpec">The query definition to execute.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The string values returned by the query, or an empty collection when nothing matches.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<IEnumerable<string>> GetValuesByPartitionAsync(PartitionKey partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAcrossPartitionsAsync(string? filterClause = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether any entities match the supplied filter in a single partition.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the lookup.</param>
    /// <param name="filterClause">An optional SQL predicate without the leading <c>where</c> keyword.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns><see langword="true" /> when at least one entity matches; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<bool> ExistsByPartitionAsync(string partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Determines whether any entities match the supplied filter in a single partition.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the lookup.</param>
    /// <param name="filterClause">An optional SQL predicate without the leading <c>where</c> keyword.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns><see langword="true" /> when at least one entity matches; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<bool> ExistsByPartitionAsync(PartitionKey partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    Task<long> CountAcrossPartitionsAsync(string? filterClause = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts the entities that match the supplied filter in a single partition.
    /// </summary>
    /// <param name="partitionKey">The partition key value that scopes the count.</param>
    /// <param name="filterClause">An optional SQL predicate without the leading <c>where</c> keyword.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The number of matching entities.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<long> CountByPartitionAsync(string partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Counts the entities that match the supplied filter in a single partition.
    /// </summary>
    /// <param name="partitionKey">The partition key that scopes the count.</param>
    /// <param name="filterClause">An optional SQL predicate without the leading <c>where</c> keyword.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The number of matching entities.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<long> CountByPartitionAsync(PartitionKey partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new entity in the container using the supplied partition key value.
    /// </summary>
    /// <param name="partitionKey">The partition key value to use when creating the entity.</param>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the entity has been created.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task AddAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a new entity in the container using the supplied partition key.
    /// </summary>
    /// <param name="partitionKey">The partition key to use when creating the entity.</param>
    /// <param name="entity">The entity to create.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the entity has been created.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task AddAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key value to use when creating the entities.</param>
    /// <param name="entities">The entities to create in the batch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="entities" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> AddRangeAsync(string partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key to use when creating the entities.</param>
    /// <param name="entities">The entities to create in the batch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="entities" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> AddRangeAsync(PartitionKey partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or replaces an entity in the container using the supplied partition key value.
    /// </summary>
    /// <param name="partitionKey">The partition key value to use when upserting the entity.</param>
    /// <param name="entity">The entity to create or replace.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the entity has been upserted.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task UpsertAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or replaces an entity in the container using the supplied partition key.
    /// </summary>
    /// <param name="partitionKey">The partition key to use when upserting the entity.</param>
    /// <param name="entity">The entity to create or replace.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the entity has been upserted.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task UpsertAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or replaces multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key value to use when upserting the entities.</param>
    /// <param name="entities">The entities to create or replace in the batch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="entities" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> UpsertRangeAsync(string partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates or replaces multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key to use when upserting the entities.</param>
    /// <param name="entities">The entities to create or replace in the batch.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="entities" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> UpsertRangeAsync(PartitionKey partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an entity by its partition key value and identifier.
    /// </summary>
    /// <param name="partitionKey">The partition key value of the entity to delete.</param>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    ///     <see langword="true" /> when the entity existed and was deleted; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<bool> DeleteIfExistsAsync(string partitionKey, string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an entity by its partition key and identifier when it exists.
    /// </summary>
    /// <param name="partitionKey">The partition key of the entity to delete.</param>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>
    ///     <see langword="true" /> when the entity existed and was deleted; otherwise, <see langword="false" />.
    /// </returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<bool> DeleteIfExistsAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an entity by its partition key value and identifier.
    /// </summary>
    /// <param name="partitionKey">The partition key value of the entity to delete.</param>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the delete request has finished.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task DeleteAsync(string partitionKey, string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes an entity by its partition key and identifier.
    /// </summary>
    /// <param name="partitionKey">The partition key of the entity to delete.</param>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>A task that completes when the delete request has finished.</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task DeleteAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key value of the entities to delete.</param>
    /// <param name="ids">The identifiers of the entities to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="ids" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ids" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> DeleteRangeAsync(string partitionKey, IReadOnlyList<string> ids,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Deletes multiple entities in a single partition using a transactional batch.
    /// </summary>
    /// <param name="partitionKey">The partition key of the entities to delete.</param>
    /// <param name="ids">The identifiers of the entities to delete.</param>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    /// <returns>The transactional batch response returned by Cosmos DB.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="ids" /> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="ids" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<TransactionalBatchResponse> DeleteRangeAsync(PartitionKey partitionKey, IReadOnlyList<string> ids,
        CancellationToken cancellationToken = default);
}