using System.Text.Json.Serialization;

namespace cdxviz.Models.Bom;

public class SimpleBom
{
    [JsonPropertyName("metadata")]
    public BomMetadata? Metadata { get; set; }

    [JsonPropertyName("components")]
    public SimpleComponent[]? Components { get; set; }

    [JsonPropertyName("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }

    [JsonPropertyName("vulnerabilities")]
    public SimpleVulnerability[]? Vulnerabilities { get; set; }
}
