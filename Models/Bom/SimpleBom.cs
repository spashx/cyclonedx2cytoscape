using Newtonsoft.Json;

namespace cdxviz.Models.Bom;

public class SimpleBom
{
    [JsonProperty("metadata")]
    public BomMetadata? Metadata { get; set; }

    [JsonProperty("components")]
    public SimpleComponent[]? Components { get; set; }

    [JsonProperty("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }

    [JsonProperty("vulnerabilities")]
    public SimpleVulnerability[]? Vulnerabilities { get; set; }
}
