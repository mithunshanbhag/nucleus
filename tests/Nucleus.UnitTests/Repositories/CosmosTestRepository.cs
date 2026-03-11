namespace Nucleus.UnitTests.Repositories;

public class CosmosTestRepository(Database cosmosDatabase)
    : CosmosGenericRepositoryBase<TestDao>(cosmosDatabase, "testContainer"), ICosmosTestRepository;