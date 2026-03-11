namespace Nucleus.UnitTests.Repositories;

public class CosmosGenericRepositoryTests
{
    private const string PartitionKeyValue = "Temperature";

    private readonly Mock<Container> _containerMock = new();
    private readonly Mock<Database> _databaseMock = new();
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
        await act.Should().ThrowAsync<OperationCanceledException>();
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
        await act.Should().ThrowAsync<OperationCanceledException>();
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
        await act.Should().ThrowAsync<OperationCanceledException>();
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
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion
}