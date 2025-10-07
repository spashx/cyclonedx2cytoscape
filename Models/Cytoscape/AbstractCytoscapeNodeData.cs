using Newtonsoft.Json;

namespace CdxViz.Models.Cytoscape;

public abstract class AbstractCytoscapeNodeData
{
    [JsonProperty("id")]
    public string Id { get; set; } = "";

    [JsonProperty("label")]
    public string Label { get; set; } = "";

    [JsonProperty("class")]
    public string Class { get; set; } = "";
}
