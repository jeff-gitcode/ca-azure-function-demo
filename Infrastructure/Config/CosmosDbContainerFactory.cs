using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Infrastructure.Config;

public interface ICosmosDbContainerFactory
{
    ICosmosDbContainer GetContainer(string containerName);

    Task<Container> GetContainerAsync(string containerName);

    Task EnsureDbSetupAsync();
}

public class CosmosDbContainerFactory : ICosmosDbContainerFactory
{
    private readonly CosmosDbSettings _options;
    private readonly CosmosClient _client;
    // private readonly string _databaseName;
    // private readonly List<ContainerInfo> _containers;

    public CosmosDbContainerFactory(CosmosClient client, CosmosDbSettings options)
    {
        _client = client;
        _options = options;
    }

    public async Task<Container> GetContainerAsync(string containerName)
    {
        var containerInfo = _options.Containers.FirstOrDefault(c => c.Name == containerName);
        if (containerInfo == null)
        {
            throw new ArgumentException($"Container {containerName} not found");
        }

        var database = _client.GetDatabase(_options.DatabaseName);
        var container = await database.CreateContainerIfNotExistsAsync(containerName, containerInfo.PartitionKey);
        return container.Container;
    }

    public ICosmosDbContainer GetContainer(string containerName)
    {
        if (_options.Containers.Where(x => x.Name == containerName) == null)
        {
            throw new ArgumentException($"Unable to find container: {containerName}");
        }

        return new CosmosDbContainer(_client, _options.DatabaseName, containerName);
    }

    public async Task EnsureDbSetupAsync()
    {
        Microsoft.Azure.Cosmos.DatabaseResponse database = await _client.CreateDatabaseIfNotExistsAsync(_options.DatabaseName);

        foreach (ContainerInfo container in _options.Containers)
        {
            await database.Database.CreateContainerIfNotExistsAsync(container.Name, $"{container.PartitionKey}");
        }
    }
}
