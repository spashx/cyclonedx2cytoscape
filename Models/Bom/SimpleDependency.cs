using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class SimpleDependency
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }

    [JsonProperty("dependsOn")]
    public string[]? DependsOn { get; set; }

    [JsonProperty("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }
}
