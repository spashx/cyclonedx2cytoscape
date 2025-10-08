using CdxViz.Models.Cytoscape;
using System.Text.Json.Serialization;

namespace cdxviz.Models.Cytoscape;

public class ComponentNodeData : AbstractCytoscapeNodeData
{
    public ComponentNodeData()
    {
        Class = "component";
    }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("group")]
    public string Group { get; set; } = "";

    [JsonPropertyName("severity")]
    public string Severity { get; set; } = "none";

    [JsonPropertyName("topParent")]
    public bool IsTopParent { get; set; } = false;
}
