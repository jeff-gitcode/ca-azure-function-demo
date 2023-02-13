
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Config;

public interface IConfigConstants
{
    string USER_CONTAINER { get; }
}

public class ConfigConstants : IConfigConstants
{
    public IConfiguration Configuration { get; }
    private readonly CosmosDbSettings cosmosDbConfig;

    public ConfigConstants(IConfiguration configuration)
    {
        this.Configuration = configuration;
        this.cosmosDbConfig = this.Configuration.GetSection(CosmosDbSettings.CosmosDb).Get<CosmosDbSettings>();
    }


    public string USER_CONTAINER => this.cosmosDbConfig?.Containers[0]?.Name;
}