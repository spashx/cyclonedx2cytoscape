using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class SimpleComponent
{
    [JsonProperty("bom-ref")]
    public string? BomRef { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("version")]
    public string? Version { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("group")]
    public string? Group { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("purl")]
    public string? Purl { get; set; }
    
    [JsonProperty("licenses")]
    public SimpleLicense[]? Licenses { get; set; }
}
