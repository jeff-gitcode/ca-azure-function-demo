using Microsoft.Azure.Cosmos;

namespace Infrastructure.Config;
public interface ICosmosDbContainer
{
    /// <summary>
    ///     Instance of Azure Cosmos DB Container class
    /// </summary>
    Container _container { get; }
}

public class CosmosDbContainer : ICosmosDbContainer
{
    public Container _container { get; }

    public CosmosDbContainer(CosmosClient cosmosClient,
                             string databaseName,
                             string containerName)
    {
        this._container = cosmosClient.GetContainer(databaseName, containerName);
    }
}