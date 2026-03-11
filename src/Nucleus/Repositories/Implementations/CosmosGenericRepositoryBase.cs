namespace Nucleus.Repositories.Implementations;

public abstract class CosmosGenericRepositoryBase<TEntity> : ICosmosGenericRepository<TEntity> where TEntity : class
{
    protected readonly string ContainerName;

    protected readonly Database CosmosDatabase;

    protected CosmosGenericRepositoryBase(Database cosmosDatabase, string containerName)
    {
        ContainerName = containerName;
        CosmosDatabase = cosmosDatabase;
    }

    public async Task<IEnumerable<TEntity>> QueryAsync(string querySpec, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await ExecuteQueryAsync<TEntity>(querySpec, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> QueryAsync(QueryDefinition queryDefinition,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await ExecuteQueryAsync<TEntity>(queryDefinition.QueryText, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> ListAsync(string? filterClause = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var querySpec = "select * from c";

        if (!string.IsNullOrWhiteSpace(filterClause)) querySpec = $"{querySpec} where {filterClause}";

        return await ExecuteQueryAsync<TEntity>(querySpec, cancellationToken);
    }

    public async Task<TEntity?> GetAsync(string partitionKey, string id, CancellationToken cancellationToken = default)
    {
        return await GetAsync(new PartitionKey(partitionKey), id, cancellationToken);
    }

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

    public async Task<IEnumerable<string>> GetValuesAsync(QueryDefinition querySpec,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        return await ExecuteQueryAsync<string>(querySpec.QueryText, cancellationToken);
    }

    public async Task AddAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        await AddAsync(new PartitionKey(partitionKey), entity, cancellationToken);
    }

    public async Task AddAsync(PartitionKey partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .CreateItemAsync(entity, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task UpsertAsync(string partitionKey, TEntity entity, CancellationToken cancellationToken = default)
    {
        await UpsertAsync(new PartitionKey(partitionKey), entity, cancellationToken);
    }

    public async Task UpsertAsync(PartitionKey partitionKey, TEntity entity,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .UpsertItemAsync(entity, partitionKey, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string partitionKey, string id, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(new PartitionKey(partitionKey), id, cancellationToken);
    }

    public async Task DeleteAsync(PartitionKey partitionKey, string id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await CosmosDatabase
            .GetContainer(ContainerName)
            .DeleteItemAsync<TEntity>(id, partitionKey, cancellationToken: cancellationToken);
    }

    private async Task<IEnumerable<TReturn>> ExecuteQueryAsync<TReturn>(string querySpec,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var queryIterator = CosmosDatabase
            .GetContainer(ContainerName)
            .GetItemQueryIterator<TReturn>(new QueryDefinition(querySpec));

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