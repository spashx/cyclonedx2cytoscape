using System.Text.Json.Serialization;

namespace CdxViz.Models.Bom;

public class SimpleDependency
{
    [JsonPropertyName("ref")]
    public string? Ref { get; set; }

    [JsonPropertyName("dependsOn")]
    public string[]? DependsOn { get; set; }

    [JsonPropertyName("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }
}
