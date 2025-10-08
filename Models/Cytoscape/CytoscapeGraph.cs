using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeGraph
{
    [JsonPropertyName("elements")]
    public CytoscapeElements Elements { get; set; } = new();
}
