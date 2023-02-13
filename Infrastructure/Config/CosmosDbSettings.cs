namespace Infrastructure.Config;
public class CosmosDbSettings
{
    public const string CosmosDb = "ConnectionStrings:TestDB";

    public string? EndpointUrl { get; set; }
    public string? PrimaryKey { get; set; }
    public string? DatabaseName { get; set; }
    public List<ContainerInfo>? Containers { get; set; }
}

public class ContainerInfo
{
    public string? Name { get; set; }
    public string? PartitionKey { get; set; }
}