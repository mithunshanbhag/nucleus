namespace Nucleus.Repositories.Implementations;

/// <summary>
///     Provides shared Cosmos DB repository operations for a single entity type.
/// </summary>
/// <typeparam name="TEntity">The persistence model stored in the configured Cosmos DB container.</typeparam>
/// <remarks>
///     Typical usage involves the caller instantiating a Cosmos SDK client as follows:
///     <code>
/// var dbConnectionString = builderContext.Configuration[ConfigKeys.CosmosDbConnectionString];
/// var dbName = builderContext.Configuration[ConfigKeys.MetricsCosmosDbName];
/// var serializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase };
/// var cosmosClientOptions = new CosmosClientOptions { SerializerOptions = serializerOptions };
/// services.AddSingleton(_ => new CosmosClient(dbConnectionString, cosmosClientOptions).GetDatabase(dbName));
/// </code>
/// </remarks>
public abstract class CosmosGenericRepositoryBase<TEntity> : ICosmosGenericRepository<TEntity> where TEntity : class
{
    protected readonly string ContainerName;

    protected readonly Database CosmosDatabase;

    /// <summary>
    ///     Initializes a new instance of the <see cref="CosmosGenericRepositoryBase{TEntity}" /> class.
    /// </summary>
    /// <param name="cosmosDatabase">The Cosmos DB database used by the repository.</param>
    /// <param name="containerName">The name of the container that stores the entities.</param>
    protected CosmosGenericRepositoryBase(Database cosmosDatabase, string containerName)
    {
        ContainerName = containerName;
        CosmosDatabase = cosmosDatabase;
    }

