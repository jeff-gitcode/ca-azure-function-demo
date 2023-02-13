using System.IO;
using Application;
using Infrastructure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Infrastructure.Config;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;

[assembly: FunctionsStartup(typeof(Azure.Function.Startup))]

namespace Azure.Function;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        builder.Services.AddLogging();

        builder.Services.AddSingleton<IConfiguration>(configuration);

        builder.Services.Configure<CosmosDbSettings>(configuration.GetSection(CosmosDbSettings.CosmosDb));

        var cosmosDbConfig = configuration.GetSection(CosmosDbSettings.CosmosDb).Get<CosmosDbSettings>();

        builder.Services.AddInfrastructure(cosmosDbConfig);

        // builder.Services.AddScoped<IUserU, UserRepository>();

        builder.AddSwashBuckle(Assembly.GetExecutingAssembly());

        builder.Services.AddApplication();
    }
}