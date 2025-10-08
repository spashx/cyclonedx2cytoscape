using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public abstract class AbstractCytoscapeNodeData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("label")]
    public string Label { get; set; } = "";

    [JsonPropertyName("class")]
    public string Class { get; set; } = "";
}
