using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeEdge
{
    [JsonPropertyName("data")]
    public CytoscapeEdgeData Data { get; set; } = new();
}