    /// <inheritdoc />
    [Obsolete("Use QueryAcrossPartitionsAsync(string, CancellationToken) to make cross-partition behavior explicit.")]
    public async Task<IEnumerable<TEntity>> QueryAsync(string querySpec, CancellationToken cancellationToken = default)
    {
        return await QueryAcrossPartitionsAsync(querySpec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryAcrossPartitionsAsync(string querySpec,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<TEntity>(new QueryDefinition(querySpec), null, cancellationToken);
    }

    /// <inheritdoc />
    [Obsolete("Use QueryAcrossPartitionsAsync(QueryDefinition, CancellationToken) to make cross-partition behavior explicit.")]
    public async Task<IEnumerable<TEntity>> QueryAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        return await QueryAcrossPartitionsAsync(queryDefinition, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryAcrossPartitionsAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<TEntity>(queryDefinition, null, cancellationToken);
    }

    /// <inheritdoc />
    [Obsolete("Use ListAcrossPartitionsAsync(string?, CancellationToken) to make cross-partition behavior explicit.")]
    public async Task<IEnumerable<TEntity>> ListAsync(string? filterClause = null,
        CancellationToken cancellationToken = default)
    {
        return await ListAcrossPartitionsAsync(filterClause, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListAcrossPartitionsAsync(string? filterClause = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<TEntity>(CreateListQueryDefinition(filterClause), null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryByPartitionAsync(string partitionKey, string querySpec,
        CancellationToken cancellationToken = default)
    {
        return await QueryByPartitionAsync(new PartitionKey(partitionKey), querySpec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryByPartitionAsync(PartitionKey partitionKey, string querySpec,
        CancellationToken cancellationToken = default)
    {
        return await QueryByPartitionAsync(partitionKey, new QueryDefinition(querySpec), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryByPartitionAsync(string partitionKey, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        return await QueryByPartitionAsync(new PartitionKey(partitionKey), queryDefinition, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> QueryByPartitionAsync(PartitionKey partitionKey, QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<TEntity>(queryDefinition, CreateQueryRequestOptions(partitionKey), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListByPartitionAsync(string partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default)
    {
        return await ListByPartitionAsync(new PartitionKey(partitionKey), filterClause, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TEntity>> ListByPartitionAsync(PartitionKey partitionKey, string? filterClause = null,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<TEntity>(
            CreateListQueryDefinition(filterClause),
            CreateQueryRequestOptions(partitionKey),
            cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(string partitionKey, string id, CancellationToken cancellationToken = default)
    {
        return await GetAsync(new PartitionKey(partitionKey), id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TEntity?> GetAsync(PartitionKey partitionKey, string id,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var response = await CosmosDatabase
                .GetContainer(ContainerName)
                .ReadItemAsync<TEntity>(id, partitionKey, cancellationToken: cancellationToken);

            return response?.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    /// <inheritdoc />
    [Obsolete("Use GetValuesAcrossPartitionsAsync(QueryDefinition, CancellationToken) to make cross-partition behavior explicit.")]
    public async Task<IEnumerable<string>> GetValuesAsync(QueryDefinition querySpec,
        CancellationToken cancellationToken = default)
    {
        return await GetValuesAcrossPartitionsAsync(querySpec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetValuesAcrossPartitionsAsync(QueryDefinition querySpec,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<string>(querySpec, null, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetValuesByPartitionAsync(string partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default)
    {
        return await GetValuesByPartitionAsync(new PartitionKey(partitionKey), querySpec, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<string>> GetValuesByPartitionAsync(PartitionKey partitionKey, QueryDefinition querySpec,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteQueryAsync<string>(querySpec, CreateQueryRequestOptions(partitionKey), cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(new PartitionKey(partitionKey), entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .CreateItemAsync(entity, partitionKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> AddRangeAsync(string partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        return await AddRangeAsync(new PartitionKey(partitionKey), entities, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> AddRangeAsync(PartitionKey partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ValidateBatchItems(entities, nameof(entities));

        return await ExecuteBatchAsync(partitionKey, batch =>
        {
            foreach (var entity in entities) batch.CreateItem(entity);
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpsertAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        await UpsertAsync(new PartitionKey(partitionKey), entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpsertAsync(PartitionKey partitionKey, TEntity entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .UpsertItemAsync(entity, partitionKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> UpsertRangeAsync(string partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        return await UpsertRangeAsync(new PartitionKey(partitionKey), entities, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> UpsertRangeAsync(PartitionKey partitionKey, IReadOnlyList<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        ValidateBatchItems(entities, nameof(entities));

        return await ExecuteBatchAsync(partitionKey, batch =>
        {
            foreach (var entity in entities) batch.UpsertItem(entity);
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(string partitionKey, string id, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(new PartitionKey(partitionKey), id, cancellationToken);
    }

    /// <inheritdoc />
    public async Task DeleteAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .DeleteItemAsync<TEntity>(id, partitionKey, cancellationToken: cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> DeleteRangeAsync(string partitionKey, IReadOnlyList<string> ids,
        CancellationToken cancellationToken = default)
    {
        return await DeleteRangeAsync(new PartitionKey(partitionKey), ids, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TransactionalBatchResponse> DeleteRangeAsync(PartitionKey partitionKey, IReadOnlyList<string> ids,
        CancellationToken cancellationToken = default)
    {
        ValidateBatchItems(ids, nameof(ids));

        return await ExecuteBatchAsync(partitionKey, batch =>
        {
            foreach (var id in ids)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(id);
                batch.DeleteItem(id);
            }
        }, cancellationToken);
    }

    private Container Container => CosmosDatabase.GetContainer(ContainerName);

    private static QueryDefinition CreateListQueryDefinition(string? filterClause)
    {
        var querySpec = "select * from c";

        if (!string.IsNullOrWhiteSpace(filterClause)) querySpec = $"{querySpec} where {filterClause}";

        return new QueryDefinition(querySpec);
    }

    private static QueryRequestOptions CreateQueryRequestOptions(PartitionKey partitionKey)
    {
        return new QueryRequestOptions
        {
            PartitionKey = partitionKey
        };
    }

    private static void ValidateBatchItems<TItem>(IReadOnlyList<TItem> items, string paramName)
    {
        ArgumentNullException.ThrowIfNull(items);

        if (items.Count == 0) throw new ArgumentException("At least one item must be supplied.", paramName);
    }

    private async Task<TransactionalBatchResponse> ExecuteBatchAsync(
        PartitionKey partitionKey,
        Action<TransactionalBatch> configureBatch,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var batch = Container.CreateTransactionalBatch(partitionKey);
        configureBatch(batch);

        return await batch.ExecuteAsync(cancellationToken);
    }

    private async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TReturn>(
        QueryDefinition queryDefinition,
        QueryRequestOptions? queryRequestOptions,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(queryDefinition);

        cancellationToken.ThrowIfCancellationRequested();

        using var queryIterator = Container.GetItemQueryIterator<TReturn>(
            queryDefinition,
            continuationToken: null,
            requestOptions: queryRequestOptions);

        var results = new List<TReturn>();
        while (queryIterator.HasMoreResults)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var response = await queryIterator.ReadNextAsync(cancellationToken);
            results.AddRange(response.ToList());
        }

        return results;
    }
}
