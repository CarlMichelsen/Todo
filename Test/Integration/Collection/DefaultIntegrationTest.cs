using Test.Integration.Factory;

namespace Test.Integration.Collection;

[CollectionDefinition(nameof(DefaultIntegrationTest))]
public class DefaultIntegrationTest : ICollectionFixture<IntegrationTestFactory>;