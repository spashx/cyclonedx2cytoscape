using Newtonsoft.Json;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeEdge
{
    [JsonProperty("data")]
    public CytoscapeEdgeData Data { get; set; } = new();
}
