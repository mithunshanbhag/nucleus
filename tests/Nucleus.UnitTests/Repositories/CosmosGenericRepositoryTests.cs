namespace Nucleus.UnitTests.Repositories;

public class CosmosGenericRepositoryTests
{
    private const string PartitionKeyValue = "Temperature";

    private readonly Mock<Container> _containerMock = new();
    private readonly Mock<Database> _databaseMock = new();
    private readonly Mock<TransactionalBatch> _transactionalBatchMock = new();
    private readonly ICosmosTestRepository _sut;

    public CosmosGenericRepositoryTests()
    {
        _databaseMock
            .Setup(db => db.GetContainer(It.IsAny<string>()))
            .Returns(_containerMock.Object);

        _sut = new CosmosTestRepository(_databaseMock.Object);
    }

    private static TestDao CreateEntity()
    {
        return new TestDao
        {
            Id = DateTime.UtcNow.ToString("o")
        };
    }

    #region Positive Cases

    [Fact]
    public async Task AddAsync_ValidRequest_Returns()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.AddAsync(PartitionKeyValue, entity, cancellationToken);

        // Assert
        _containerMock.Verify(
            c => c.CreateItemAsync(entity, new PartitionKey(PartitionKeyValue), null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpsertAsync_ValidRequest_Returns()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.UpsertAsync(PartitionKeyValue, entity, cancellationToken);

        // Assert
        _containerMock.Verify(
            c => c.UpsertItemAsync(entity, new PartitionKey(PartitionKeyValue), null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidRequest_Returns()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.DeleteAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        _containerMock.Verify(
            c => c.DeleteItemAsync<TestDao>(entity.Id, new PartitionKey(PartitionKeyValue), null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteIfExistsAsync_ExistingItem_ReturnsTrue()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _sut.DeleteIfExistsAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        Assert.True(result);
        _containerMock.Verify(
            c => c.DeleteItemAsync<TestDao>(entity.Id, new PartitionKey(PartitionKeyValue), null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetAsync_ValidRequest_Returns()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.GetAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        _containerMock.Verify(
            c => c.ReadItemAsync<TestDao>(entity.Id, new PartitionKey(PartitionKeyValue), null, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task QueryAcrossPartitionsAsync_QueryDefinition_ReturnsResults()
    {
        // Arrange
        var records = new[] { CreateEntity() };
        var cancellationToken = CancellationToken.None;
        var queryDefinition = new QueryDefinition("select * from c where c.id = @id")
            .WithParameter("@id", records[0].Id);
        var iteratorMock = CreateFeedIterator(records, cancellationToken);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<TestDao>(queryDefinition, null, null))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.QueryAcrossPartitionsAsync(queryDefinition, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Same(records[0], result.Single());
        _containerMock.Verify(
            c => c.GetItemQueryIterator<TestDao>(queryDefinition, null, null),
            Times.Once);
    }

    [Fact]
    public async Task ListByPartitionAsync_ValidRequest_UsesPartitionScopedQuery()
    {
        // Arrange
        var records = new[] { CreateEntity() };
        var cancellationToken = CancellationToken.None;
        var iteratorMock = CreateFeedIterator(records, cancellationToken);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<TestDao>(
                It.Is<QueryDefinition>(q => q.QueryText == "select * from c where c.id = 'abc'"),
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.ListByPartitionAsync(PartitionKeyValue, "c.id = 'abc'", cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Same(records[0], result.Single());
        _containerMock.Verify(
            c => c.GetItemQueryIterator<TestDao>(
                It.Is<QueryDefinition>(q => q.QueryText == "select * from c where c.id = 'abc'"),
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))),
            Times.Once);
    }

    [Fact]
    public async Task GetScalarValuesByPartitionAsync_ValidRequest_UsesPartitionScopedQuery()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var expectedValue = 42L;
        var iteratorMock = CreateFeedIterator<long>([expectedValue], cancellationToken);
        var queryDefinition = new QueryDefinition("select value count(1) from c where c.type = @type")
            .WithParameter("@type", PartitionKeyValue);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<long>(
                queryDefinition,
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.GetScalarValuesByPartitionAsync<long>(PartitionKeyValue, queryDefinition, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedValue, result.Single());
        _containerMock.Verify(
            c => c.GetItemQueryIterator<long>(
                queryDefinition,
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))),
            Times.Once);
    }

    [Fact]
    public async Task GetValuesByPartitionAsync_ValidRequest_UsesPartitionScopedQuery()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var expectedValue = Guid.NewGuid().ToString("N");
        var iteratorMock = CreateFeedIterator<string>([expectedValue], cancellationToken);
        var queryDefinition = new QueryDefinition("select value c.id from c where c.type = @type")
            .WithParameter("@type", PartitionKeyValue);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<string>(
                queryDefinition,
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.GetValuesByPartitionAsync(PartitionKeyValue, queryDefinition, cancellationToken);

        // Assert
        Assert.Single(result);
        Assert.Equal(expectedValue, result.Single());
        _containerMock.Verify(
            c => c.GetItemQueryIterator<string>(
                queryDefinition,
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))),
            Times.Once);
    }

    [Fact]
    public async Task ExistsByPartitionAsync_MatchingRecords_ReturnsTrue()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;
        var iteratorMock = CreateFeedIterator<long>([1L], cancellationToken);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<long>(
                It.Is<QueryDefinition>(q => q.QueryText == "select value count(1) from c where c.id = 'abc'"),
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.ExistsByPartitionAsync(PartitionKeyValue, "c.id = 'abc'", cancellationToken);

        // Assert
        Assert.True(result);
        _containerMock.Verify(
            c => c.GetItemQueryIterator<long>(
                It.Is<QueryDefinition>(q => q.QueryText == "select value count(1) from c where c.id = 'abc'"),
                null,
                It.Is<QueryRequestOptions>(options =>
                    options != null &&
                    options.PartitionKey.HasValue &&
                    options.PartitionKey.Value.Equals(new PartitionKey(PartitionKeyValue)))),
            Times.Once);
    }

    [Fact]
    public async Task CountAcrossPartitionsAsync_ValidRequest_ReturnsCount()
    {
        // Arrange
        const long expectedCount = 3;
        var cancellationToken = CancellationToken.None;
        var iteratorMock = CreateFeedIterator<long>([expectedCount], cancellationToken);

        _containerMock
            .Setup(c => c.GetItemQueryIterator<long>(
                It.Is<QueryDefinition>(q => q.QueryText == "select value count(1) from c where c.id = 'abc'"),
                null,
                null))
            .Returns(iteratorMock.Object);

        // Act
        var result = await _sut.CountAcrossPartitionsAsync("c.id = 'abc'", cancellationToken);

        // Assert
        Assert.Equal(expectedCount, result);
        _containerMock.Verify(
            c => c.GetItemQueryIterator<long>(
                It.Is<QueryDefinition>(q => q.QueryText == "select value count(1) from c where c.id = 'abc'"),
                null,
                null),
            Times.Once);
    }

    [Fact]
    public async Task AddRangeAsync_ValidRequest_ReturnsBatchResponse()
    {
        // Arrange
        var entities = new[] { CreateEntity(), CreateEntity() };
        var cancellationToken = CancellationToken.None;
        var responseMock = new Mock<TransactionalBatchResponse>();

        _containerMock
            .Setup(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.CreateItem(entities[0], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.CreateItem(entities[1], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.ExecuteAsync(cancellationToken))
            .ReturnsAsync(responseMock.Object);

        // Act
        var result = await _sut.AddRangeAsync(PartitionKeyValue, entities, cancellationToken);

        // Assert
        Assert.Same(responseMock.Object, result);
        _containerMock.Verify(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)), Times.Once);
        _transactionalBatchMock.Verify(b => b.CreateItem(entities[0], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.CreateItem(entities[1], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.ExecuteAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task UpsertRangeAsync_ValidRequest_ReturnsBatchResponse()
    {
        // Arrange
        var entities = new[] { CreateEntity(), CreateEntity() };
        var cancellationToken = CancellationToken.None;
        var responseMock = new Mock<TransactionalBatchResponse>();

        _containerMock
            .Setup(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.UpsertItem(entities[0], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.UpsertItem(entities[1], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.ExecuteAsync(cancellationToken))
            .ReturnsAsync(responseMock.Object);

        // Act
        var result = await _sut.UpsertRangeAsync(PartitionKeyValue, entities, cancellationToken);

        // Assert
        Assert.Same(responseMock.Object, result);
        _containerMock.Verify(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)), Times.Once);
        _transactionalBatchMock.Verify(b => b.UpsertItem(entities[0], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.UpsertItem(entities[1], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.ExecuteAsync(cancellationToken), Times.Once);
    }

    [Fact]
    public async Task DeleteRangeAsync_ValidRequest_ReturnsBatchResponse()
    {
        // Arrange
        var ids = new[] { Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N") };
        var cancellationToken = CancellationToken.None;
        var responseMock = new Mock<TransactionalBatchResponse>();

        _containerMock
            .Setup(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.DeleteItem(ids[0], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.DeleteItem(ids[1], null))
            .Returns(_transactionalBatchMock.Object);
        _transactionalBatchMock
            .Setup(b => b.ExecuteAsync(cancellationToken))
            .ReturnsAsync(responseMock.Object);

        // Act
        var result = await _sut.DeleteRangeAsync(PartitionKeyValue, ids, cancellationToken);

        // Assert
        Assert.Same(responseMock.Object, result);
        _containerMock.Verify(c => c.CreateTransactionalBatch(new PartitionKey(PartitionKeyValue)), Times.Once);
        _transactionalBatchMock.Verify(b => b.DeleteItem(ids[0], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.DeleteItem(ids[1], null), Times.Once);
        _transactionalBatchMock.Verify(b => b.ExecuteAsync(cancellationToken), Times.Once);
    }

    #endregion

    #region Negative Cases

    [Fact]
    public async Task AddAsync_CancellationRequest_ThrowsException()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = new CancellationToken(true);

        // Act
        var act = () => _sut.AddAsync(PartitionKeyValue, entity, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task UpsertAsync_CancellationRequest_ThrowsException()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = new CancellationToken(true);

        // Act
        var act = () => _sut.UpsertAsync(PartitionKeyValue, entity, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteAsync_CancellationRequest_ThrowsException()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = new CancellationToken(true);

        // Act
        var act = () => _sut.DeleteAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task DeleteIfExistsAsync_CancellationRequest_ThrowsException()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = new CancellationToken(true);

        // Act
        var act = () => _sut.DeleteIfExistsAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    [Fact]
    public async Task GetAsync_CancellationRequest_ThrowsException()
    {
        // Arrange
        var entity = CreateEntity();
        var cancellationToken = new CancellationToken(true);

        // Act
        var act = () => _sut.GetAsync(PartitionKeyValue, entity.Id!, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }

    #endregion

    #region Boundary Cases

    [Fact]
    public async Task AddRangeAsync_EmptyEntities_ThrowsException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var act = () => _sut.AddRangeAsync(PartitionKeyValue, Array.Empty<TestDao>(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task DeleteRangeAsync_EmptyIds_ThrowsException()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var act = () => _sut.DeleteRangeAsync(PartitionKeyValue, Array.Empty<string>(), cancellationToken);

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(act);
    }

    [Fact]
    public async Task DeleteIfExistsAsync_MissingItem_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N");
        var cancellationToken = CancellationToken.None;

        _containerMock
            .Setup(c => c.DeleteItemAsync<TestDao>(id, new PartitionKey(PartitionKeyValue), null, cancellationToken))
            .ThrowsAsync(CreateCosmosException(System.Net.HttpStatusCode.NotFound));

        // Act
        var result = await _sut.DeleteIfExistsAsync(PartitionKeyValue, id, cancellationToken);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_MissingItem_ThrowsException()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N");
        var cancellationToken = CancellationToken.None;

        _containerMock
            .Setup(c => c.DeleteItemAsync<TestDao>(id, new PartitionKey(PartitionKeyValue), null, cancellationToken))
            .ThrowsAsync(CreateCosmosException(System.Net.HttpStatusCode.NotFound));

        // Act
        var act = () => _sut.DeleteAsync(PartitionKeyValue, id, cancellationToken);

        // Assert
        await Assert.ThrowsAsync<CosmosException>(act);
    }

    #endregion

    private static CosmosException CreateCosmosException(System.Net.HttpStatusCode statusCode)
    {
        return new CosmosException("Cosmos operation failed.", statusCode, 0, Guid.NewGuid().ToString("N"), 0);
    }

    private static Mock<FeedIterator<TItem>> CreateFeedIterator<TItem>(IReadOnlyList<TItem> items, CancellationToken cancellationToken)
    {
        var feedResponseMock = CreateFeedResponse(items);
        var iteratorMock = new Mock<FeedIterator<TItem>>();

        iteratorMock.SetupSequence(x => x.HasMoreResults)
            .Returns(true)
            .Returns(false);
        iteratorMock
            .Setup(x => x.ReadNextAsync(cancellationToken))
            .ReturnsAsync(feedResponseMock.Object);

        return iteratorMock;
    }

    private static Mock<FeedResponse<TItem>> CreateFeedResponse<TItem>(IReadOnlyList<TItem> items)
    {
        var feedResponseMock = new Mock<FeedResponse<TItem>>();

        feedResponseMock.Setup(x => x.GetEnumerator()).Returns(() => items.GetEnumerator());
        feedResponseMock
            .As<System.Collections.IEnumerable>()
            .Setup(x => x.GetEnumerator())
            .Returns(() => items.GetEnumerator());

        return feedResponseMock;
    }
}
