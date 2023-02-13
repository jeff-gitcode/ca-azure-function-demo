using Newtonsoft.Json;

namespace Domain;

public abstract class BaseEntity
{
    [JsonProperty(PropertyName = "id")]
    public virtual string? Id { get; set; }
}
