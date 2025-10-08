using System.Text.Json.Serialization;

namespace CdxViz.Models.Cytoscape;

public class CytoscapeEdgeData
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("source")]
    public string Source { get; set; } = "";

    [JsonPropertyName("target")]
    public string Target { get; set; } = "";
    
    [JsonPropertyName("class")]
    public string Class { get; set; } = "";
}
