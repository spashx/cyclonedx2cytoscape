using System.Text.Json.Serialization;

namespace CdxViz.Models.Bom;

public class SimpleComponent
{
    [JsonPropertyName("bom-ref")]
    public string? BomRef { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("purl")]
    public string? Purl { get; set; }
    
    [JsonPropertyName("licenses")]
    public SimpleLicense[]? Licenses { get; set; }
}
