using Application.Interfaces.SPI;
using Infrastructure.Config;
using Infrastructure.Persistance;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure;

public static class RegisterService
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, CosmosDbSettings cosmosDbSettings)
    {
        var client = new CosmosClient(cosmosDbSettings.EndpointUrl, cosmosDbSettings.PrimaryKey);
        var cosmosDbClientFactory = new CosmosDbContainerFactory(client, cosmosDbSettings);
        services.AddSingleton<ICosmosDbContainerFactory>(cosmosDbClientFactory);
        // services.AddSingleton<IConfigConstants, ConfigConstants>();
        services.AddScoped<IUserRepository, UserCosmosDbRepository>();

        return services;
    }

}
