using Test.Integration.Factory;

namespace Test.Integration.Collection;

[CollectionDefinition(nameof(DefaultIntegrationTestCollection))]
public class DefaultIntegrationTestCollection : ICollectionFixture<IntegrationTestFactory>;