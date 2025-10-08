using CdxViz.Models.Cytoscape;
using System.Text.Json.Serialization;

namespace cdxviz.Models.Cytoscape;

public class LicenseNodeData : AbstractCytoscapeNodeData
{
    public LicenseNodeData()
    {
        Class = "license";
    }

    [JsonPropertyName("url")]
    public string Url { get; set; } = "";
}
